using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Responses
{
    public class UnifiedSalesErrorResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public List<UnifiedSalesError> Errors { get; set; }
        public object Guid { get; set; }
    }

    public class UnifiedSalesError
    {
        public int Code { get; set; }
        public string Field { get; set; }
        public string DeveloperMessage { get; set; }
        public string UserMessage { get; set; }
        public object MoreInfo { get; set; }
    }
}
