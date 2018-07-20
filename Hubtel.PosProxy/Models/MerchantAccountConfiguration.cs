﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models
{
    public class MerchantAccountConfiguration : IMerchantAccountConfiguration
    {
        public string PublicBaseUrl { get; set; }
        public string PrivateBaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string Scheme { get; set; }
        public string MomoPrimaryCallbackUrl { get; set; }
        public string MomoSecondaryCallbackUrl { get; set; }
        public string CardPrimaryCallbackUrl { get; set; }
        public string CardSecondaryCallbackUrl { get; set; }
    }

    public interface IMerchantAccountConfiguration
    {
        string PublicBaseUrl { get; set; }
        string PrivateBaseUrl { get; set; }
        string ApiKey { get; set; }
        string Scheme { get; set; }
        string MomoPrimaryCallbackUrl { get; set; }
        string MomoSecondaryCallbackUrl { get; set; }
        string CardPrimaryCallbackUrl { get; set; }
        string CardSecondaryCallbackUrl { get; set; }
    }
}
