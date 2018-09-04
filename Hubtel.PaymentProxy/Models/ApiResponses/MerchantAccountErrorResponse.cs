using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Responses
{
    public class MerchantAccountErrorResponse
    {
        public string ResponseCode { get; set; }
        public string Message { get; set; }
        public List<MerchantAccountError> Errors { get; set; }
    }

    public class MerchantAccountError
    {
        public string Field { get; set; }
        public List<string> Messages { get; set; }
    }
    
}
