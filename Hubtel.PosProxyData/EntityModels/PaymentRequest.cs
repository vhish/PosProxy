using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxyData.EntityModels
{
    public class PaymentRequest : BaseEntity
    {
        public string PaymentType { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMsisdn { get; set; }
        public string CustomerEmail { get; set; }
        public string Channel { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string ClientReference { get; set; }
        public string VodafoneToken { get; set; }
        public string SalesOrderId { get; set; }
    }
}
