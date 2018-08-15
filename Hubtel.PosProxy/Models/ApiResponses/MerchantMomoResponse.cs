using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Responses
{
    public class MerchantMomoResponse
    {
        public string ResponseCode { get; set; }
        public MerchantMomo Data { get; set; }
    }

    public class MerchantMomo
    {
        public decimal AmountAfterCharges { get; set; }
        public string TransactionId { get; set; }
        public string ClientReference { get; set; }
        public string Description { get; set; }
        public string ExternalTransactionId { get; set; }
        public decimal Amount { get; set; }
        public decimal Charges { get; set; }
        public object Meta { get; set; }
    }
    
}
