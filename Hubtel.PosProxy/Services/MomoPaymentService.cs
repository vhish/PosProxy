using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.PosProxy.Constants;
using Hubtel.PosProxy.Helpers;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Requests;
using Hubtel.PosProxyData.EntityModels;
using Hubtel.PosProxyData.Repositories;
using Microsoft.AspNetCore.Http;

namespace Hubtel.PosProxy.Services
{
    public class MomoPaymentService : PaymentService, IMomoPaymentService
    {
        private readonly IMerchantAccountService _merchantAccountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly IUnifiedSalesService _unifiedSalesService;

        public MomoPaymentService(IMerchantAccountService merchantAccountService, 
            IPaymentRequestRepository paymentRequestRepository, IUnifiedSalesService unifiedSalesService)
        {
            _merchantAccountService = merchantAccountService;
            _paymentRequestRepository = paymentRequestRepository;
            _unifiedSalesService = unifiedSalesService;
        }

        public override bool CheckStatus()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var accountId = UserHelper.GetAccountId(user);

            var accountNumber = await _merchantAccountService.FindAccountNumberAsync(accountId).ConfigureAwait(false);
            var response = await _merchantAccountService.ChargeMomoAsync(accountNumber, paymentRequest).ConfigureAwait(false);
            
            if(response != null && response.Equals(ResponseCodes.PAYMENT_REQUEST_SUCCESSFUL))
            {
                paymentRequest.TransactionId = response.Data.TransactionId;
                paymentRequest.ExternalTransactionId = response.Data.ExternalTransactionId;
                paymentRequest.AmountAfterCharges = response.Data.AmountAfterCharges;
                paymentRequest.Charges = response.Data.Charges;
            }

            paymentRequest= await _paymentRequestRepository.UpdateAsync(paymentRequest, paymentRequest.Id).ConfigureAwait(false);
            
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

    public interface IMomoPaymentService : IPaymentService
    {

    }
}
