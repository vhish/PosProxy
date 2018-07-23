using Hubtel.PosProxyData.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.BackgroundServices
{
    internal interface IScopedProcessingService
    {
        void DoWork();
    }

    internal class ScopedProcessingService : IScopedProcessingService
    {
        private readonly ILogger _logger;
        private readonly IPaymentRequestRepository _paymentRequestRepository;

        public ScopedProcessingService(ILogger<ScopedProcessingService> logger, IPaymentRequestRepository paymentRequestRepository)
        {
            _logger = logger;
            _paymentRequestRepository = paymentRequestRepository;
        }

        public void DoWork()
        {
            while (true)
            {
                _logger.LogInformation("Scoped Processing Service is working.");
                Thread.Sleep(5000);
            }
            
        }
    }
}
