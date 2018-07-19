using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models
{
    public class UnifiedSalesConfiguration : IUnifiedSalesConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scheme { get; set; }
    }

    public interface IUnifiedSalesConfiguration
    {
        string BaseUrl { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string Scheme { get; set; }
    }
}
