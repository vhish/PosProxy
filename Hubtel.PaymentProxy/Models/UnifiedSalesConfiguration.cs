using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models
{
    public class UnifiedSalesConfiguration : IUnifiedSalesConfiguration
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string Scheme { get; set; }
    }

    public interface IUnifiedSalesConfiguration
    {
        string BaseUrl { get; set; }
        string ApiKey { get; set; }
        string Scheme { get; set; }
    }
}
