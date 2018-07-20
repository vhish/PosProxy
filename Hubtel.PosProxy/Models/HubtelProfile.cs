using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models
{
    public class HubtelProfile
    {
        public string AccountId { get; set; }

        public int AccountNumber { get; set; }

        public string Company { get; set; }

        public string PrimaryContact { get; set; }

        public string MobileNumber { get; set; }

        public string EmailAddress { get; set; }

        public decimal Balance { get; set; }

        public decimal Credit { get; set; }

        public decimal UnpostedBalance { get; set; }

        public int NumberOfServices { get; set; }

        public string LastAccessed { get; set; }

        public string AccountManager { get; set; }

        public string AccountStatus { get; set; }

        public string TimeZone { get; set; }

        public string CountryCode { get; set; }

        public string BillingCountry { get; set; }

        public string BillingCurrency { get; set; }
    }
}
