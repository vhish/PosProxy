using Hubtel.PaymentProxy.Lib;
using Hubtel.PaymentProxy.Models;
using Hubtel.PaymentProxy.Models.ApiResponses;
using Hubtel.PaymentProxy.Models.Dtos;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Services
{
    public class CustomerProfileService : ICustomerProfileService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;
        private readonly IMnpHttpClient _mnpHttpClient;
        private readonly IProfilerHttpClient _profilerHttpClient;
        private readonly ILogger<CustomerProfileService> _logger;
        private readonly DistributedCacheEntryOptions _cacheEntryOptions;
        private readonly IPaymentTypeConfiguration _paymentTypeConfiguration;

        public CustomerProfileService(IDistributedCache distributedCache,
            IConfiguration configuration,
            IMnpHttpClient mnpHttpClient,
            IProfilerHttpClient profilerHttpClient,
            ILogger<CustomerProfileService> logger,
            IPaymentTypeConfiguration paymentTypeConfiguration)
        {
            var cacheDuration = Convert.ToInt32(configuration["MnpApi:CacheExpiryDurationMinutes"]);
            _distributedCache = distributedCache;
            _configuration = configuration;
            _mnpHttpClient = mnpHttpClient;
            _profilerHttpClient = profilerHttpClient;
            _logger = logger;
            _paymentTypeConfiguration = paymentTypeConfiguration;
            _cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(cacheDuration));
        }

        /// <summary>
        /// Get the Customer profile 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<MnpData> GetCustomerProfile(string phoneNumber)
        {
            var mnpResultTask = MnpLookupAsync($"+{phoneNumber}");
            var nameResultTask = NameLookup(phoneNumber);

            var mnp = await mnpResultTask;
            if (mnp == null) return null;

            var customerProfile = new MnpData
            {
                MobileNumber = mnp.InternationalFormat.Replace("+", ""),
                NetworkCode = mnp.CurrentCarrier.NetworkCode,
                Name = mnp.CurrentCarrier.Name,
                WalletName = await nameResultTask,
                Channel = _paymentTypeConfiguration.Channels.FirstOrDefault(x => x.NetworkCode == mnp.CurrentCarrier.NetworkCode)?.Name
            };
            return customerProfile;
        }

        /// <summary>
        /// Mnp Lookup
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private async Task<MnpApiResponse> MnpLookupAsync(string phoneNumber)
        {
            var mnpResponse = _distributedCache.GetString($"MNP:{phoneNumber.Replace("+", "")}");
            if (mnpResponse != null)
            {
                //return JsonConvert.DeserializeObject<MnpApiResponse>(mnpResponse);
            }

            var mnpLookupApi = _configuration["MnpApi:BaseUrl"];

            try
            {
                _logger.LogInformation($"Requesting for MNP info on {phoneNumber}");
                using (var response = await _mnpHttpClient.GetAsync($"{mnpLookupApi}/{phoneNumber}"))
                {
                    _logger.LogInformation($"Received MNP info for {phoneNumber}");
                    if (response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var obj = JsonConvert.DeserializeObject<MnpApiResponse>(body);

                        await _distributedCache.SetStringAsync($"MNP:{phoneNumber}", JsonConvert.SerializeObject(obj), _cacheEntryOptions);

                        return obj;
                    }
                    _logger.LogDebug(response.ReasonPhrase);
                }
            }
            catch (TimeoutException)
            {
                _logger.LogError($"MNPLookup timeout for this phone number {phoneNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError("MNPLookup failed: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Namelookup
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private async Task<string> NameLookup(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace("+", "");
            var profilerResponse = _distributedCache.GetString($"NAME:{phoneNumber}");
            if (profilerResponse != null)
            {
                //return profilerResponse;
            }

            var profilerApi = _configuration["ProfilerApi:BaseUrl"];

            try
            {
                _logger.LogInformation($"Requesting for Profile info on {phoneNumber}");
                using (var response = await _profilerHttpClient.GetAsync($"{profilerApi}/{phoneNumber}"))
                {
                    _logger.LogInformation($"Received Profile for {phoneNumber}");
                    if (response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var obj = JsonConvert.DeserializeObject<ProfilerApiResponse>(body);
                        if (obj.Success)
                        {
                            NameLookupData name = obj.Data;
                            if (name.Names.Count == 0)
                            {
                                return "";
                            }
                            var foundName = name.Names.OrderByDescending(c => c.Score).First().Name;

                            await _distributedCache.SetStringAsync($"NAME:{phoneNumber}", foundName, _cacheEntryOptions);

                            return foundName;
                        }
                    }

                }
            }
            catch (TimeoutException)
            {
                _logger.LogError($"Name Lookup timeout for this phone number {phoneNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Name Lookup failed: " + ex.Message);
            }
            return string.Empty;
        }
    }

    public interface ICustomerProfileService
    {
        Task<MnpData> GetCustomerProfile(string phoneNumber);
    }
}
