using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models
{
    public class PaymentTypeConfiguration : IPaymentTypeConfiguration
    {
        public List<PaymentType> PaymentTypes { get; set; }
        public List<Channel> Channels { get; set; }
    }

    public interface IPaymentTypeConfiguration
    {
        List<PaymentType> PaymentTypes { get; set; }
        List<Channel> Channels { get; set; }
    }

    public class PaymentType
    {
        public string Type { get; set; }
        public string Class { get; set; }
        public bool? RequireMsisdn { get; set; }
        public bool? CheckStatus { get; set; }
        public string PrimaryCallbackUrl { get; set; }
        public string SecondaryCallbackUrl { get; set; }
        public string ApplicationAlias { get; set; }
    }

    public class Channel
    {
        public string Name { get; set; }
        public bool? Requiretoken { get; set; }
    }
}
