using Hubtel.PosProxy.Helpers;
using Hubtel.PosProxy.Lib;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Responses;
using Hubtel.PosProxy.Models.Requests;
using Hubtel.PosProxyData.EntityModels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Services
{
    public class MerchantAccountService : IMerchantAccountService
    {
        private readonly IMerchantAccountHttpClient _merchantAccountHttpClient;
        private readonly IMerchantAccountConfiguration _merchantAccountConfiguration;
        private readonly ILogger _logger;
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _cacheOptions;

        public MerchantAccountService(IMerchantAccountHttpClient merchantAccountHttpClient,
            IMerchantAccountConfiguration merchantAccountConfiguration,
            ILogger<MerchantAccountService> logger, IDistributedCache cache)
        {
            _merchantAccountHttpClient = merchantAccountHttpClient;
            _merchantAccountConfiguration = merchantAccountConfiguration;
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
            return accountNumber;
        }

        private async Task<MerchantAccountNumberResponse> SearchAsync(string accountId)
        {
            var url = $"{_merchantAccountConfiguration.PrivateBaseUrl}/merchants/search?hubtelAccountId={accountId}";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);

            using (var response = await _merchantAccountHttpClient.GetAsync(url, _merchantAccountConfiguration.Scheme, authToken))
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
        
        public async Task<MerchantMomoResponse> ChargeMomoAsync(PaymentRequest paymentRequest, string accountId)
        {
            string merchantAccountNumber = await FindAccountNumberAsync(accountId).ConfigureAwait(false);
            var url = $"{_merchantAccountConfiguration.PrivateBaseUrl}/merchants/{merchantAccountNumber}/receive/mobilemoney";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);

            var momoPaymentRequest = MomoPaymentRequest.ToMomoPaymentRequest(paymentRequest,
                _merchantAccountConfiguration.MomoPrimaryCallbackUrl,
                _merchantAccountConfiguration.MomoSecondaryCallbackUrl);
            

            using (var response = await _merchantAccountHttpClient.PostAsync(url, momoPaymentRequest, "Basic", authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MerchantMomoResponse>(respData);

                    _logger.LogDebug(respData);
                    return result;
                }
                _logger.LogError(response.ReasonPhrase);
            }
            return new MerchantMomoResponse();
        }

        public async Task<MerchantCardResponse> ChargeCardAsync(PaymentRequest paymentRequest, string accountId)
        {
            string merchantAccountNumber = await FindAccountNumberAsync(accountId).ConfigureAwait(false);
            var url = $"{_merchantAccountConfiguration.PrivateBaseUrl}/merchants/{merchantAccountNumber}/receive/payworks";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);

            var cardPaymentRequest = CardPaymentRequest.ToCardPaymentRequest(paymentRequest,
                _merchantAccountConfiguration.CardPrimaryCallbackUrl,
                _merchantAccountConfiguration.CardSecondaryCallbackUrl);


            using (var response = await _merchantAccountHttpClient.PostAsync(url, cardPaymentRequest, "Basic", authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MerchantCardResponse>(respData);

                    _logger.LogDebug(respData);
                    return result;
                }
                _logger.LogError(response.ReasonPhrase);
            }
            return new MerchantCardResponse();
        }

        public async Task<MomoFeeResponse> GetMomoFeesAsync(PaymentRequest paymentRequest, string accountId)
        {
            string merchantAccountNumber = await FindAccountNumberAsync(accountId).ConfigureAwait(false);
            var url = $"{_merchantAccountConfiguration.PrivateBaseUrl}/merchants/{merchantAccountNumber}/charges/mobile/receive";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);

            var momoFeeRequest = new MomoFeeRequest
            {
                Amount = paymentRequest.Amount,
                Channel = paymentRequest.MomoChannel,
                FeesOnCustomer = paymentRequest.CustomerPaysFee
            };

            using (var response = await _merchantAccountHttpClient.PostAsync(url, momoFeeRequest, "Basic", authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MomoFeeResponse>(respData);

                    _logger.LogDebug(respData);
                    return result;
                }
                _logger.LogError(response.ReasonPhrase);
            }
            return new MomoFeeResponse();
        }

        public async Task<MomoChannelResponse> GetChannelsAsync(string accountId)
        {
            var url = $"{_merchantAccountConfiguration.PrivateBaseUrl}/channels";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);

            using (var response = await _merchantAccountHttpClient.GetAsync(url, "Basic", authToken))
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

        public async Task<MerchantTransactionCheckResponse> CheckTransactionStatusAsync(PaymentRequest paymentRequest, string accountId)
        {
            string merchantAccountNumber = await FindAccountNumberAsync(accountId).ConfigureAwait(false);
            var url = $"{_merchantAccountConfiguration.PublicBaseUrl}/merchants/{merchantAccountNumber}/transactions/status?" +
                $"hubtelTransactionId={paymentRequest.TransactionId}";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_merchantAccountConfiguration.ApiKey, accountId);

            using (var response = await _merchantAccountHttpClient.GetAsync(url, "Basic", authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<MerchantTransactionCheckResponse>(respData);

                    _logger.LogDebug(respData);
                    return result;
                }
                _logger.LogError(response.ReasonPhrase);
            }
            return new MerchantTransactionCheckResponse();
        }
    }

    public interface IMerchantAccountService
    {
        Task<MerchantMomoResponse> ChargeMomoAsync(PaymentRequest paymentRequest, string accountId);
        Task<MerchantCardResponse> ChargeCardAsync(PaymentRequest paymentRequest, string accountId);
        Task<MomoFeeResponse> GetMomoFeesAsync(PaymentRequest paymentRequest, string accountId);
        Task<MomoChannelResponse> GetChannelsAsync(string accountId);
        Task<MerchantTransactionCheckResponse> CheckTransactionStatusAsync(PaymentRequest paymentRequest, string accountId);
    }
}
