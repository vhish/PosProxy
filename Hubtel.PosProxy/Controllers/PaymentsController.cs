using AutoMapper;
using Hubtel.PosProxy.Constants;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Dtos;
using Hubtel.PosProxy.Services;
using Hubtel.PosProxyData.Constants;
using Hubtel.PosProxyData.EntityModels;
using Hubtel.PosProxyData.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IMapper _mapper;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly IServiceProvider _provider;

        public PaymentsController(ICardPaymentService cardPaymentService,
            IMomoPaymentService momoPaymentService, IMapper mapper,
            IPaymentRequestRepository paymentRequestRepository)
        {
            _mapper = mapper;
            _paymentRequestRepository = paymentRequestRepository;
            _cardPaymentService = cardPaymentService;
            _momoPaymentService = momoPaymentService;
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentRequestDto payload)
        {
            TextInfo caseFormat = new CultureInfo("en-US", false).TextInfo;

            var paymentRequest = _mapper.Map<PaymentRequest>(payload);
            paymentRequest = await _paymentRequestRepository.AddAsync(paymentRequest).ConfigureAwait(false);

            var paymentTypeClassName = $"{caseFormat.ToTitleCase(payload.PaymentType.ToLower())}PaymentService";
            var r = Type.GetType(paymentTypeClassName);
            var paymentService = (PaymentService)ActivatorUtilities.CreateInstance(_provider, Type.GetType(paymentTypeClassName));
            if (await paymentService.ProcessPaymentAsync(paymentRequest).ConfigureAwait(false))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost, Route("card-callback")]
        public async Task<IActionResult> CardCallback([FromBody] CardCallbackRequestDto payload)
        {
            if (payload.Data?.ClientReference != null)
            {
                var paymentRequest = GetPaymentRequest(payload.Data.ClientReference, payload.ResponseCode);
                var response = await _cardPaymentService.RecordPaymentAsync(paymentRequest).ConfigureAwait(false);
            }
            return Ok();
        }

        [HttpPost, Route("momo-callback")]
        public async Task<IActionResult> MomoCallback([FromBody] MomoCallbackRequestDto payload)
        {
            if(payload.Data?.ClientReference != null)
            {
                var paymentRequest = GetPaymentRequest(payload.Data.ClientReference, payload.ResponseCode);
                var response = await _momoPaymentService.RecordPaymentAsync(paymentRequest).ConfigureAwait(false);
            }
            return Ok();
        }

        private PaymentRequest GetPaymentRequest(string clientReference, string responseCode)
        {
            var paymentRequest = _paymentRequestRepository.Find(x => x.ClientReference == clientReference);

            if (responseCode.Equals(ResponseCodes.PAYMENT_SUCCESSFUL))
                paymentRequest.Status = En.PaymentStatus.SUCCESSFUL;
            else
                paymentRequest.Status = En.PaymentStatus.FAILED;

            return paymentRequest;
        }
    }
}
