using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Dtos
{
    public class MomoCallbackRequestDto
    {
        public string ResponseCode { get; set; }
        public MomoCallback Data { get; set; }
    }

    public class MomoCallback
    {
        public double AmountAfterCharges { get; set; }
        public string TransactionId { get; set; }
        public string ClientReference { get; set; }
        public string Description { get; set; }
        public string ExternalTransactionId { get; set; }
        public decimal Amount { get; set; }
        public double Charges { get; set; }
        public string Meta { get; set; }
    }
}
