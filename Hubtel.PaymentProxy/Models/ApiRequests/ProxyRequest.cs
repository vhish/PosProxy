﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.ApiRequests
{
    public class ProxyRequest
    {
        public string Url { get; set; }

        public string Method { get; set; }

        public object RequestBody { get; set; }
    }
}
