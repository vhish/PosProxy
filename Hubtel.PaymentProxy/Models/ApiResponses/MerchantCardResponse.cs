using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Responses
{
    public class MerchantCardResponse
    {
        public string ResponseCode { get; set; }
        public Data Data { get; set; }
    }
    public class TransactionSession
    {
        public string MerchantId { get; set; }
        public string MerchantSecretKey { get; set; }
        public string TransactionSessionId { get; set; }
    }

    public class Data
    {
        public string Description { get; set; }
        public string TransactionId { get; set; }
        public string ClientReference { get; set; }
        public TransactionSession TransactionSession { get; set; }
        public object Meta { get; set; }
        public object CardDetail { get; set; }
    }
}
