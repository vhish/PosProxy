using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Responses
{
    public class MomoFeeResponse
    {
        public string ResponseCode { get; set; }
        public MomoFee Data { get; set; }
    }

    public class MomoFee
    {
        public decimal Amount { get; set; }
        public decimal Charges { get; set; }
        public decimal AmountAfterCharges { get; set; }
        public decimal AmountCharged { get; set; }
    }
}
