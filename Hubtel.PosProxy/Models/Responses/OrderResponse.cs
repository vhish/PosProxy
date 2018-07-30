using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Responses
{
  
    public class OrderResponseWrapper
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public OrderResponse Data { get; set; }
        public object Errors { get; set; }
    }

    public class OrderResponse
    {
        public string Token { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
