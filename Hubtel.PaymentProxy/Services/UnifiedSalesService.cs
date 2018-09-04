using Hubtel.PaymentProxy.Constants;
using Hubtel.PaymentProxy.Extensions;
using Hubtel.PaymentProxy.Helpers;
using Hubtel.PaymentProxy.Lib;
using Hubtel.PaymentProxy.Models;
using Hubtel.PaymentProxy.Models.Dtos;
using Hubtel.PaymentProxy.Models.Requests;
using Hubtel.PaymentProxy.Models.Responses;
using Hubtel.PaymentProxyData.EntityModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Services
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

        public async Task<HubtelPosProxyResponse<PaymentResponse>> RecordPaymentAsync(PaymentRequest paymentRequest, string accountId)
        {
            var url = $"{_unifiedSalesConfiguration.BaseUrl}/payments"; ///{orderPaymentRequest.SalesOrderId}/payment";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_unifiedSalesConfiguration.ApiKey, accountId);

            using (var response = await _unifiedSalesHttpClient.PostAsync(url, paymentRequest, _unifiedSalesConfiguration.Scheme, authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<PaymentResponse>(respData);
                    
                    _logger.LogDebug(respData);
                    return Responses.SuccessResponse(StatusMessage.Found, result, ResponseCodes.SUCCESS);
                }
                var error = JsonConvert.DeserializeObject<UnifiedSalesErrorResponse>(respData, new JsonSerializerSettings
                {
                    Error = HandleDeserializationError
                });
                if (error.Code == 4000)
                {
                    var validationError = JsonConvert.DeserializeObject<UnifiedSalesValidationErrorResponse>(respData, new JsonSerializerSettings
                    {
                        Error = HandleDeserializationError
                    });
                    return Responses.ErrorResponse(validationError.ToErrors(), new PaymentResponse(), error.Message, ResponseCodes.EXTERNAL_ERROR);
                }
                _logger.LogError(response.ReasonPhrase);
                _logger.LogError(respData);
                return Responses.ErrorResponse(error.ToErrors(), new PaymentResponse(), error.Message, ResponseCodes.EXTERNAL_ERROR);
            }
        }

        public async Task<HubtelPosProxyResponse<OrderResponse>> RecordOrderAsync(OrderRequest orderRequest, string accountId)
        {
            var url = $"{_unifiedSalesConfiguration.BaseUrl}/orders";

            var authToken = HubtelBasicAuthHelper.GenerateToken(_unifiedSalesConfiguration.ApiKey, accountId);

            using (var response = await _unifiedSalesHttpClient.PostAsync(url, orderRequest, _unifiedSalesConfiguration.Scheme, authToken))
            {
                var respData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<OrderResponseWrapper>(respData);

                    _logger.LogDebug(respData);
                    return Responses.SuccessResponse(StatusMessage.Found, result.Data, ResponseCodes.SUCCESS);
                }
                var error = JsonConvert.DeserializeObject<UnifiedSalesErrorResponse>(respData, new JsonSerializerSettings
                {
                    Error = HandleDeserializationError
                });
                if(error.Code == 4000)
                {
                    var validationError = JsonConvert.DeserializeObject<UnifiedSalesValidationErrorResponse>(respData, new JsonSerializerSettings
                    {
                        Error = HandleDeserializationError
                    });
                    return Responses.ErrorResponse(validationError.ToErrors(), new OrderResponse(), error.Message, ResponseCodes.EXTERNAL_ERROR);
                }
                _logger.LogError(response.ReasonPhrase);
                _logger.LogError(respData);
                return Responses.ErrorResponse(error.ToErrors(), new OrderResponse(), error.Message, ResponseCodes.EXTERNAL_ERROR);
            }
        }

        public void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;

            _logger.LogError(currentError);
        }
    }

    public interface IUnifiedSalesService
    {
        Task<HubtelPosProxyResponse<PaymentResponse>> RecordPaymentAsync(PaymentRequest paymentRequest, string accountId);
        Task<HubtelPosProxyResponse<OrderResponse>> RecordOrderAsync(OrderRequest orderRequest, string accountId);
    }
}
