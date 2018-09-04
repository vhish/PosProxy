using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.PaymentProxyData.EntityModels
{
    public class SalesOrderZipFile : BaseEntity
    {
        public string Bucketname { get; set; }
        public string Filename { get; set; }
        public string MimeType { get; set; }
        public bool Processed { get; set; }
    }
}
