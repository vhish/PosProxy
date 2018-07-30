using AutoMapper;
using FluentValidation.AspNetCore;
using Hubtel.PosProxy.Constants;
using Hubtel.PosProxy.Extensions;
using Hubtel.PosProxy.Filters;
using Hubtel.PosProxy.Helpers;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Dtos;
using Hubtel.PosProxy.Models.Requests;
using Hubtel.PosProxy.Models.Validators;
using Hubtel.PosProxy.Services;
using Hubtel.PosProxyData.Constants;
using Hubtel.PosProxyData.EntityModels;
using Hubtel.PosProxyData.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Controllers
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

        public PaymentsController(ICardPaymentService cardPaymentService,
            IMomoPaymentService momoPaymentService, IMapper mapper,
            IPaymentRequestRepository paymentRequestRepository, IServiceProvider provider,
            IPaymentTypeConfiguration paymentTypeConfiguration, IUnifiedSalesService unifiedSalesService)
        {
            _mapper = mapper;
            _paymentRequestRepository = paymentRequestRepository;
            _cardPaymentService = cardPaymentService;
            _momoPaymentService = momoPaymentService;
            _provider = provider;
            _paymentTypeConfiguration = paymentTypeConfiguration;
            _unifiedSalesService = unifiedSalesService;
        }

        [BenchmarkAttributeFilter, HttpPost, Route("")]
        public async Task<IActionResult> Create([FromBody] CreateTransactionRequestDto payload)
        {
            TextInfo caseFormat = new CultureInfo("en-US", false).TextInfo;

            var accountId = UserHelper.GetAccountId(User);

            if (payload.Order != null)
            {
                var orderValidator = new OrderRequestValidator();
                orderValidator.Validate(payload.Order).AddToModelState(ModelState, null);
                if (!ModelState.IsValid) return BadRequest(Responses.ErrorResponse<PaymentRequest>(ModelState.ToErrors(), StatusMessage.ValidationErrors, ResponseCodes.VALIDATION_ERRORS));

                var orderRequest = _mapper.Map<OrderRequest>(payload.Order);
                var orderResult = await _unifiedSalesService.RecordOrderAsync(orderRequest, accountId).ConfigureAwait(false);
                if (!orderResult.Success)
                {
                    orderResult.Data = null;
                    return BadRequest(orderResult);
                }
                payload.Payment.SalesOrderId = orderResult.Data.Token;
            }

            var paymentValidator = new PaymentRequestValidator(_paymentTypeConfiguration);
            paymentValidator.Validate(payload.Payment).AddToModelState(ModelState, null);
            if (!ModelState.IsValid) return BadRequest(Responses.ErrorResponse<PaymentRequest>(ModelState.ToErrors(), StatusMessage.ValidationErrors, ResponseCodes.VALIDATION_ERRORS));

            var paymentRequest = _mapper.Map<PaymentRequest>(payload.Payment);
            paymentRequest = await _paymentRequestRepository.AddAsync(paymentRequest).ConfigureAwait(false);

            var paymentTypeClassName = $"Hubtel.PosProxy.Services.{caseFormat.ToTitleCase(payload.Payment.PaymentType.ToLower())}PaymentService";
            var paymentService = (PaymentService)ActivatorUtilities.CreateInstance(_provider, Type.GetType(paymentTypeClassName));
            //-->
            var processPaymentResult = await paymentService.ProcessPayment(paymentRequest).ConfigureAwait(false);
            if (processPaymentResult.Success)
            {
                return Ok(processPaymentResult);
            }
            await _paymentRequestRepository.DeleteByClientReferenceAsync(paymentRequest.ClientReference).ConfigureAwait(false);
            processPaymentResult.Data = null;
            return BadRequest(processPaymentResult);
        }
        
        [ServiceFilter(typeof(IpAttributeFilter))]
        [AllowAnonymous]
        [HttpPost, Route("card-callback")]
        public async Task<IActionResult> CardCallback([FromBody] CardCallbackRequestDto payload)
        {
            if (payload.Data?.ClientReference != null)
            {
                var paymentRequest = GetPaymentRequest(payload.Data.ClientReference, payload.Data.TransactionId, 
                    payload.ResponseCode);
                var response = await _cardPaymentService.RecordPaymentAsync(paymentRequest).ConfigureAwait(false);
            }
            return Ok();
        }

        [ServiceFilter(typeof(IpAttributeFilter))]
        [AllowAnonymous]
        [HttpPost, Route("momo-callback")]
        public async Task<IActionResult> MomoCallback([FromBody] MomoCallbackRequestDto payload)
        {
            if(payload.Data?.ClientReference != null)
            {
                var paymentRequest = GetPaymentRequest(payload.Data.ClientReference, payload.Data.TransactionId, 
                    payload.ResponseCode);
                if(paymentRequest.Status.Equals(En.PaymentStatus.SUCCESSFUL) || paymentRequest.Status.Equals(En.PaymentStatus.FAILED))
                {
                    var response = await _momoPaymentService.RecordPaymentAsync(paymentRequest).ConfigureAwait(false);
                }
            }
            return Ok();
        }

        [ServiceFilter(typeof(IpAttributeFilter))]
        [AllowAnonymous]
        [HttpPost, Route("hubtelme-callback")]
        public async Task<IActionResult> HubtelMeCallback([FromBody] HubtelMeCallbackRequestDto payload)
        {
            if (payload.Data?.ClientReference != null)
            {
                var paymentRequest = GetPaymentRequest(payload.Data.ClientReference, payload.Data.TransactionId,
                    payload.ResponseCode);
                if (paymentRequest.Status.Equals(En.PaymentStatus.SUCCESSFUL) || paymentRequest.Status.Equals(En.PaymentStatus.FAILED))
                {
                    var response = await _hubtelMePaymentService.RecordPaymentAsync(paymentRequest).ConfigureAwait(false);
                }
            }
            return Ok();
        }

        private PaymentRequest GetPaymentRequest(string clientReference, string transactionId, string responseCode)
        {
            var paymentRequest = _paymentRequestRepository.Find(x => x.ClientReference == clientReference && 
            x.TransactionId == transactionId);
            if(paymentRequest != null)
            {
                if (responseCode.Equals(ResponseCodes.PAYMENT_SUCCESSFUL))
                    paymentRequest.Status = En.PaymentStatus.SUCCESSFUL;
                else
                    paymentRequest.Status = En.PaymentStatus.FAILED;
            }
            return paymentRequest;
        }
    }
}
