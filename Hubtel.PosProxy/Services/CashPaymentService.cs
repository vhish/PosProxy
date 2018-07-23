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
            IPaymentRequestRepository paymentRequestRepository) : base(unifiedSalesService, paymentRequestRepository)
        {
            _unifiedSalesService = unifiedSalesService;
            _paymentRequestRepository = paymentRequestRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<bool> CheckStatusAsync(PaymentRequest paymentRequest)
        {
            return true;
        }

        public override async Task<bool> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            paymentRequest.Status = En.PaymentStatus.SUCCESSFUL;
            await RecordPaymentAsync(paymentRequest).ConfigureAwait(false);

            return true;
        }

        /*public override async Task<bool> RecordPaymentAsync(PaymentRequest paymentRequest)
        {
            //var user = _httpContextAccessor.HttpContext.User;
            //var accountId = UserHelper.GetAccountId(user);
            var accountId = paymentRequest.AccountId;

            var orderPaymentRequest = OrderPaymentRequest.ToOrderPaymentRequest(paymentRequest);
            var response = await _unifiedSalesService.RecordPaymentAsync(orderPaymentRequest, accountId).ConfigureAwait(false);
            if(response != null)
            {
                paymentRequest = await _paymentRequestRepository.UpdateAsync(paymentRequest, paymentRequest.Id).ConfigureAwait(false);
                return true;
            }
            return false;
        }*/
    }

    public interface ICashPaymentService : IPaymentService
    {

    }
}
