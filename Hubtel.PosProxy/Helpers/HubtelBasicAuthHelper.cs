using Hubtel.PosProxy.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Helpers
{
    public class HubtelBasicAuthHelper
    {
        public static string GenerateToken(string apikey, string accountId)
        {
            dynamic hubtelAccount = new ExpandoObject();
            hubtelAccount.AccountId = accountId;
            var hubtelAccountJson = JsonConvert.SerializeObject(hubtelAccount);

            return Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apikey}:{hubtelAccountJson}"));
        }
    }
}
