using Hubtel.PaymentProxy.Models;
using Hubtel.PaymentProxy.Services;
using Hubtel.PaymentProxyData.Constants;
using Hubtel.PaymentProxyData.Core;
using Hubtel.PaymentProxyData.EntityModels;
using Hubtel.PaymentProxyData.Models;
using Hubtel.PaymentProxyData.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.BackgroundServices
{
    internal class PaymentStatusCheckService : IPaymentStatusCheckService
    {
        private readonly ILogger _logger;
        private readonly IPaymentTypeConfiguration _paymentTypeConfiguration;
        private readonly IServiceProvider _provider;
        private readonly IConfiguration _configuration;
        private readonly IMomoPaymentService _momoPaymentService;
        private readonly ICardPaymentService _cardPaymentService;
        private readonly IHubtelMePaymentService _hubtelMePaymentService;

        public PaymentStatusCheckService(ILogger<PaymentStatusCheckService> logger, 
            IPaymentTypeConfiguration paymentTypeConfiguration, IServiceProvider provider,
            IConfiguration configuration, IMomoPaymentService momoPaymentService,
            ICardPaymentService cardPaymentService, IHubtelMePaymentService hubtelMePaymentService)
        {
            _logger = logger;
            _paymentTypeConfiguration = paymentTypeConfiguration;
            _provider = provider;
            _configuration = configuration;
            _momoPaymentService = momoPaymentService;
            _cardPaymentService = cardPaymentService;
            _hubtelMePaymentService = hubtelMePaymentService;
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            using (var db = new CheckStatusDbContext(_configuration))
            {
                do
                {
                    try
                    {
                        var paymentTypes = _paymentTypeConfiguration.PaymentTypes.Where(x => x.CheckStatus.HasValue
                            && x.CheckStatus.Value == true).Select(x => x.Type).ToArray();

                        var paymentRequests = FetchPendingPaymentRequests(db, paymentTypes);
                        if (paymentRequests != null && paymentRequests.Any())
                        {
                            var processPaymentTasks = new List<Task>();
                            foreach (var paymentRequest in paymentRequests)
                            {
                                processPaymentTasks.Add(CheckStatusAsync(paymentRequest));
                                //CheckStatusAsync(paymentRequest).Wait();
                            }
                            Task.WaitAll(processPaymentTasks.ToArray());
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }

                    _logger.LogInformation("Payment Status Check Service is working.");
                    Thread.Sleep(5000);
                } while (!stoppingToken.IsCancellationRequested);
            }
        }

        private async Task CheckStatusAsync(PaymentRequest paymentRequest)
        {
            if (!paymentRequest.Status.Equals(En.PaymentStatus.PENDING))
                return;

            using (var db = new CheckStatusDbContext(_configuration))
            {
                if (paymentRequest.PaymentType.ToLower().Contains("momo"))
                {
                    var checkStatusResponse = await _momoPaymentService.CheckStatusAsync(paymentRequest).ConfigureAwait(false);
                    db.PaymentRequests.Update(checkStatusResponse.Data);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
                if (paymentRequest.PaymentType.ToLower().Contains("card"))
                {
                    var checkStatusResponse = await _cardPaymentService.CheckStatusAsync(paymentRequest).ConfigureAwait(false);
                    db.PaymentRequests.Update(checkStatusResponse.Data);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
                if (paymentRequest.PaymentType.ToLower().Contains("hubtelme"))
                {
                    var checkStatusResponse = await _hubtelMePaymentService.CheckStatusAsync(paymentRequest).ConfigureAwait(false);
                    db.PaymentRequests.Update(checkStatusResponse.Data);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            
        }

        private List<PaymentRequest> FetchPendingPaymentRequests(CheckStatusDbContext db, string[] paymentTypes)
        {
            //payment type must be MOMO or CARD
            //date created > 5 minutes ago
            //date created < 24 hours
            //fetch a batch of 50, keep last record id for offset increment
            //when offset exceeds max id, reset to 0

            var paymentRequests = GetPaymentRequests(db, paymentTypes, PaymentRequestOffset.Offset);

            if (!paymentRequests.Any() && PaymentRequestOffset.Offset > 0)
            {
                PaymentRequestOffset.Offset = 0;
                paymentRequests = GetPaymentRequests(db, paymentTypes, PaymentRequestOffset.Offset);
            }
            else
            {
                var lastTransaction = paymentRequests.LastOrDefault();
                if (lastTransaction != null)
                    PaymentRequestOffset.Offset = lastTransaction.Id;
                else
                    PaymentRequestOffset.Offset = 0;
            }

            return paymentRequests;
        }

        private List<PaymentRequest> GetPaymentRequests(CheckStatusDbContext db, string[] paymentTypes, int paymentRequestOffset)
        {
            return db.PaymentRequests.Where(x => paymentTypes.Contains(x.PaymentType) && x.CreatedAt < DateTime.Now.AddMinutes(-5) &&
                x.CreatedAt > DateTime.Now.AddHours(-24) && x.Id > paymentRequestOffset && x.Status == "pending")
                .OrderBy(x => x.Id).Take(50).ToList();
        }
    }

    internal interface IPaymentStatusCheckService
    {
        Task DoWorkAsync(CancellationToken stoppingToken);
    }
}
