using Hubtel.PosProxy.Helpers;
using Hubtel.PosProxy.Lib;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxy.Models.Requests;
using Hubtel.PosProxy.Models.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Services
{
    public class UnifiedSalesService : IUnifiedSalesService
    {
        private readonly IUnifiedSalesHttpClient _unifiedSalesHttpClient;
        private readonly IUnifiedSalesConfiguration _unifiedSalesConfiguration;
        private readonly ILogger _logger;

        public UnifiedSalesService(IUnifiedSalesHttpClient unifiedSalesHttpClient,
            IUnifiedSalesConfiguration unifiedSalesConfiguration, ILogger<UnifiedSalesService> logger)
        {
            _unifiedSalesConfiguration = unifiedSalesConfiguration;
            _unifiedSalesHttpClient = unifiedSalesHttpClient;
            _logger = logger;
        }

        public async Task<OrderPaymentResponse> RecordPaymentAsync(OrderPaymentRequest orderPaymentRequest, string accountId)
        {
            var url = $"{_unifiedSalesConfiguration.BaseUrl}";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_unifiedSalesConfiguration.ApiKey, accountId);

            using (var response = await _unifiedSalesHttpClient.PostAsync(url, orderPaymentRequest, "Basic", authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<OrderPaymentResponse>(respData);

                    _logger.LogDebug(respData);
                    return result;
                }
                _logger.LogError(response.ReasonPhrase);
            }
            return new OrderPaymentResponse();
        }
    }

    public interface IUnifiedSalesService
    {
        Task<OrderPaymentResponse> RecordPaymentAsync(OrderPaymentRequest orderPaymentRequest, string accountId);
    }
}
