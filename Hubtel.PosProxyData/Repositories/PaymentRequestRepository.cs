using Hubtel.PosProxyData.Constants;
using Hubtel.PosProxyData.Core;
using Hubtel.PosProxyData.EntityModels;
using Hubtel.PosProxyData.Models;
using Microsoft.Extensions.Caching.Memory;
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
        
        public async Task DeleteByClientReferenceAsync(string clientReference)
        {
            var sql = $"DELETE from PaymentRequests WHERE ClientReference = '{clientReference}'";
            await ExecuteSqlCommandAsync(sql).ConfigureAwait(false);
        }
    }

    public interface IPaymentRequestRepository : IBaseRepository<PaymentRequest>
    {
        Task DeleteByClientReferenceAsync(string clientReference);
    }
}
