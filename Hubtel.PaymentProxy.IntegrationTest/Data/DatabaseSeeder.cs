using Hubtel.PaymentProxyData.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.IntegrationTest.Data
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public DatabaseSeeder(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task Seed()
        {
            // Add all the predefined articles
            //_applicationDbContext.PaymentRequests.Add();
            //_applicationDbContext.SaveChanges();
            
        }
    }
}
