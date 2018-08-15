using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Dtos
{
    public class CreateTransactionRequestDto
    {
        public OrderRequestDto Order;
        public PaymentRequestDto Payment;
    }
}
