using Hubtel.PaymentProxy.IntegrationTest.Data;
using Hubtel.PaymentProxyData.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.PaymentProxy.IntegrationTest
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("testing");

            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkSqlServer()
                    .BuildServiceProvider();

                // Add a database context (ApplicationDbContext) using an in-memory 
                // database for testing.
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer($"Server=.\\SQLEXPRESS;Database=HubtelPaymentProxyTest;Trusted_Connection=True;MultipleActiveResultSets=true");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                // Register the database seeder
                services.AddTransient<DatabaseSeeder>();

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var serviceScope = sp.CreateScope())
                {
                    var scopedServices = serviceScope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();
                    
                    // Ensure the database is created.
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    try
                    {
                        // Seed the database with test data.
                        var seeder = serviceScope.ServiceProvider.GetService<DatabaseSeeder>();
                        seeder.Seed().Wait();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"An error occurred seeding the " +
                            "database with test messages. Error: {ex.Message}");
                    }
                }
            });

            base.ConfigureWebHost(builder);
        }
    }
}
