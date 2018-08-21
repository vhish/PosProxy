using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Responses
{
    public class OrderResponseWrapper
    {
        public OrderResponse Data { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
    }

    public class OrderResponse
    {
        public Guid Id { get; set; }
        public string IntegrationChannel { get; set; }
        public string PosDeviceId { get; set; }
        public string PosDeviceType { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public string AssignedTo { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string CustomerMobileNumber { get; set; }
        public string CustomerName { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? DiscountRate { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal TotalAmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public List<PaymentResponse> Payments { get; set; }
        public string OfflineGuid { get; set; }
    }

    public class OrderItem
    {
        public string ItemId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? DiscountRate { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string Note { get; set; }
        public bool IsReturned { get; set; }
    }

}
