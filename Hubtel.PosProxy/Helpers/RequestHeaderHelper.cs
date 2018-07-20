using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static string GetHeaderValue(HttpRequest Request, string Key)
        {
            if (!Request.Headers.TryGetValue(Key, out var headerValue))
            {
                return "";
            }
            return headerValue;
        }

        public static bool TryGetBasicAuthToken(string clientId, string secret, out string authToken)
        {
            try
            {
                authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ':' + secret));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                authToken = "";
                return false;
            }
            return true;
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
