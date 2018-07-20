using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.PosProxy.Models;
using Hubtel.PosProxyData.EntityModels;

namespace Hubtel.PosProxy.Services
{
    public class CardPaymentService : PaymentService, ICardPaymentService
    {
        public override bool CheckStatus()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> RecordPaymentAsync(PaymentRequest paymentRequest)
        {
            throw new NotImplementedException();
        }
    }

    public interface ICardPaymentService : IPaymentService
    {

    }
}
