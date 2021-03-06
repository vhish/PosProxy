﻿using Microsoft.Extensions.Configuration;
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
    public sealed class ProfilerHttpClientBase
    {
        private static volatile HttpClient instance;
        private static object syncRoot = new object();

        private ProfilerHttpClientBase() { }

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

    public class ProfilerHttpClient : IProfilerHttpClient
    {
        private HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _basicAuth;
        private readonly int _timeout;

        public ProfilerHttpClient(ILogger<ProfilerHttpClient> logger, IConfiguration configuration)
        {
            _httpClient = ProfilerHttpClientBase.Instance;
            _logger = logger;

            //_timeout = Convert.ToInt32(configuration["ProfilerApi:TimeoutSeconds"]);
            _basicAuth = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{configuration["ProfilerApi:ClientId"]}:{configuration["ProfilerApi:ClientSecret"]}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuth);
            //_httpClient.Timeout = TimeSpan.FromSeconds(_timeout);
        }

        public HttpClient GetHttpClientBase()
        {
            return _httpClient;
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
            string authorizationScheme = "", string token = "")
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

        public async Task<HttpResponseMessage> GetAsync(string requestUri, string authorizationScheme = "", string token = "")
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
            if (!string.IsNullOrEmpty(authorizationScheme) && !string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorizationScheme, token);
            var response = await _httpClient.PutAsync(requestUri, httpContent);
            return response;
        }

        public async Task<HttpResponseMessage> MakePostRequestAsync(string requestUri, HttpContent httpContent,
            string authorizationScheme, string token)
        {
            if (!string.IsNullOrEmpty(authorizationScheme) && !string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(authorizationScheme, token);
            var response = await _httpClient.PostAsync(requestUri, httpContent);
            return response;
        }

        private async Task<HttpResponseMessage> MakeGetRequestAsync(string requestUri, string authorizationScheme,
            string token)
        {
            if (!string.IsNullOrEmpty(authorizationScheme) && !string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(authorizationScheme, token);
            var response = await _httpClient.GetAsync(requestUri);
            return response;
        }

        private async Task<HttpResponseMessage> MakeDeleteRequestAsync(string requestUri, string authorizationScheme,
            string token)
        {
            if (!string.IsNullOrEmpty(authorizationScheme) && !string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(authorizationScheme, token);
            var response = await _httpClient.DeleteAsync(requestUri);
            return response;
        }
    }

    public interface IProfilerHttpClient
    {
        Task<HttpResponseMessage> PutAsync<T>(string requestUri, T payload,
            string authorizationScheme, string token);
        Task<HttpResponseMessage> PostAsync<T>(string requestUri, T payload,
            string authorizationScheme = "", string token = "");
        Task<HttpResponseMessage> GetAsync(string requestUri, string authorizationScheme = "", string token = "");
        Task<HttpResponseMessage> DeleteAsync(string requestUri, string authorizationScheme, string token);
        HttpClient GetHttpClientBase();
    }
}
