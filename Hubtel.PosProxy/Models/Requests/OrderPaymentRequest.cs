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
        public string SalesOrderId { get; set; }
        public string MerchantAccountTxId { get; set; }
        public string PaymentType { get; set; }
        public string PaymentReference { get; set; }
        public string Note { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal PaymentFee { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        public bool MerchantBearsFee { get; set; }
        public Branch Branch { get; set; }
        public string PosDevice { get; set; }
        public Employee Employee { get; set; }

        public static OrderPaymentRequest ToOrderPaymentRequest(PaymentRequest paymentRequest)
        {
            return new OrderPaymentRequest
            {
                SalesOrderId = paymentRequest.SalesOrderId,
                PaymentAmount = paymentRequest.Amount,
                PaymentDate = paymentRequest.PaymentDate,
                MerchantBearsFee = !paymentRequest.CustomerPaysFee,
                Note = paymentRequest.Note,
                PaymentFee = paymentRequest.Charges,
                PaymentStatus = paymentRequest.Status,
                PaymentType = paymentRequest.PaymentType,
                PaymentReference = paymentRequest.ClientReference,
                PosDevice = paymentRequest.PosDevice,
                Branch = new Branch
                {
                    BranchId = paymentRequest.BranchId,
                    Name = paymentRequest.BranchName
                },
                Employee = new Employee
                {
                    EmployeeId = paymentRequest.EmployeeId,
                    PhoneNumber = paymentRequest.EmployeePhone,
                    Name = paymentRequest.EmployeeName,
                    EmailAddress = paymentRequest.EmployeeEmail
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
