using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.ApiResponses
{
    public class MnpApiResponse
    {
        public string Status { get; set; }
        public string InternationalFormat { get; set; }
        public LocalNumber LocalNumber { get; set; }
        public CurrentCarrier CurrentCarrier { get; set; }
        public OriginalCarrier OriginalCarrier { get; set; }
        public string Ported { get; set; }
    }

    public class LocalNumber
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public int CountryPrefix { get; set; }
        public string NationalFormat { get; set; }
    }
    public class CurrentCarrier
    {
        public string NetworkCode { get; set; }
        public string Country { get; set; }
        public string NetworkType { get; set; }
        public string Name { get; set; }
    }
    public class OriginalCarrier
    {
        public string NetworkCode { get; set; }
        public string Country { get; set; }
        public string NetworkType { get; set; }
        public string Name { get; set; }
    }
}
