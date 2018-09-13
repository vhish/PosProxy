using Hubtel.PaymentProxyData.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.ApiRequests
{
    public class MomoPaymentRequest
    {
        public string CustomerName { get; set; }
        public string CustomerMsisdn { get; set; }
        public string CustomerEmail { get; set; }
        public string Channel { get; set; }
        public decimal Amount { get; set; }
        public string PrimaryCallbackUrl { get; set; }
        public string SecondaryCallbackUrl { get; set; }
        public string Description { get; set; }
        public string ClientReference { get; set; }
        public string Token { get; set; }
        public bool FeesOnCustomer { get; set; }

        public static MomoPaymentRequest ToMomoPaymentRequest(PaymentRequest paymentRequest, 
            string primaryCallbackUrl, string secondaryCallbackUrl)
        {
            return new MomoPaymentRequest
            {
                Amount = decimal.Round(paymentRequest.AmountPaid, 2),
                Channel = paymentRequest.MomoChannel,
                CustomerMsisdn = paymentRequest.MomoPhoneNumber,
                CustomerName = paymentRequest.CustomerName,
                Description = paymentRequest.Description,
                Token = paymentRequest.MomoToken,
                ClientReference = paymentRequest.ClientReference,
                PrimaryCallbackUrl = primaryCallbackUrl,
                SecondaryCallbackUrl = secondaryCallbackUrl,
                FeesOnCustomer = paymentRequest.ChargeCustomer.HasValue ? paymentRequest.ChargeCustomer.Value : true
            };
        }
    }
}
