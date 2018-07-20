using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxyData.EntityModels
{
    public class PaymentRequest : BaseEntity
    {
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public string SalesOrderId { get; set; }
        public string MomoPhoneNumber { get; set; }
        public string MomoChannel { get; set; }
        public string MomoToken { get; set; }
        public string Description { get; set; }

        public string TransactionSession { get; set; }
        public string TransactionId { get; set; }
        public string ExternalTransactionId { get; set; }
        public decimal AmountAfterCharges { get; set; }
        public decimal Charges { get; set; }

        [MaxLength(255)]
        public string ClientReference { get; set; }
        public bool CustomerPaysFee { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Note { get; set; }
        public string PosDevice { get; set; }

        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }

        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeePhone { get; set; }
        public string EmployeeEmail { get; set; }

        public string BranchId { get; set; }
        public string BranchName { get; set; }

        public string Status { get; set; }
    }
}
