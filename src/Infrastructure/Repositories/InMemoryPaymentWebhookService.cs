// Infrastructure/Services/InMemoryPaymentWebhookService.cs
using ToursApp.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ToursApp.Infrastructure.Services
{
    public class InMemoryPaymentWebhookService : IPaymentWebhookService
    {
        private readonly ILogger<InMemoryPaymentWebhookService> _logger;

        public InMemoryPaymentWebhookService(ILogger<InMemoryPaymentWebhookService> logger)
        {
            _logger = logger;
        }

        public Task<bool> ProcessWebhookAsync(string jsonPayload, string signature)
        {
            _logger.LogInformation("Processing webhook (in-memory): {JsonPayload}", jsonPayload);
            return Task.FromResult(true);
        }
    }
}