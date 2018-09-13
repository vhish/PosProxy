using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.ApiResponses
{
    public class ProfilerApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public NameLookupData Data { get; set; }
    }

    public class NameLookupData
    {
        public string PhoneNumber { get; set; }
        public List<NameLookupNames> Names { get; set; }
    }

    public class NameLookupNames
    {
        public string Name { get; set; }
        public decimal Score { get; set; }
    }
}
