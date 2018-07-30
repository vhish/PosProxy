using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.BackgroundServices
{
    internal class SetupHostedService : IHostedService
    {
        private readonly ILogger _logger;

        public SetupHostedService(IServiceProvider services,
            ILogger<SetupHostedService> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is starting.");

            DoWork(cancellationToken);

            return Task.CompletedTask;
        }

        private void DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is working.");

            using (var scope = Services.CreateScope())
            {
                var paymentStatusCheckService =
                    scope.ServiceProvider
                        .GetRequiredService<IPaymentStatusCheckService>();

                paymentStatusCheckService.DoWorkAsync(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
