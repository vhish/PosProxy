using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Dtos
{
    public class OrderItemDto
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
