using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.ApiResponses
{
    public class MerchantHubtelMeResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public HubtelMePaymentData Data { get; set; }
        public dynamic Errors { get; set; }
    }

    public class HubtelMePaymentData {

        public string ApplicationTransactionId { get; set; }
        public string ApplicationAccountId { get; set; }
        public string SalesInvoiceToken { get; set; }
        public string Status { get; set; }
        public string RecipientMerchantAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
