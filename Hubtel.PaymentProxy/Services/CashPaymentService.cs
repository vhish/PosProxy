using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.PaymentProxy.Constants;
using Hubtel.PaymentProxy.Extensions;
using Hubtel.PaymentProxy.Helpers;
using Hubtel.PaymentProxy.Models;
using Hubtel.PaymentProxy.Models.ApiRequests;
using Hubtel.PaymentProxyData.Constants;
using Hubtel.PaymentProxyData.Core;
using Hubtel.PaymentProxyData.EntityModels;
using Hubtel.PaymentProxyData.Repositories;
using Microsoft.AspNetCore.Http;

namespace Hubtel.PaymentProxy.Services
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
