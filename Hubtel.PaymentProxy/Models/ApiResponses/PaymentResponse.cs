using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Responses
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public string PaymentType { get; set; }
        public Guid? OrderId { get; set; }
        public string MomoPhoneNumber { get; set; }
        public string MomoChannel { get; set; }
        public string MomoToken { get; set; }
        public string TransactionId { get; set; } // from merchant account
        public string ExternalTransactionId { get; set; }// from telco to merchant account
        public decimal? AmountAfterCharges { get; set; }
        public decimal? Charges { get; set; }
        public bool ChargeCustomer { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public string PosDeviceId { get; set; }
        public string PosDeviceType { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string CustomerMobileNumber { get; set; }
        public string CustomerName { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public bool IsRefund { get; set; }
        public bool IsSuccessful { get; set; }
        public string ReceiptNumber { get; set; }
        public string OfflineGuid { get; set; }
        public virtual OrderResponse Order { get; set; }
    }
}
