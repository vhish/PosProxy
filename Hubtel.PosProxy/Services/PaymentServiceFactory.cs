using Hubtel.PosProxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Services
{
    public class PaymentServiceFactory : IPaymentServiceFactory
    {
        private readonly ICashPaymentService _cashPaymentService;
        public PaymentServiceFactory(ICashPaymentService cashPaymentService)
        {
            //_cashPaymentService = cashPaymentService;
        }

        public PaymentService GetPaymentService(string paymentMethodType)
        {
            switch (paymentMethodType.ToLower())
            {
                case "cash":
                    return new CashPaymentService();
                case "card":
                    return new CardPaymentService();
                case "momo":
                    return new MomoPaymentService();
                default:
                    return null;
            }
        }
    }

    public interface IPaymentServiceFactory
    {
        PaymentService GetPaymentService(string paymentMethodType);
    }
}
