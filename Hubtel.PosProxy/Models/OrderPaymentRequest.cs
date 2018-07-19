using Hubtel.PosProxyData.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models
{
    public class OrderPaymentRequest
    {
        public string MerchantAccountTxId { get; set; }
        public string PaymentType { get; set; }
        public string PaymentReference { get; set; }
        public string Note { get; set; }
        public int PaymentAmount { get; set; }
        public int PaymentFee { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        public bool MerchantBearsFee { get; set; }
        public Branch Branch { get; set; }
        public string PosDevice { get; set; }
        public Employee Employee { get; set; }

        public OrderPaymentRequest ToOrderPaymentRequest(PaymentRequest paymentRequest)
        {
            return new OrderPaymentRequest();
        }

        public OrderPaymentRequest ToOrderPaymentRequest(MomoCallbackResponse response)
        {
            return new OrderPaymentRequest();
        }

        public OrderPaymentRequest ToOrderPaymentRequest(CardCallbackResponse response)
        {
            return new OrderPaymentRequest();
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
