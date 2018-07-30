using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.PosProxy.Constants;
using Hubtel.PosProxy.Extensions;
using Hubtel.PosProxy.Helpers;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Requests;
using Hubtel.PosProxyData.Constants;
using Hubtel.PosProxyData.Core;
using Hubtel.PosProxyData.EntityModels;
using Hubtel.PosProxyData.Repositories;
using Microsoft.AspNetCore.Http;

namespace Hubtel.PosProxy.Services
{
    public class CashPaymentService : PaymentService, ICashPaymentService
    {
        private readonly IUnifiedSalesService _unifiedSalesService;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CashPaymentService(IUnifiedSalesService unifiedSalesService, IHttpContextAccessor httpContextAccessor,
            IPaymentRequestRepository paymentRequestRepository) : base(unifiedSalesService, paymentRequestRepository)
        {
            _unifiedSalesService = unifiedSalesService;
            _paymentRequestRepository = paymentRequestRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<HubtelPosProxyResponse<PaymentRequest>> CheckStatusAsync(PaymentRequest paymentRequest)
        {
            return new HubtelPosProxyResponse<PaymentRequest>();
        }

        public override async Task<HubtelPosProxyResponse<PaymentRequest>> ProcessPayment(PaymentRequest paymentRequest)
        {
            paymentRequest.Status = En.PaymentStatus.SUCCESSFUL;
            var orderResponse = await RecordPaymentAsync(paymentRequest).ConfigureAwait(false);
            if (orderResponse.Success)
            {
                paymentRequest = await _paymentRequestRepository.UpdateAsync(paymentRequest, paymentRequest.Id).ConfigureAwait(false);
                return Responses.SuccessResponse(StatusMessage.Found, paymentRequest, ResponseCodes.SUCCESS);
            }
            return Responses.ErrorResponse(orderResponse.Errors, new PaymentRequest(), orderResponse.Message, ResponseCodes.EXTERNAL_ERROR);
        }
        
    }

    public interface ICashPaymentService : IPaymentService
    {

    }
}
