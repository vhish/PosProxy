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
using Microsoft.Extensions.Logging;

namespace Hubtel.PosProxy.Services
{
    public class MomoPaymentService : PaymentService, IMomoPaymentService
    {
        private readonly IMerchantAccountService _merchantAccountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly IUnifiedSalesService _unifiedSalesService;
        private readonly ILogger _logger;

        public MomoPaymentService(IMerchantAccountService merchantAccountService, 
            IPaymentRequestRepository paymentRequestRepository, IUnifiedSalesService unifiedSalesService,
            ILogger<MomoPaymentService> logger, IHttpContextAccessor httpContextAccessor) : base(unifiedSalesService, paymentRequestRepository)
        {
            _merchantAccountService = merchantAccountService;
            _paymentRequestRepository = paymentRequestRepository;
            _unifiedSalesService = unifiedSalesService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<bool> CheckStatusAsync(PaymentRequest paymentRequest)
        {
            var accountId = paymentRequest.AccountId;
            var response = await _merchantAccountService.CheckTransactionStatusAsync(paymentRequest, accountId).ConfigureAwait(false);
            if(response != null)
            {
                paymentRequest.Status = response.Status ? En.PaymentStatus.SUCCESSFUL : En.PaymentStatus.FAILED;
                await RecordPaymentAsync(paymentRequest).ConfigureAwait(false);
                return true;
            }
            return false;
        }

        public override async Task<bool> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            //var user = _httpContextAccessor.HttpContext.User;
            //var accountId = UserHelper.GetAccountId(user);
            var accountId = paymentRequest.AccountId;

            var response = await _merchantAccountService.ChargeMomoAsync(paymentRequest, accountId).ConfigureAwait(false);
            
            if(response != null && response.Equals(ResponseCodes.PAYMENT_REQUEST_SUCCESSFUL))
            {
                paymentRequest.TransactionId = response.Data.TransactionId;
                paymentRequest.ExternalTransactionId = response.Data.ExternalTransactionId;
                paymentRequest.AmountAfterCharges = response.Data.AmountAfterCharges;
                paymentRequest.Charges = response.Data.Charges;

                paymentRequest = await _paymentRequestRepository.UpdateAsync(paymentRequest, paymentRequest.Id).ConfigureAwait(false);
                _logger.LogDebug("Momo:ProcessPayment: request succeeded.");
                return true;
            }
            _logger.LogError("Momo:ProcessPayment: request failed.");
            return false;
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

    public interface IMomoPaymentService : IPaymentService
    {

    }
}
