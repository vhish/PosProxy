using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.PosProxyData.Core
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlServer(
                "Server=localhost\\SQLEXPRESS;Database=HPosProxy;Trusted_Connection=True;MultipleActiveResultSets=true;",
                sql => sql.MigrationsAssembly("Hubtel.PosProxyData"));

            return new ApplicationDbContext(builder.Options);
        }
    }
}
