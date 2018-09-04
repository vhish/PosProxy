using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Responses
{
    public class UnifiedSalesErrorResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public object Data { get; set; }
    }

    public class UnifiedSalesValidationErrorResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public List<UnifiedSalesValidationError> Data { get; set; }
    }

    public class UnifiedSalesValidationError
    {
        public string Field { get; set; }
        public List<string> Messages { get; set; }
    }
}
