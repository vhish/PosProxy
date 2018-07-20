using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.PosProxy.Constants;
using Hubtel.PosProxy.Helpers;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Requests;
using Hubtel.PosProxyData.Constants;
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
            IPaymentRequestRepository paymentRequestRepository)
        {
            _unifiedSalesService = unifiedSalesService;
            _paymentRequestRepository = paymentRequestRepository;
            _httpContextAccessor = httpContextAccessor;
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
            var user = _httpContextAccessor.HttpContext.User;
            var accountId = UserHelper.GetAccountId(user);

            var orderPaymentRequest = OrderPaymentRequest.ToOrderPaymentRequest(paymentRequest);
            var response = await _unifiedSalesService.RecordPaymentAsync(orderPaymentRequest, accountId).ConfigureAwait(false);
            paymentRequest = await _paymentRequestRepository.UpdateAsync(paymentRequest, paymentRequest.Id).ConfigureAwait(false);

            return true;
        }
    }

    public interface ICashPaymentService : IPaymentService
    {

    }
}
