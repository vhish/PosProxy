using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Helpers
{
    public class BasicAuthTokenHelper
    {
        public static string GetBase64Token(string username, string password = null)
        {
            if (string.IsNullOrEmpty(password))
                return Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}"));
            return Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        }
    }
}
