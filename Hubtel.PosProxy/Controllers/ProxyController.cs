using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hubtel.PosProxy.Helpers;
using Hubtel.PosProxy.Lib;
using Hubtel.PosProxy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Hubtel.PosProxy.Controllers
{
    [Route("api/proxy")]
    [ApiController]
    public class ProxyController : BaseController
    {
        private readonly IProxyHttpClient _proxyHttpClient;
        private readonly ILogger<ProxyController> _logger;

        public ProxyController(IProxyHttpClient proxyHttpClient, ILogger<ProxyController> logger)
        {
            _proxyHttpClient = proxyHttpClient;
            _logger = logger;
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> RelayRequest([FromBody] ProxyRequest proxyRequest)
        {
            var schemeToken = RequestHeaderHelper.GetSchemeAndToken(Request);

            try
            {
                using (var response = await MakeHttpRequest(proxyRequest, schemeToken.Key, schemeToken.Value))
                {
                    return await ActionResultAsync(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Pos proxy failed: " + ex.Message);
            }
            return BadRequest();
        }

        private async Task<HttpResponseMessage> MakeHttpRequest(ProxyRequest proxyRequest, string scheme, string accessToken)
        {
            if (proxyRequest.Method.ToLower() == "get")
            {
                return await _proxyHttpClient.GetAsync(proxyRequest.Url, scheme, accessToken);
            }

            if (proxyRequest.Method.ToLower() == "post")
            {
                return await _proxyHttpClient.PostAsync(proxyRequest.Url, proxyRequest.RequestBody, scheme,
                    accessToken);
            }

            if (proxyRequest.Method.ToLower() == "put")
            {
                return await _proxyHttpClient.PutAsync(proxyRequest.Url, proxyRequest.RequestBody, scheme,
                    accessToken);
            }

            if (proxyRequest.Method.ToLower() == "delete")
            {
                return await _proxyHttpClient.DeleteAsync(proxyRequest.Url, scheme, accessToken);
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

    }
    
}
