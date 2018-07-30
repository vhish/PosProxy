using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Dtos
{
    public class CreateOrderRequestDto
    {
        public string InvoiceIdentifier { get; set; }
        public string IntegrationChannel { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string OrderIdentifier { get; set; }
        public string Guid { get; set; }
        public string AssignedTo { get; set; }
        public string Status { get; set; }
        public List<ItemDto> Items { get; set; }
        public List<DiscountDto> Discounts { get; set; }
        public List<TaxDto> Taxes { get; set; }
        public decimal GrossAmount { get; set; }
        public string Note { get; set; }
        public CustomerDto Customer { get; set; }
        public EmployeeDto Employee { get; set; }
        public string PosDevice { get; set; }
        public BranchDto Branch { get; set; }
        public dynamic CustomData { get; set; }
    }

    public class DiscountDto
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class TaxDto
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public bool IsIncludedInPrice { get; set; }
    }



    public class ItemDto
    {
        public string ItemIdentifier { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public List<DiscountDto> Discounts { get; set; }
        public List<TaxDto> Taxes { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string PosDevice { get; set; }
        public string Type { get; set; }
        public dynamic Metadata { get; set; }
    }
}
