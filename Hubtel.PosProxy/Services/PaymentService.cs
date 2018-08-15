using Hubtel.PosProxy.Extensions;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Requests;
using Hubtel.PosProxy.Models.Responses;
using Hubtel.PosProxyData.Core;
using Hubtel.PosProxyData.EntityModels;
using Hubtel.PosProxyData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Services
{
    public abstract class PaymentService : IPaymentService
    {
        private readonly IUnifiedSalesService _unifiedSalesService;
        private readonly IPaymentRequestRepository _paymentRequestRepository;

        public PaymentService(IUnifiedSalesService unifiedSalesService, IPaymentRequestRepository paymentRequestRepository)
        {
            _unifiedSalesService = unifiedSalesService;
            _paymentRequestRepository = paymentRequestRepository;
        }

        public abstract Task<HubtelPosProxyResponse<PaymentRequest>> CheckStatusAsync(PaymentRequest paymentRequest);
        public abstract Task<HubtelPosProxyResponse<PaymentRequest>> ProcessPayment(PaymentRequest paymentRequest);

        public async Task<HubtelPosProxyResponse<OrderPaymentResponse>> RecordPaymentAsync(PaymentRequest paymentRequest)
        {
            var accountId = paymentRequest.AccountId;

            //var orderPaymentRequest = OrderPaymentRequest.ToOrderPaymentRequest(paymentRequest);
            var response = await _unifiedSalesService.RecordPaymentAsync(paymentRequest, accountId).ConfigureAwait(false);
            
            return response;
        }
    }

    public interface IPaymentService
    {
        Task<HubtelPosProxyResponse<PaymentRequest>> ProcessPayment(PaymentRequest paymentRequest);
        Task<HubtelPosProxyResponse<OrderPaymentResponse>> RecordPaymentAsync(PaymentRequest paymentRequest);
        Task<HubtelPosProxyResponse<PaymentRequest>> CheckStatusAsync(PaymentRequest paymentRequest);
    }
}
