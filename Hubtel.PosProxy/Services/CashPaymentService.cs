using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.PosProxy.Constants;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Requests;
using Hubtel.PosProxyData.Constants;
using Hubtel.PosProxyData.EntityModels;
using Hubtel.PosProxyData.Repositories;

namespace Hubtel.PosProxy.Services
{
    public class CashPaymentService : PaymentService, ICashPaymentService
    {
        private readonly IUnifiedSalesService _unifiedSalesService;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        public CashPaymentService(IUnifiedSalesService unifiedSalesService, IPaymentRequestRepository paymentRequestRepository)
        {
            _unifiedSalesService = unifiedSalesService;
            _paymentRequestRepository = paymentRequestRepository;
        }

        public override bool CheckStatus()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            paymentRequest.Status = En.PaymentStatus.SUCCESSFUL;
            await RecordPaymentAsync(paymentRequest).ConfigureAwait(false);

            return true;
        }

        public override async Task<bool> RecordPaymentAsync(PaymentRequest paymentRequest)
        {
            var orderPaymentRequest = OrderPaymentRequest.ToOrderPaymentRequest(paymentRequest);
            var response = await _unifiedSalesService.RecordPaymentAsync(orderPaymentRequest).ConfigureAwait(false);
            await _paymentRequestRepository.SetToSuccessfulAsync(paymentRequest.ClientReference).ConfigureAwait(false);

            return true;
        }
    }

    public interface ICashPaymentService : IPaymentService
    {

    }
}
