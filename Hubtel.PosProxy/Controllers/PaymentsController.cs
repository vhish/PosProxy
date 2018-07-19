using AutoMapper;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Dtos;
using Hubtel.PosProxy.Services;
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
        private readonly IPaymentServiceFactory _paymentServiceFactory;
        private readonly IMapper _mapper;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly IServiceProvider _provider;

        public PaymentsController(IPaymentServiceFactory paymentServiceFactory, IMapper mapper,
            IPaymentRequestRepository paymentRequestRepository)
        {
            _paymentServiceFactory = paymentServiceFactory;
            _mapper = mapper;
            _paymentRequestRepository = paymentRequestRepository;
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentRequestDto payload)
        {
            TextInfo caseFormat = new CultureInfo("en-US", false).TextInfo;

            //assuming validation is done

            var paymentRequest = _mapper.Map<PaymentRequest>(payload);
            await _paymentRequestRepository.AddAsync(paymentRequest).ConfigureAwait(false);

            //var paymentMethod = _paymentServiceFactory.GetPaymentService(payload.PaymentType);
            //paymentMethod.ProcessPayment(paymentRequest);

            var paymentTypeClassName = $"{caseFormat.ToTitleCase(payload.PaymentType)}PaymentService";
            var paymentService = (IPaymentService)ActivatorUtilities.CreateInstance(_provider, Type.GetType(paymentTypeClassName));
            if (paymentService.ProcessPayment(paymentRequest))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
