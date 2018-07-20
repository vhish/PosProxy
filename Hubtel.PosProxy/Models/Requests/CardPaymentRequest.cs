using Hubtel.PosProxyData.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Requests
{
    public class CardPaymentRequest
    {
        public string CustomerName { get; set; }
        public string CustomerMsisdn { get; set; }
        public string CustomerEmail { get; set; }
        public decimal Amount { get; set; }
        public string PrimaryCallbackUrl { get; set; }
        public string SecondaryCallbackUrl { get; set; }
        public string Description { get; set; }
        public string ClientReference { get; set; }

        public static CardPaymentRequest ToCardPaymentRequest(PaymentRequest paymentRequest,
            string primaryCallbackUrl, string secondaryCallbackUrl)
        {
            return new CardPaymentRequest
            {
                Amount = decimal.Round(paymentRequest.Amount, 2),
                CustomerMsisdn = paymentRequest.MomoPhoneNumber,
                CustomerName = paymentRequest.CustomerName,
                CustomerEmail = paymentRequest.CustomerEmail,
                Description = paymentRequest.Description,
                ClientReference = paymentRequest.ClientReference,
                PrimaryCallbackUrl = primaryCallbackUrl,
                SecondaryCallbackUrl = secondaryCallbackUrl
            };
        }
    }
}
