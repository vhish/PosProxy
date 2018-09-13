using AutoMapper;
using FluentValidation.AspNetCore;
using Hubtel.PaymentProxy.Constants;
using Hubtel.PaymentProxy.Extensions;
using Hubtel.PaymentProxy.Filters;
using Hubtel.PaymentProxy.Helpers;
using Hubtel.PaymentProxy.Models;
using Hubtel.PaymentProxy.Models.ApiResponses;
using Hubtel.PaymentProxy.Models.Dtos;
using Hubtel.PaymentProxy.Models.ApiRequests;
using Hubtel.PaymentProxy.Models.ApiResponses;
using Hubtel.PaymentProxy.Models.Validators;
using Hubtel.PaymentProxy.Services;
using Hubtel.PaymentProxyData.Constants;
using Hubtel.PaymentProxyData.EntityModels;
using Hubtel.PaymentProxyData.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Controllers
{
    [Authorize]
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : BaseController
    {
        private readonly ICardPaymentService _cardPaymentService;
        private readonly IMomoPaymentService _momoPaymentService;
        private readonly IHubtelMePaymentService _hubtelMePaymentService;
        private readonly IMapper _mapper;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly IServiceProvider _provider;
        private readonly IPaymentTypeConfiguration _paymentTypeConfiguration;
        private readonly IUnifiedSalesService _unifiedSalesService;
        private readonly IMerchantAccountService _merchantAccountService;
        private readonly ICustomerProfileService _customerProfileService;

        public PaymentsController(ICardPaymentService cardPaymentService,
            IMomoPaymentService momoPaymentService, IMapper mapper,
            IPaymentRequestRepository paymentRequestRepository, IServiceProvider provider,
            IPaymentTypeConfiguration paymentTypeConfiguration, IUnifiedSalesService unifiedSalesService,
            IMerchantAccountService merchantAccountService, ICustomerProfileService customerProfileService)
        {
            _mapper = mapper;
            _paymentRequestRepository = paymentRequestRepository;
            _cardPaymentService = cardPaymentService;
            _momoPaymentService = momoPaymentService;
            _provider = provider;
            _paymentTypeConfiguration = paymentTypeConfiguration;
            _unifiedSalesService = unifiedSalesService;
            _merchantAccountService = merchantAccountService;
            _customerProfileService = customerProfileService;
        }

        [BenchmarkAttributeFilter, HttpPost, Route("momo-fee")]
        public async Task<IActionResult> GetMomoFees([FromBody] FeesRequestDto payload)
        {
            if(payload == null)
            {
                ModelState.AddModelError("fees", "The fees request cannot be blank");
                return BadRequest(Responses.ErrorResponse<FeesResponseDto>(ModelState.ToErrors(), StatusMessage.ValidationErrors, ResponseCodes.VALIDATION_ERRORS));
            }

            var feesValidator = new FeesRequestValidator();
            feesValidator.Validate(payload).AddToModelState(ModelState, null);
            if (!ModelState.IsValid) return BadRequest(Responses.ErrorResponse<FeesResponseDto>(ModelState.ToErrors(), StatusMessage.ValidationErrors, ResponseCodes.VALIDATION_ERRORS));

            var phone = payload.MomoPhoneNumber.AsPhoneNumber();
            if (phone == null)
            {
                ModelState.AddModelError("phone", "The phone number is not valid");
                return BadRequest(Responses.ErrorResponse<FeesResponseDto>(ModelState.ToErrors(), StatusMessage.ValidationErrors, ResponseCodes.VALIDATION_ERRORS));
            }

            var accountId = UserHelper.GetAccountId(User);

            var customer = await _customerProfileService.GetCustomerProfile(phone).ConfigureAwait(false);

            if(customer.Channel == null)
            {
                return NotFound(Responses.ErrorResponse(null, new FeesResponseDto(), "The phone number network code was not found", ResponseCodes.NOT_FOUND));
            }

            var fees = await _merchantAccountService.GetMomoFeesAsync(payload.Amount, customer.Channel, 
                payload.ChargeCustomer.Value, accountId).ConfigureAwait(false);

            if(fees == null)
            {
                return NotFound(Responses.ErrorResponse(null, new FeesResponseDto(), "Error getting fees", ResponseCodes.EXTERNAL_ERROR));
            }

            var feeResponse = new FeesResponseDto
            {
                Amount = payload.Amount,
                ChargeCustomer = payload.ChargeCustomer.Value,
                MomoPhoneNumber = payload.MomoPhoneNumber,
                CustomerProfile = customer,
                MomoFee = fees.Data
            };
            return Ok(Responses.SuccessResponse(StatusMessage.Found, feeResponse, ResponseCodes.SUCCESS));
        }

        [BenchmarkAttributeFilter, HttpPost, Route("")]
        public async Task<IActionResult> Create([FromBody] OrderRequestDto payload)
        {
            TextInfo caseFormat = new CultureInfo("en-US", false).TextInfo;

            var accountId = UserHelper.GetAccountId(User);

            if (payload == null)
            {
                ModelState.AddModelError("order", "The order cannot be blank");
                return BadRequest(Responses.ErrorResponse<PaymentRequest>(ModelState.ToErrors(), StatusMessage.ValidationErrors, ResponseCodes.VALIDATION_ERRORS));
            }

            //call a function that assigns the customer, branch and employee from an order to the payment if empty on the payment
            payload = setCustomerDataOnPayment(payload);

            var payment = payload?.Payment;
            if (payment == null)
            {
                ModelState.AddModelError("payment", "There is no payment attached to the order");
                return BadRequest(Responses.ErrorResponse<PaymentRequest>(ModelState.ToErrors(), StatusMessage.ValidationErrors, ResponseCodes.VALIDATION_ERRORS));
            }

            var paymentValidator = new PaymentRequestValidator(_paymentTypeConfiguration);
            paymentValidator.Validate(payment).AddToModelState(ModelState, null);
            if (!ModelState.IsValid) return BadRequest(Responses.ErrorResponse<PaymentRequest>(ModelState.ToErrors(), StatusMessage.ValidationErrors, ResponseCodes.VALIDATION_ERRORS));

            var orderValidator = new OrderRequestValidator();
            orderValidator.Validate(payload).AddToModelState(ModelState, null);
            if (!ModelState.IsValid) return BadRequest(Responses.ErrorResponse<PaymentRequest>(ModelState.ToErrors(), StatusMessage.ValidationErrors, ResponseCodes.VALIDATION_ERRORS));

            var orderRequest = _mapper.Map<OrderRequest>(payload);
            
            var paymentRequest = _mapper.Map<PaymentRequest>(payment);
            paymentRequest.OrderRequestDoc = JsonConvert.SerializeObject(orderRequest);
            paymentRequest = await _paymentRequestRepository.AddAsync(paymentRequest).ConfigureAwait(false);

            var paymentTypeClassName = $"Hubtel.PaymentProxy.Services.{caseFormat.ToTitleCase(payment.PaymentType.ToLower())}PaymentService";
            var paymentService = (PaymentService)ActivatorUtilities.CreateInstance(_provider, Type.GetType(paymentTypeClassName));
            //-->
            var processPaymentResult = await paymentService.ProcessPayment(paymentRequest).ConfigureAwait(false);
            if (processPaymentResult.Success)
            {
                processPaymentResult.Data.OrderRequestDoc = null;
                return Ok(processPaymentResult);
            }
            await _paymentRequestRepository.DeleteByClientReferenceAsync(paymentRequest.ClientReference).ConfigureAwait(false);
            processPaymentResult.Data = null;
            return BadRequest(processPaymentResult);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ServiceFilter(typeof(IpAttributeFilter))]
        [AllowAnonymous]
        [HttpPost, Route("card-callback")]
        public async Task<IActionResult> CardCallback([FromBody] CardCallbackRequestDto payload)
        {
            if (payload.Data?.ClientReference != null)
            {
                var paymentRequest = FinalizePaymentRequest(payload.Data.ClientReference, payload.Data.TransactionId, 
                    payload.ResponseCode);
                if(paymentRequest != null)
                {
                    if (paymentRequest.Status.Equals(En.PaymentStatus.SUCCESSFUL) || paymentRequest.Status.Equals(En.PaymentStatus.FAILED))
                    {
                        paymentRequest.MergeCardCallbackData(payload);
                        var response = await _cardPaymentService.RecordPaymentAsync(paymentRequest).ConfigureAwait(false);
                    }
                }
            }
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ServiceFilter(typeof(IpAttributeFilter))]
        [AllowAnonymous]
        [HttpPost, Route("momo-callback")]
        public async Task<IActionResult> MomoCallback([FromBody] MomoCallbackRequestDto payload)
        {
            if(payload.Data?.ClientReference != null)
            {
                var paymentRequest = FinalizePaymentRequest(payload.Data.ClientReference, payload.Data.TransactionId, 
                    payload.ResponseCode);
                if (paymentRequest != null)
                {
                    if (paymentRequest.Status.Equals(En.PaymentStatus.SUCCESSFUL) || paymentRequest.Status.Equals(En.PaymentStatus.FAILED))
                    {
                        paymentRequest.MergeMomoCallbackData(payload);
                        var response = await _momoPaymentService.RecordPaymentAsync(paymentRequest).ConfigureAwait(false);
                    }
                }
            }
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ServiceFilter(typeof(IpAttributeFilter))]
        [AllowAnonymous]
        [HttpPost, Route("hubtelme-callback")]
        public async Task<IActionResult> HubtelMeCallback([FromBody] HubtelMeCallbackRequestDto payload)
        {
            if (payload.Data?.ClientReference != null)
            {
                var paymentRequest = FinalizePaymentRequest(payload.Data.ClientReference, payload.Data.TransactionId,
                    payload.ResponseCode);
                if (paymentRequest != null)
                {
                    if (paymentRequest.Status.Equals(En.PaymentStatus.SUCCESSFUL) || paymentRequest.Status.Equals(En.PaymentStatus.FAILED))
                    {
                        paymentRequest.MergeHubtelMeCallbackData(payload);
                        var response = await _hubtelMePaymentService.RecordPaymentAsync(paymentRequest).ConfigureAwait(false);
                    }
                }
                    
            }
            return Ok();
        }

        private PaymentRequest FinalizePaymentRequest(string clientReference, string transactionId, string responseCode)
        {
            var paymentRequest = _paymentRequestRepository.Find(x => x.ClientReference == clientReference);
            if (paymentRequest != null)
            {
                if (responseCode.Equals(ResponseCodes.PAYMENT_SUCCESSFUL))
                {
                    paymentRequest.Status = En.PaymentStatus.SUCCESSFUL;
                    paymentRequest.IsSuccessful = true;
                }
                else
                {
                    paymentRequest.Status = En.PaymentStatus.FAILED;
                    paymentRequest.IsSuccessful = false;
                }
            }
            return paymentRequest;
        }

        private OrderRequestDto setCustomerDataOnPayment(OrderRequestDto payload)
        {
            var payment = payload?.Payment;
            if (payment == null)
            {
                return payload;
            }
            if (String.IsNullOrEmpty(payment.CustomerName))
            {
                payment.CustomerName = payload.CustomerName;
            }
            if (String.IsNullOrEmpty(payment.BranchName))
            {
                payment.BranchName = payload.BranchName;
            }
            if (String.IsNullOrEmpty(payment.BranchId))
            {
                payment.BranchId = payload.BranchId;
            }
            if (String.IsNullOrEmpty(payment.EmployeeName))
            {
                payment.EmployeeName = payload.EmployeeName;
            }
            if (String.IsNullOrEmpty(payment.EmployeeId))
            {
                payment.EmployeeId = payload.EmployeeId;
            }

            return payload;
        }
    }
}
