using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Helpers
{
    public class RequestHeaderHelper
    {
        public static KeyValuePair<string, string> GetSchemeAndToken(HttpRequest Request)
        {
            // Get Authorization header value
            if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
            {
                return new KeyValuePair<string, string>();
            }
            var authToken = authorization.First();
            var raw = authToken.Split(' ');

            if (raw.Length != 2)
            {
                return new KeyValuePair<string, string>();
            }

            return new KeyValuePair<string, string>(raw[0], raw[1]);
        }
    }
}
