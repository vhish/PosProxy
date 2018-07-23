using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Requests;
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

        public abstract Task<bool> CheckStatusAsync(PaymentRequest paymentRequest);
        public abstract Task<bool> ProcessPaymentAsync(PaymentRequest paymentRequest);

        public async Task<bool> RecordPaymentAsync(PaymentRequest paymentRequest)
        {
            var accountId = paymentRequest.AccountId;

            var orderPaymentRequest = OrderPaymentRequest.ToOrderPaymentRequest(paymentRequest);
            var response = await _unifiedSalesService.RecordPaymentAsync(orderPaymentRequest, accountId).ConfigureAwait(false);
            if (response != null)
            {
                paymentRequest = await _paymentRequestRepository.UpdateAsync(paymentRequest, paymentRequest.Id).ConfigureAwait(false);
                return true;
            }
            return false;
        }
    }

    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(PaymentRequest paymentRequest);
        Task<bool> RecordPaymentAsync(PaymentRequest paymentRequest);
        Task<bool> CheckStatusAsync(PaymentRequest paymentRequest);
    }
}
