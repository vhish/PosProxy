using Hubtel.PosProxyData.Core;
using Hubtel.PosProxyData.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxyData.Repositories
{
    public class PaymentRequestRepository : BaseRepository<PaymentRequest>, IPaymentRequestRepository
    {
        public PaymentRequestRepository(ApplicationDbContext context) : base(context)
        {
        }
    }

    public interface IPaymentRequestRepository : IBaseRepository<PaymentRequest>
    {
    }
}
