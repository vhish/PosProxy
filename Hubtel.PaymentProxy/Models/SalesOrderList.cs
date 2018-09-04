using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models
{
    public class SalesOrderList
    {
        public List<SalesOrder> Data { get; set; }
    }

    public class SalesOrder
    {
        public string Id { get; set; }
    }
}
