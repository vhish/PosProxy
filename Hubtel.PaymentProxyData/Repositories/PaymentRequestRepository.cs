using Hubtel.PaymentProxyData.Constants;
using Hubtel.PaymentProxyData.Core;
using Hubtel.PaymentProxyData.EntityModels;
using Hubtel.PaymentProxyData.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxyData.Repositories
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

        public async Task UpdateStatusAsync(PaymentRequest paymentRequest)
        {
            var sql = $"UPDATE PaymentRequests SET Status='{paymentRequest.Status}', IsSuccessful={(paymentRequest.IsSuccessful ? 1:0)} " +
                $"WHERE ClientReference = '{paymentRequest.ClientReference}'";
            await ExecuteSqlCommandAsync(sql).ConfigureAwait(false);
        }
    }

    public interface IPaymentRequestRepository : IBaseRepository<PaymentRequest>
    {
        Task DeleteByClientReferenceAsync(string clientReference);
        Task UpdateStatusAsync(PaymentRequest paymentRequest);
    }
}
