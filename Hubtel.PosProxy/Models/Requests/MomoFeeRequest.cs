using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Requests
{
    public class MomoFeeRequest
    {
        public string Channel { get; set; }
        public decimal Amount { get; set; }
        public bool FeesOnCustomer { get; set; }
    }
}
