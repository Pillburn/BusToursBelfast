// Infrastructure/Services/StripePaymentService.cs
using Microsoft.Extensions.Options;
using Stripe;
using ToursApp.Domain.Interfaces;

public class StripePaymentService : IStripePaymentService
{
    private readonly StripeSettings _stripeSettings;

    public StripePaymentService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    public async Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100), // Stripe uses smallest currency unit
            Currency = currency,
            PaymentMethodTypes = new List<string> { "card" }
        };

        var service = new PaymentIntentService();
        return await service.CreateAsync(options);
    }

    public Task HandleWebhookEventAsync(string json, string stripeSignature)
    {
        throw new NotImplementedException();
    }

    public Task<Payment> ProcessPaymentAsync(string paymentIntentId)
    {
        throw new NotImplementedException();
    }

    // Other implementations...
}