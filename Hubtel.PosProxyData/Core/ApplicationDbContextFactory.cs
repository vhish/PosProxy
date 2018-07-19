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
                "Data Source=tcp:localhost\\SQLEXPRESS,1433;Database=HPosProxy;User Id=hubtelpos;Password=hubtelpos;MultipleActiveResultSets=true;",
                sql => sql.MigrationsAssembly("Hubtel.PosProxyData"));

            return new ApplicationDbContext(builder.Options);
        }
    }
}
