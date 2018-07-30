using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Requests
{
    public class OrderRequest
    {
        public string InvoiceIdentifier { get; set; }
        public string IntegrationChannel { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string OrderIdentifier { get; set; }
        public string Guid { get; set; }
        public string AssignedTo { get; set; }
        public string Status { get; set; }
        public List<OrderItem> Items { get; set; }
        public List<OrderDiscount> Discounts { get; set; }
        public List<OrderTax> Taxes { get; set; }
        public decimal GrossAmount { get; set; }
        public string Note { get; set; }
        public OrderCustomer Customer { get; set; }
        public OrderEmployee Employee { get; set; }
        public string PosDevice { get; set; }
        public OrderBranch Branch { get; set; }
        public dynamic CustomData { get; set; }
    }

    public class OrderDiscount
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class OrderTax
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public bool IsIncludedInPrice { get; set; }
    }



    public class OrderItem
    {
        public string ItemIdentifier { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public List<OrderDiscount> Discounts { get; set; }
        public List<OrderTax> Taxes { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string PosDevice { get; set; }
        public string Type { get; set; }
        public dynamic Metadata { get; set; }
    }

    public class OrderCustomer
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class OrderBranch
    {
        public string BranchId { get; set; }
        public string Name { get; set; }
    }

    public class OrderEmployee
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }
}
