using Hubtel.PosProxyData.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Requests
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
        public string VodafoneToken { get; set; }

        public static MomoPaymentRequest ToMomoPaymentRequest(PaymentRequest paymentRequest, 
            string primaryCallbackUrl, string secondaryCallbackUrl)
        {
            return new MomoPaymentRequest
            {
                Amount = decimal.Round(paymentRequest.Amount, 2),
                Channel = paymentRequest.MomoChannel,
                CustomerMsisdn = paymentRequest.MomoPhoneNumber,
                CustomerName = paymentRequest.CustomerName,
                CustomerEmail = paymentRequest.CustomerEmail,
                Description = paymentRequest.Description,
                VodafoneToken = paymentRequest.MomoToken,
                ClientReference = paymentRequest.ClientReference,
                PrimaryCallbackUrl = primaryCallbackUrl,
                SecondaryCallbackUrl = secondaryCallbackUrl
            };
        }
    }
}
