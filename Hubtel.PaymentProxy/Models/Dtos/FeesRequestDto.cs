using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Dtos
{
    public class FeesRequestDto
    {
        public decimal Amount { get; set; }
        public string MomoPhoneNumber { get; set; }
        public bool? ChargeCustomer { get; set; }
    }
}
