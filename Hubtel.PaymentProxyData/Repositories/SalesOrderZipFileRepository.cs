using Hubtel.PaymentProxyData.Constants;
using Hubtel.PaymentProxyData.Core;
using Hubtel.PaymentProxyData.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxyData.Repositories
{
    public class SalesOrderZipFileRepository : BaseRepository<SalesOrderZipFile>, ISalesOrderZipFileRepository
    {
        public SalesOrderZipFileRepository(ApplicationDbContext context) : base(context)
        {
        }
        
    }

    public interface ISalesOrderZipFileRepository : IBaseRepository<SalesOrderZipFile>
    {
        
    }
}
