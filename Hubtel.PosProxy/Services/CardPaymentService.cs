using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.PosProxy.Constants;
using Hubtel.PosProxy.Extensions;
using Hubtel.PosProxy.Helpers;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Requests;
using Hubtel.PosProxyData.Constants;
using Hubtel.PosProxyData.Core;
using Hubtel.PosProxyData.EntityModels;
using Hubtel.PosProxyData.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Hubtel.PosProxy.Services
{
    public class CardPaymentService : PaymentService, ICardPaymentService
    {
        private readonly IMerchantAccountService _merchantAccountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly IUnifiedSalesService _unifiedSalesService;
        private readonly ILogger _logger;

        public CardPaymentService(IMerchantAccountService merchantAccountService,
            IPaymentRequestRepository paymentRequestRepository, IUnifiedSalesService unifiedSalesService,
            ILogger<CardPaymentService> logger) : base(unifiedSalesService, paymentRequestRepository)
        {
            _merchantAccountService = merchantAccountService;
            _paymentRequestRepository = paymentRequestRepository;
            _unifiedSalesService = unifiedSalesService;
            _logger = logger;
        }

        public override async Task<HubtelPosProxyResponse<PaymentRequest>> CheckStatusAsync(PaymentRequest paymentRequest)
        {
            var accountId = paymentRequest.AccountId;
            var response = await _merchantAccountService.CheckTransactionStatusAsync(paymentRequest, accountId).ConfigureAwait(false);
            if (response != null)
            {
                paymentRequest.SetStatus(response.Data.Data[0].TransactionStatus);

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

            var response = await _merchantAccountService.ChargeCardAsync(paymentRequest, accountId).ConfigureAwait(false);

            if (response != null && response.Equals(ResponseCodes.PAYMENT_REQUEST_SUCCESSFUL))
            {
                paymentRequest.TransactionId = response.Data.Data.TransactionId;
                paymentRequest.TransactionSession = response.Data.Data.TransactionSession.TransactionSessionId;

                paymentRequest = await _paymentRequestRepository.UpdateAsync(paymentRequest, paymentRequest.Id).ConfigureAwait(false);

                _logger.LogDebug("Card:ProcessPayment: request succeeded.");
                return Responses.SuccessResponse(StatusMessage.Created, paymentRequest, ResponseCodes.SUCCESS);
            }
            _logger.LogError("Card:ProcessPayment: request failed.");
            return Responses.ErrorResponse(response.Errors, new PaymentRequest(), response.Message, ResponseCodes.EXTERNAL_ERROR);
        }
    }

    public interface ICardPaymentService : IPaymentService
    {

    }
}
