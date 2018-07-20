using Hubtel.PosProxyData.Constants;
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

        /*public async Task SetToSuccessfulAsync(string clientReference)
        {
            var sql = $"UPDATE PaymentRequests SET Status = '{En.PaymentStatus.SUCCESSFUL}' WHERE " +
                $"ClientReference = '{clientReference}'";
            await ExecuteSqlCommandAsync(sql);
        }

        public async Task SetToFailedAsync(string clientReference)
        {
            var sql = $"UPDATE PaymentRequests SET Status = '{En.PaymentStatus.FAILED}' WHERE " +
                $"ClientReference = '{clientReference}'";
            await ExecuteSqlCommandAsync(sql);
        }*/
    }

    public interface IPaymentRequestRepository : IBaseRepository<PaymentRequest>
    {
        //Task SetToSuccessfulAsync(string clientReference);
        //Task SetToFailedAsync(string clientReference);
    }
}
