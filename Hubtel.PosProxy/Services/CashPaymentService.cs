using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxyData.EntityModels;

namespace Hubtel.PosProxy.Services
{
    public class CashPaymentService : PaymentService, ICashPaymentService
    {
        public CashPaymentService()
        {

        }

        public override bool CheckStatus()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessPayment(PaymentRequest paymentRequest)
        {
            //record the payment against the sales order on UnifiedSales API
            var orderPaymentRequest = new OrderPaymentRequest();
            orderPaymentRequest.ToOrderPaymentRequest(paymentRequest);
        }
    }

    public interface ICashPaymentService : IPaymentService
    {

    }
}
