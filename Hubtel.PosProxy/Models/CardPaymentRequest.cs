using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models
{
    public class CardPaymentRequest
    {
        public string CustomerName { get; set; }
        public string CustomerMsisdn { get; set; }
        public string CustomerEmail { get; set; }
        public string Channel { get; set; }
        public string Amount { get; set; }
        public string PrimaryCallbackUrl { get; set; }
        public string SecondaryCallbackUrl { get; set; }
        public string Description { get; set; }
        public string ClientReference { get; set; }
        public string VodafoneToken { get; set; }
    }
}
