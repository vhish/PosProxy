using Hubtel.PaymentProxy.Helpers;
using Hubtel.PaymentProxy.Lib;
using Hubtel.PaymentProxy.Models;
using Hubtel.PaymentProxy.Models.Responses;
using Hubtel.PaymentProxy.Models.Requests;
using Hubtel.PaymentProxyData.EntityModels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.PaymentProxy.Extensions;
using Hubtel.PaymentProxy.Constants;
using System.Diagnostics;

namespace Hubtel.PaymentProxy.Services
{
    public class MerchantAccountService : IMerchantAccountService
    {
        private readonly IMerchantAccountHttpClient _merchantAccountHttpClient;
        private readonly IMerchantAccountConfiguration _merchantAccountConfiguration;
        private readonly IPaymentTypeConfiguration _paymentTypeConfiguration;
        private readonly ILogger _logger;
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _cacheOptions;

        public MerchantAccountService(IMerchantAccountHttpClient merchantAccountHttpClient,
            IMerchantAccountConfiguration merchantAccountConfiguration, IPaymentTypeConfiguration paymentTypeConfiguration,
            ILogger<MerchantAccountService> logger, IDistributedCache cache)
        {
            _merchantAccountHttpClient = merchantAccountHttpClient;
            _merchantAccountConfiguration = merchantAccountConfiguration;
            _paymentTypeConfiguration = paymentTypeConfiguration;
            _logger = logger;
            _cache = cache;
            _cacheOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(72400))
                .SetSlidingExpiration(TimeSpan.FromMinutes(3600));
        }

        private async Task<string> FindAccountNumberAsync(string accountId)
        {
            //first search cache if not found go to api
            var accountNumber = await _cache.GetStringAsync(accountId).ConfigureAwait(false);
            if (string.IsNullOrEmpty(accountNumber))
            {
                var response = await SearchAsync(accountId).ConfigureAwait(false);
                if(response.Data != null)
                {
                    accountNumber = response.Data.AccountNumber;
                    await _cache.SetStringAsync(accountId, accountNumber, _cacheOptions).ConfigureAwait(false);
                }
            }
            return accountNumber ?? "";
        }

        private async Task<MerchantAccountNumberResponse> SearchAsync(string accountId)
        {
            var url = $"{_merchantAccountConfiguration.PrivateBaseUrl}/merchants/search?hubtelAccountId={accountId}";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);

            using (var response = await _merchantAccountHttpClient.GetAsync(url, _merchantAccountConfiguration.Scheme, 
                authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MerchantAccountNumberResponse>(respData);

                    _logger.LogDebug(respData);
                    return result;
                }
                _logger.LogError(response.ReasonPhrase);
            }
            return new MerchantAccountNumberResponse();
        }

        public async Task<HubtelPosProxyResponse<MerchantMomoResponse>> ChargeMomoAsync(PaymentRequest paymentRequest, string accountId)
        {
            string merchantAccountNumber = await FindAccountNumberAsync(accountId).ConfigureAwait(false);
            var url = $"{_merchantAccountConfiguration.PublicBaseUrl}/merchants/{merchantAccountNumber}/receive/mobilemoney";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);
            var paymentType = _paymentTypeConfiguration.PaymentTypes.Find(x => x.Type.ToLower().Equals(paymentRequest.PaymentType.ToLower()));
            var momoPaymentRequest = MomoPaymentRequest.ToMomoPaymentRequest(paymentRequest,
                $"{_merchantAccountConfiguration.CallbackBaseUrl}/{paymentType.PrimaryCallbackUrl}",
                $"{_merchantAccountConfiguration.CallbackBaseUrl}/{paymentType.SecondaryCallbackUrl}");
            
            using (var response = await _merchantAccountHttpClient.PostAsync(url, momoPaymentRequest, 
                _merchantAccountConfiguration.Scheme, authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MerchantMomoResponse>(respData);

                    _logger.LogDebug(respData);
                    return Responses.SuccessResponse(StatusMessage.Created, result, ResponseCodes.SUCCESS);
                }
                var error = GetMerchantAccountErrorResponse(respData, response.ReasonPhrase);
                _logger.LogError(response.ReasonPhrase);
                _logger.LogError(respData);
                return Responses.ErrorResponse(error.ToErrors(), new MerchantMomoResponse(), error.Message, ResponseCodes.EXTERNAL_ERROR);
            }
        }

        public async Task<HubtelPosProxyResponse<MerchantCardResponse>> ChargeCardAsync(PaymentRequest paymentRequest, string accountId)
        {
            string merchantAccountNumber = await FindAccountNumberAsync(accountId).ConfigureAwait(false);
            var url = $"{_merchantAccountConfiguration.PrivateBaseUrl}/merchants/{merchantAccountNumber}/receive/payworks";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);
            var paymentType = _paymentTypeConfiguration.PaymentTypes.Find(x => x.Type.ToLower().Equals(paymentRequest.PaymentType));
            var cardPaymentRequest = CardPaymentRequest.ToCardPaymentRequest(paymentRequest,
                $"{_merchantAccountConfiguration.CallbackBaseUrl}/{paymentType.PrimaryCallbackUrl}",
                $"{_merchantAccountConfiguration.CallbackBaseUrl}/{paymentType.SecondaryCallbackUrl}");


            using (var response = await _merchantAccountHttpClient.PostAsync(url, cardPaymentRequest,
                _merchantAccountConfiguration.Scheme, authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MerchantCardResponse>(respData);

                    _logger.LogDebug(respData);
                    return Responses.SuccessResponse(StatusMessage.Created, result, ResponseCodes.SUCCESS);
                }
                var error = GetMerchantAccountErrorResponse(respData, response.ReasonPhrase);
                _logger.LogError(response.ReasonPhrase);
                _logger.LogError(respData);
                return Responses.ErrorResponse(error.ToErrors(), new MerchantCardResponse(), error.Message, ResponseCodes.EXTERNAL_ERROR);
            }
        }

        public async Task<HubtelPosProxyResponse<MerchantHubtelMeResponse>> ChargeHubtelMeAsync(PaymentRequest paymentRequest, string accountId)
        {
            string merchantAccountNumber = await FindAccountNumberAsync(accountId).ConfigureAwait(false);
            var url = $"{_merchantAccountConfiguration.PrivateBaseUrl}/merchants/{merchantAccountNumber}/applications/initiate-payment";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);
            var paymentType = _paymentTypeConfiguration.PaymentTypes.Find(x => x.Type.ToLower().Equals(paymentRequest.PaymentType));
            var hubtelMePaymentRequest = HubtelMePaymentRequest.ToHubtelMePaymentRequest(paymentRequest,
                $"{_merchantAccountConfiguration.CallbackBaseUrl}/{paymentType.PrimaryCallbackUrl}", 
                paymentType.ApplicationAlias);

            using (var response = await _merchantAccountHttpClient.PostAsync(url, hubtelMePaymentRequest,
                _merchantAccountConfiguration.Scheme, authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MerchantHubtelMeResponse>(respData);

                    _logger.LogDebug(respData);
                    return Responses.SuccessResponse(StatusMessage.Created, result, ResponseCodes.SUCCESS);

                }
                var error = GetMerchantAccountErrorResponse(respData, response.ReasonPhrase);
                _logger.LogError(response.ReasonPhrase);
                _logger.LogError(respData);
                return Responses.ErrorResponse(error.ToErrors(), new MerchantHubtelMeResponse(), error.Message, ResponseCodes.EXTERNAL_ERROR);
            }
        }

        public async Task<MomoFeeResponse> GetMomoFeesAsync(PaymentRequest paymentRequest, string accountId)
        {
            string merchantAccountNumber = await FindAccountNumberAsync(accountId).ConfigureAwait(false);
            var url = $"{_merchantAccountConfiguration.PrivateBaseUrl}/merchants/{merchantAccountNumber}/charges/mobile/receive";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);

            var momoFeeRequest = new MomoFeeRequest
            {
                Amount = paymentRequest.AmountPaid,
                Channel = paymentRequest.MomoChannel,
                FeesOnCustomer = paymentRequest.ChargeCustomer ?? true
            };

            using (var response = await _merchantAccountHttpClient.PostAsync(url, momoFeeRequest,
                _merchantAccountConfiguration.Scheme, authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MomoFeeResponse>(respData);

                    _logger.LogDebug(respData);
                    return result;
                }
                var error = GetMerchantAccountErrorResponse(respData, response.ReasonPhrase);
                _logger.LogError(response.ReasonPhrase);
            }
            return new MomoFeeResponse();
        }

        public async Task<MomoChannelResponse> GetChannelsAsync(string accountId)
        {
            var url = $"{_merchantAccountConfiguration.PrivateBaseUrl}/channels";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);

            using (var response = await _merchantAccountHttpClient.GetAsync(url,
                _merchantAccountConfiguration.Scheme, authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MomoChannelResponse>(respData);

                    _logger.LogDebug(respData);
                    return result;
                }
                _logger.LogError(response.ReasonPhrase);
            }
            return new MomoChannelResponse();
        }

        public async Task<HubtelPosProxyResponse<MerchantTransactionCheckResponse>> CheckTransactionStatusAsync(PaymentRequest paymentRequest, string accountId)
        {
            string merchantAccountNumber = await FindAccountNumberAsync(accountId).ConfigureAwait(false);
            var url = $"{_merchantAccountConfiguration.PublicBaseUrl}/merchants/{merchantAccountNumber}/transactions/status?" +
                $"hubtelTransactionId={paymentRequest.TransactionId}";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);

            using (var response = await _merchantAccountHttpClient.GetAsync(url, 
                _merchantAccountConfiguration.Scheme, authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MerchantTransactionCheckResponse>(respData);

                    _logger.LogDebug(respData);
                    return Responses.SuccessResponse(StatusMessage.Found, result, ResponseCodes.SUCCESS);
                }
                var error = GetMerchantAccountErrorResponse(respData, response.ReasonPhrase);
                _logger.LogError(response.ReasonPhrase);
                _logger.LogError(respData);
                return Responses.ErrorResponse(error.ToErrors(), new MerchantTransactionCheckResponse(), error.Message, ResponseCodes.EXTERNAL_ERROR);
            }
        }

        public async Task<HubtelPosProxyResponse<MerchantTransactionCheckResponse>> CheckHubtelMeTransactionStatusAsync(PaymentRequest paymentRequest, string accountId)
        {
            var url = $"{_merchantAccountConfiguration.PublicBaseUrl}/merchants/applications/status/{paymentRequest.TransactionId}";
            
            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);

            using (var response = await _merchantAccountHttpClient.GetAsync(url,
                _merchantAccountConfiguration.Scheme, authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MerchantTransactionCheckResponse>(respData);

                    _logger.LogDebug(respData);
                    return Responses.SuccessResponse(StatusMessage.Found, result, ResponseCodes.SUCCESS);
                }
                var error = GetMerchantAccountErrorResponse(respData, response.ReasonPhrase);
                _logger.LogError(response.ReasonPhrase);
                _logger.LogError(respData);
                return Responses.ErrorResponse(error.ToErrors(), new MerchantTransactionCheckResponse(), error.Message, ResponseCodes.EXTERNAL_ERROR);
            }
        }

        private MerchantAccountErrorResponse GetMerchantAccountErrorResponse(string respData, string reasonPhrase)
        {
            var error = JsonConvert.DeserializeObject<MerchantAccountErrorResponse>(respData);
            if (error == null)
            {
                error = new MerchantAccountErrorResponse
                {
                    Message = $"MerchantAccount response : {reasonPhrase}"
                };
            }
            return error;
        }
    }

    public interface IMerchantAccountService
    {
        Task<HubtelPosProxyResponse<MerchantMomoResponse>> ChargeMomoAsync(PaymentRequest paymentRequest, string accountId);
        Task<HubtelPosProxyResponse<MerchantCardResponse>> ChargeCardAsync(PaymentRequest paymentRequest, string accountId);
        Task<HubtelPosProxyResponse<MerchantHubtelMeResponse>> ChargeHubtelMeAsync(PaymentRequest paymentRequest, string accountId);
        Task<MomoFeeResponse> GetMomoFeesAsync(PaymentRequest paymentRequest, string accountId);
        Task<MomoChannelResponse> GetChannelsAsync(string accountId);
        Task<HubtelPosProxyResponse<MerchantTransactionCheckResponse>> CheckTransactionStatusAsync(PaymentRequest paymentRequest, string accountId);
        Task<HubtelPosProxyResponse<MerchantTransactionCheckResponse>> CheckHubtelMeTransactionStatusAsync(PaymentRequest paymentRequest, string accountId);
    }
}
