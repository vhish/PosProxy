using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Lib
{
    public sealed class MerchantAccountHttpClientBase
    {
        private static volatile HttpClient instance;
        private static object syncRoot = new object();

        private MerchantAccountHttpClientBase() { }

        public static HttpClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new HttpClient
                            {
                                //Timeout = TimeSpan.FromSeconds(10)
                            };

                        }

                    }
                }

                return instance;
            }
        }

    }

    public class MerchantAccountHttpClient : IMerchantAccountHttpClient
    {
        private HttpClient _httpClient;
        private readonly ILogger _logger;

        public MerchantAccountHttpClient(ILogger<MerchantAccountHttpClient> logger)
        {
            _httpClient = MerchantAccountHttpClientBase.Instance;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string requestUri, T payload,
            string authorizationScheme, string token)
        {
            string postData = JsonConvert.SerializeObject(payload);
            HttpContent httpContent = new StringContent(postData, Encoding.UTF8, "application/json");

            var response = await MakePutRequestAsync(requestUri, httpContent, authorizationScheme, token);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(response.ReasonPhrase);
                var respData = await response.Content.ReadAsStringAsync();
                _logger.LogError(respData);
            }

            return response;
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string requestUri, T payload,
            string authorizationScheme, string token)
        {
            string postData = JsonConvert.SerializeObject(payload);
            HttpContent httpContent = new StringContent(postData, Encoding.UTF8, "application/json");

            var response = await MakePostRequestAsync(requestUri, httpContent, authorizationScheme, token);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(response.ReasonPhrase);
                var respData = await response.Content.ReadAsStringAsync();
                _logger.LogError(respData);
            }

            return response;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri, string authorizationScheme, string token)
        {
            var response = await MakeGetRequestAsync(requestUri, authorizationScheme, token);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(response.ReasonPhrase);
                var respData = await response.Content.ReadAsStringAsync();
                _logger.LogError(respData);
            }

            return response;

        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri, string authorizationScheme, string token)
        {
            var response = await MakeDeleteRequestAsync(requestUri, authorizationScheme, token);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(response.ReasonPhrase);
                var respData = await response.Content.ReadAsStringAsync();
                _logger.LogError(respData);
            }

            return response;
        }

        public async Task<HttpResponseMessage> MakePutRequestAsync(string requestUri, HttpContent httpContent,
            string authorizationScheme, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(authorizationScheme, token);
            var response = await _httpClient.PutAsync(requestUri, httpContent);
            return response;
        }

        public async Task<HttpResponseMessage> MakePostRequestAsync(string requestUri, HttpContent httpContent,
            string authorizationScheme, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(authorizationScheme, token);
            var response = await _httpClient.PostAsync(requestUri, httpContent);
            return response;
        }

        private async Task<HttpResponseMessage> MakeGetRequestAsync(string requestUri, string authorizationScheme,
            string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(authorizationScheme, token);
            var response = await _httpClient.GetAsync(requestUri);
            return response;
        }

        private async Task<HttpResponseMessage> MakeDeleteRequestAsync(string requestUri, string authorizationScheme,
            string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(authorizationScheme, token);
            var response = await _httpClient.DeleteAsync(requestUri);
            return response;
        }
    }

    public interface IMerchantAccountHttpClient
    {
        Task<HttpResponseMessage> PutAsync<T>(string requestUri, T payload,
            string authorizationScheme, string token);
        Task<HttpResponseMessage> PostAsync<T>(string requestUri, T payload,
            string authorizationScheme, string token);
        Task<HttpResponseMessage> GetAsync(string requestUri, string authorizationScheme, string token);
        Task<HttpResponseMessage> DeleteAsync(string requestUri, string authorizationScheme, string token);
    }
}
