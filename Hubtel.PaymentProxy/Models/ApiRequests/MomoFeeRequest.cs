using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.ApiRequests
{
    public class MomoFeeRequest
    {
        public string Channel { get; set; }
        public decimal Amount { get; set; }
        public bool FeesOnCustomer { get; set; }
    }
}
