using Hubtel.PaymentProxyData.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxyData.EntityModels
{
    public class PaymentRequest : BaseEntity
    {
        public string PaymentType { get; set; }

        public Guid? OrderId { get; set; }

        public string MomoPhoneNumber { get; set; }

        public string MomoChannel { get; set; }

        public string MomoToken { get; set; }

        public string TransactionSession { get; set; }
        public string TransactionId { get; set; } // from merchant account
        public string ExternalTransactionId { get; set; }// from telco to merchant account
        public decimal AmountAfterCharges { get; set; }
        public decimal Charges { get; set; }

        public bool? ChargeCustomer { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime? PaymentDate { get; set; }

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

        [StringLength(50)]
        public string OfflineGuid { get; set; }

        [MaxLength(255)]
        public string ClientReference { get; set; }
        
        public string Status { get; set; }

        public void SetStatus(string status)
        {
            if (status.ToLower().Equals("success"))
            {
                Status = En.PaymentStatus.SUCCESSFUL;
                IsSuccessful = true;
            }
            else if (status.ToLower().Equals("failed"))
            {
                Status = En.PaymentStatus.FAILED;
                IsSuccessful = false;
            }
        }
    }
}
