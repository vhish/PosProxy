using Hubtel.PosProxy.Constants;
using Hubtel.PosProxy.Models.Responses;
using Hubtel.PosProxyData.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Requests
{
    public class OrderPaymentRequest
    {
        public string PaymentType { get; set; }

        public Guid? OrderId { get; set; }

        public string MomoPhoneNumber { get; set; }

        public string MomoChannel { get; set; }

        public string MomoToken { get; set; }

        public string TransactionId { get; set; } // from merchant account
        public string ExternalTransactionId { get; set; }// from telco to merchant account
        public decimal AmountAfterCharges { get; set; }
        public decimal Charges { get; set; }

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

        [StringLength(50)]
        public string OfflineGuid { get; set; }

        public static OrderPaymentRequest ToOrderPaymentRequest(PaymentRequest paymentRequest)
        {
            return new OrderPaymentRequest
            {
                SalesOrderId = paymentRequest.OrderId.ToString(),
                PaymentAmount = paymentRequest.AmountPaid,
                PaymentDate = paymentRequest.PaymentDate ?? DateTime.Now,
                MerchantBearsFee = !paymentRequest.ChargeCustomer ?? false,
                Note = paymentRequest.Note,
                PaymentFee = paymentRequest.Charges,
                PaymentStatus = paymentRequest.Status,
                PaymentType = paymentRequest.PaymentType,
                PaymentReference = paymentRequest.ClientReference,
                PosDevice = paymentRequest.PosDeviceId,
                Branch = new Branch
                {
                    BranchId = paymentRequest.BranchId,
                    Name = paymentRequest.BranchName
                },
                Employee = new Employee
                {
                    EmployeeId = paymentRequest.EmployeeId,
                    Name = paymentRequest.EmployeeName
                }
            };
        }
    }

    public class Branch
    {
        public string BranchId { get; set; }
        public string Name { get; set; }
    }

    public class Employee
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }
    
}
