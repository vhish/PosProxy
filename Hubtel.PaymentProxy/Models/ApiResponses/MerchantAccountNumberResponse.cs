using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.ApiResponses
{
    public class MerchantAccountNumberResponse
    {
        public string ResponseCode { get; set; }
        public MerchantAccountNumber Data { get; set; }
    }

    public class MerchantAccountNumber
    {
        public string AccountNumber { get; set; }
        public string BusinessName { get; set; }
        public string Status { get; set; }
        public bool HasMerchantAccount { get; set; }
    }
}
