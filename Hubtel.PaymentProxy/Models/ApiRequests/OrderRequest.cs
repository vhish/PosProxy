using Hubtel.PaymentProxy.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.ApiRequests
{
    public class OrderRequest
    {
        public string IntegrationChannel { get; set; }

        public int? PosDeviceId { get; set; }

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

        public string OfflineGuid { get; set; }
        
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public virtual ICollection<PaymentRequestDto> Payments { get; } = new List<PaymentRequestDto>();
    }

    public class OrderItem
    {
        public string ItemId { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public float? TaxRate { get; set; }

        public decimal? TaxAmount { get; set; }

        public float? DiscountRate { get; set; }

        public decimal? DiscountAmount { get; set; }

        public string Note { get; set; }

        public bool? IsReturned { get; set; }
    }
}
