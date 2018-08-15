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
        public Guid Id { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue { get; set; }
        public string OrderNumber { get; set; }
    }
}
