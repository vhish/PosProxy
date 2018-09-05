using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.PaymentProxy.IntegrationTest.Data
{
    public class PaymentsApiResponse<T>
    {
        public T Data { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
    }
}
