using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.PaymentProxy.Constants;
using Hubtel.PaymentProxy.Extensions;
using Hubtel.PaymentProxy.Helpers;
using Hubtel.PaymentProxy.Models;
using Hubtel.PaymentProxy.Models.Requests;
using Hubtel.PaymentProxyData.Constants;
using Hubtel.PaymentProxyData.Core;
using Hubtel.PaymentProxyData.EntityModels;
using Hubtel.PaymentProxyData.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Hubtel.PaymentProxy.Services
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

        public override async Task<HubtelPosProxyResponse<PaymentRequest>> CheckStatusAsync(PaymentRequest paymentRequest)
        {
            var accountId = paymentRequest.AccountId;
            var response = await _merchantAccountService.CheckTransactionStatusAsync(paymentRequest, accountId).ConfigureAwait(false);
            if (response != null)
            {
                if (response.Data.Data[0].TransactionStatus.ToLower().Equals("success"))
                    paymentRequest.Status = En.PaymentStatus.SUCCESSFUL;
                else if (response.Data.Data[0].TransactionStatus.ToLower().Equals("failed"))
                    paymentRequest.Status = En.PaymentStatus.FAILED;

                var orderResponse = await RecordPaymentAsync(paymentRequest).ConfigureAwait(false);
                if (orderResponse.Success)
                {
                    return Responses.SuccessResponse(StatusMessage.Found, paymentRequest, ResponseCodes.SUCCESS);
                }
            }
            return Responses.ErrorResponse(response.Errors, new PaymentRequest(), response.Message, ResponseCodes.EXTERNAL_ERROR);
        }

        public override async Task<HubtelPosProxyResponse<PaymentRequest>> ProcessPayment(PaymentRequest paymentRequest)
        {
            var accountId = paymentRequest.AccountId;

            var response = await _merchantAccountService.ChargeMomoAsync(paymentRequest, accountId).ConfigureAwait(false);

            if (response != null && response.Success && response.Data.ResponseCode.Equals(ResponseCodes.PAYMENT_REQUEST_SUCCESSFUL))
            {
                paymentRequest.TransactionId = response.Data.Data.TransactionId;
                paymentRequest.ExternalTransactionId = response.Data.Data.ExternalTransactionId;
                paymentRequest.AmountAfterCharges = response.Data.Data.AmountAfterCharges;
                paymentRequest.Charges = response.Data.Data.Charges;

                paymentRequest = await _paymentRequestRepository.UpdateAsync(paymentRequest, paymentRequest.Id).ConfigureAwait(false);
                _logger.LogDebug("Momo:ProcessPayment: request succeeded.");
                return Responses.SuccessResponse(StatusMessage.Created, paymentRequest, ResponseCodes.SUCCESS);
            }
            _logger.LogError("Momo:ProcessPayment: request failed.");
            return Responses.ErrorResponse(response.Errors, new PaymentRequest(), response.Data.Data.Description, ResponseCodes.EXTERNAL_ERROR);
        }

    }

    public interface IMomoPaymentService : IPaymentService
    {

    }
}
