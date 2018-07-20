using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Authentication
{
    public class CustomAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "Bearer";
        public string Scheme => DefaultScheme;
        public StringValues AuthKey { get; set; }
    }
}
