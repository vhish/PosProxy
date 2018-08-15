using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Dtos
{
    public class OrderRequestDto
    {
        public string IntegrationChannel { get; set; }

        public string PosDeviceId { get; set; }

        public string PosDeviceType { get; set; }

        public DateTime? OrderDate { get; set; }

        public string OrderNumber { get; set; }

        public string Status { get; set; }

        public string AssignedTo { get; set; }

        public string EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string CustomerMobileNumber { get; set; }

        public string CustomerName { get; set; }

        public string BranchId { get; set; }

        public string BranchName { get; set; }

        public float? TaxRate { get; set; }

        public decimal? TaxAmount { get; set; }

        public float? DiscountRate { get; set; }

        public decimal? DiscountAmount { get; set; }

        //public decimal Subtotal { get; set; }

        public decimal TotalAmountDue { get; set; }

        public decimal AmountPaid { get; set; }

        public bool IsReturn { get; set; }

        [StringLength(40)]
        public string OfflineGuid { get; set; }

        public virtual ICollection<PaymentRequestDto> Payments { get; set; }
        public virtual ICollection<OrderItemDto> OrderItems { get; set; }
    }
}
