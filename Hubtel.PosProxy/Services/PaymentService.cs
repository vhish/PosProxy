using Hubtel.PosProxy.Models;
using Hubtel.PosProxyData.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Services
{
    public abstract class PaymentService : IPaymentService
    {
        public abstract bool CheckStatus();
        public abstract bool ProcessPayment(PaymentRequest paymentRequest);
        
    }

    public interface IPaymentService
    {
        bool ProcessPayment(PaymentRequest paymentRequest);
        bool CheckStatus();
    }
}
