using Hubtel.PosProxyData.EntityModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.PosProxyData.Core
{
    public class ApplicationDbContext : DbContext
    {
        protected IHttpContextAccessor HttpContextAccessor;
        public DbSet<PaymentRequest> PaymentRequests { get; set; }
        public DbSet<SalesOrderZipFile> SalesOrderZipFiles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<PaymentRequest>()
                .HasIndex(b => b.ClientReference)
                .IsUnique();

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal)))
            {
                property.Relational().ColumnType = "decimal(20, 4)";
            }

        }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            OnBeforeSaving();
            return await base.SaveChangesAsync();
        }

        private void OnBeforeSaving()
        {
            AddTimestampInfo();
        }

        private void AddTimestampInfo()
        {
            var user = GetUserName();
            var userId = GetUserId();
            var accountId = GetAccountId();
            var currentAccountId = !string.IsNullOrEmpty(user) ? user : "Anonymous";
            var currentUsername = !string.IsNullOrEmpty(user) ? user : "Anonymous";
            //var currentUserid = !string.IsNullOrEmpty(userId) ? userId : "Unknown";

            var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
                    ((BaseEntity)entry.Entity).CreatedBy = currentUsername.Trim();
                    if (currentAccountId != null)
                        ((BaseEntity)entry.Entity).AccountId = currentAccountId;
                }
                ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
                ((BaseEntity)entry.Entity).UpdatedBy = currentUsername.Trim();
            }
        }

        private string GetUserName()
        {
            if (HttpContextAccessor?.HttpContext != null)
            {
                var user = HttpContextAccessor.HttpContext.User;
                var claimsIdentity = (ClaimsIdentity)user.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.Name);
                return claim?.Value;
            }
            return null;
        }

        private string GetUserId()
        {
            if (HttpContextAccessor?.HttpContext != null)
            {
                var user = HttpContextAccessor.HttpContext.User;
                var claimsIdentity = (ClaimsIdentity)user.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.MobilePhone);
                return claim?.Value;
            }
            return null;
        }

        private string GetAccountId()
        {
            if (HttpContextAccessor?.HttpContext != null)
            {
                var user = HttpContextAccessor.HttpContext.User;
                var claimsIdentity = (ClaimsIdentity)user.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.Sid);
                return claim?.Value;
            }
            return null;
        }
    }
}
