using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Dtos
{
    public class CardCallbackRequestDto
    {
        public string ResponseCode { get; set; }
        public CardCallbackData Data { get; set; }
    }
    public class CardCallbackData
    {
        public string TransactionId { get; set; }
        public string ClientReference { get; set; }
        public string Description { get; set; }
        public string TransactionSession { get; set; }
        public CardMetaData Meta { get; set; }
    }
    public class CardMetaData
    {
        public string identifier { get; set; }
        public string type { get; set; }
        public CardMetaDataTransaction transaction { get; set; }
    }
    public class CardMetaDataTransaction
    {
        public CardMetaDataPaymentDetails PaymentDetails { get; set; }
        public CardMetaDataReceiptDetails ReceiptDetails { get; set; }
        public CardMetaDataClearingDetails ClearingDetails { get; set; }
    }
    public class CardMetaDataPaymentDetails
    {
        public string Scheme { get; set; }
        public string Source { get; set; }
        public string MaskedAccount { get; set; }
    }
    public class CardMetaDataReceiptDetails
    {
        public string AuthorizationCode { get; set; }
        public CardMetaDataReceiptDetailsEmv Emv { get; set; }
    }
    public class CardMetaDataReceiptDetailsEmv
    {
        public string ApplicationID { get; set; }
        public string ApplicationLabel { get; set; }
    }
    public class CardMetaDataClearingDetails
    {
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
        public string AuthorizationCode { get; set; }
        public string TransactionIdentifier { get; set; }
    }
}
