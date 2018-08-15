using FluentValidation.Attributes;
using Hubtel.PosProxy.Models.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Dtos
{
    public class PaymentRequestDto
    {
        public string PaymentType { get; set; }

        public Guid? OrderId { get; set; }

        public string MomoPhoneNumber { get; set; }

        public string MomoChannel { get; set; }

        public string MomoToken { get; set; }

        public string MerchantTransactionId { get; set; }

        public bool? ChargeCustomer { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string Note { get; set; }

        public string Description { get; set; }

        public int PosDeviceId { get; set; }

        public string EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string CustomerMobileNumber { get; set; }

        public string CustomerName { get; set; }

        public string BranchId { get; set; }

        public string BranchName { get; set; }

        public bool IsRefund { get; set; }

        public bool IsSuccessful { get; set; }

        public string ReceiptNumber { get; set; }

        [StringLength(40)]
        public string OfflineGuid { get; set; }
    }
}
