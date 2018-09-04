using Hubtel.PaymentProxyData.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Requests
{
    public class HubtelMePaymentRequest
    {
        public string ApplicationAlias { get; set; }
        public string SalesInvoiceToken { get; set; }
        public string PaymentCallBackUrl { get; set; }
        public string ClientReference { get; set; }
        public decimal Amount { get; set; }
        public string ApplicationAccountId { get; set; }
        public string Description { get; set; }

        public static HubtelMePaymentRequest ToHubtelMePaymentRequest(PaymentRequest paymentRequest,
            string paymentCallbackUrl, string applicationAlias)
        {
            return new HubtelMePaymentRequest
            {
                Amount = decimal.Round(paymentRequest.AmountPaid, 2),
                ApplicationAccountId = paymentRequest.MomoPhoneNumber,
                ApplicationAlias = applicationAlias,
                Description = paymentRequest.Description,
                ClientReference = paymentRequest.ClientReference,
                PaymentCallBackUrl = paymentCallbackUrl,
                SalesInvoiceToken = paymentRequest.OrderId.ToString()
            };
        }
    }
}
