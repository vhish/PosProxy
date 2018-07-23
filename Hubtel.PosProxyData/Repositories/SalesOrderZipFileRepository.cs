using Hubtel.PosProxyData.Constants;
using Hubtel.PosProxyData.Core;
using Hubtel.PosProxyData.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxyData.Repositories
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
