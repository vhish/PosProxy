using Hubtel.PaymentProxy.Models.ApiResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Dtos
{
    public class FeesResponseDto
    {
        public decimal Amount { get; set; }
        public string MomoPhoneNumber { get; set; }
        public bool ChargeCustomer { get; set; }
        public MnpData CustomerProfile { get; set; }
        public MomoFee MomoFee { get; set; }
    }
    
    /*public class MnpLookupResponse
    {
        public int StatusCode { get; set; }
        public MnpData Data { get; set; }
    }*/

    public class MnpData
    {
        public string Name { get; set; }
        public string NetworkCode { get; set; }
        public string Channel { get; set; }
        public string Ported { get; set; }
        public string MobileNumber { get; set; }
        public string WalletName { get; set; }
        public string Code { get; set; }
        public string FailedMessage { get; set; }
    }

    /*public class Customer
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Network { get; set; }
        public string NetworkId { get; set; }
    }


    public class FeeResponse
    {
        public int StatusCode { get; set; }
        public FeeObject Data { get; set; }
    }

    public class FeeObject
    {
        public object State { get; set; }
        public decimal Total { get; set; }
        public decimal Fee { get; set; }
        public decimal TaxAmount { get; set; }
        public object DiscountAmount { get; set; }
    }*/
}
