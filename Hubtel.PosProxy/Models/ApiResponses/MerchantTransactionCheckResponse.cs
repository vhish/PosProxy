using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Responses
{
    public class MerchantTransactionCheckResponse
    {
        public string ResponseCode { get; set; }
        public List<MerchantTransactionCheck> Data { get; set; }
    }
    public class TransactionCycle
    {
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }

    public class MerchantTransactionCheck
    {
        public DateTime StartDate { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionId { get; set; }
        public string NetworkTransactionId { get; set; }
        public string InvoiceToken { get; set; }
        public string TransactionType { get; set; }
        public string PaymentMethod { get; set; }
        public string ClientReference { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal AmountAfterFees { get; set; }
        public string CardSchemeName { get; set; }
        public string CardNumber { get; set; }
        public string MobileNumber { get; set; }
        public string MobileChannelName { get; set; }
        public List<TransactionCycle> TransactionCycle { get; set; }
        public string RelatedTransactionId { get; set; }
        public string RelatedTransactionType { get; set; }
        public bool Disputed { get; set; }
        public decimal DisputedAmount { get; set; }
        public decimal DisputedAmountFee { get; set; }
        public decimal TotalAmountRefunded { get; set; }
        public string CustomerEmail { get; set; }
        public string BankName { get; set; }
        public string BIC { get; set; }
        public string ClearingInstitute { get; set; }
        public string CardTerminalId { get; set; }
        public string AuthorizationCode { get; set; }
        public string CardSource { get; set; }
    }

    
}
