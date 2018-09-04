using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Helpers
{
    public static class UserHelper
    {
        public static string GetAccountId(this IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.Sid);
            return claim.Value;
        }

        public static string GetUserId(this IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.MobilePhone);
            return claim.Value;
        }

        public static string GetUserName(this IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.Name);
            return claim?.Value;
        }

        public static string GetRole(this IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.Role);
            return claim?.Value;
        }
    }
}
