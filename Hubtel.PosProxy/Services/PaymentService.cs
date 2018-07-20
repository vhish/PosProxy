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
        public abstract Task<bool> ProcessPaymentAsync(PaymentRequest paymentRequest);
        public abstract Task<bool> RecordPaymentAsync(PaymentRequest paymentRequest);
    }

    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(PaymentRequest paymentRequest);
        Task<bool> RecordPaymentAsync(PaymentRequest paymentRequest);
        bool CheckStatus();
    }
}
