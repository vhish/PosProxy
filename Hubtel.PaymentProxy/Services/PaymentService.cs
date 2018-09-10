using Hubtel.PaymentProxy.Extensions;
using Hubtel.PaymentProxy.Models;
using Hubtel.PaymentProxy.Models.Requests;
using Hubtel.PaymentProxy.Models.Responses;
using Hubtel.PaymentProxyData.Core;
using Hubtel.PaymentProxyData.EntityModels;
using Hubtel.PaymentProxyData.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Services
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

        public async Task<HubtelPosProxyResponse<PaymentResponse>> RecordPaymentAsync(PaymentRequest paymentRequest)
        {
            var accountId = paymentRequest.AccountId;
            var orderResult = await RecordOrderAsync(paymentRequest).ConfigureAwait(false);
            if (!orderResult.Success)
            {
                return new HubtelPosProxyResponse<PaymentResponse>
                {
                    Code = orderResult.Code,
                    Message = orderResult.Message,
                    Success = orderResult.Success,
                    Data = null,
                    Errors = orderResult.Errors
                };
            }
            paymentRequest.OrderId = orderResult.Data.Id;
            //var orderPaymentRequest = OrderPaymentRequest.ToOrderPaymentRequest(paymentRequest);
            var response = await _unifiedSalesService.RecordPaymentAsync(paymentRequest, accountId).ConfigureAwait(false);
            
            return response;
        }

        public async Task<HubtelPosProxyResponse<OrderResponse>> RecordOrderAsync(PaymentRequest paymentRequest)
        {
            var accountId = paymentRequest.AccountId;
            var orderRequest = JsonConvert.DeserializeObject<OrderRequest>(paymentRequest.OrderRequestDoc);
            var orderResult = await _unifiedSalesService.RecordOrderAsync(orderRequest, accountId).ConfigureAwait(false);
            return orderResult;
        }
    }

    public interface IPaymentService
    {
        Task<HubtelPosProxyResponse<PaymentRequest>> ProcessPayment(PaymentRequest paymentRequest);
        Task<HubtelPosProxyResponse<PaymentResponse>> RecordPaymentAsync(PaymentRequest paymentRequest);
        Task<HubtelPosProxyResponse<PaymentRequest>> CheckStatusAsync(PaymentRequest paymentRequest);
    }
}
