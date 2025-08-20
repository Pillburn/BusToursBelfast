// Infrastructure/Services/StripePaymentService.cs
using Microsoft.Extensions.Options;
using Stripe;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Interfaces;
using ToursApp.Infrastructure.Services;
using PaymentIntent = ToursApp.Domain.Entities.PaymentIntent;

public class StripePaymentService : IStripePaymentService
{
    private readonly StripeSettings _stripeSettings;

    public StripePaymentService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    public async Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency, string createdBy)
    {
        var options = new Stripe.PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = currency.ToLower(),
            PaymentMethodTypes = new List<string> { "card" }
        };

        var service = new Stripe.PaymentIntentService();
        var stripeIntent = await service.CreateAsync(options);

        return new PaymentIntent(
            stripePaymentIntentId: stripeIntent.Id,
            amount: amount,
            currency: currency,
            clientSecret:stripeIntent.ClientSecret,
            createdBy:createdBy,
            status: MapStatus(stripeIntent.Status))
        {
        };
    }

    public Task HandleWebhookEventAsync(string json, string stripeSignature)
    {
        throw new NotImplementedException();
    }

    public Task<Payment> ProcessPaymentAsync(string paymentIntentId)
    {
        throw new NotImplementedException();
    }

    private PaymentIntentStatus MapStatus(string stripeStatus) => stripeStatus switch
    {
        "requires_payment_method" => PaymentIntentStatus.RequiresPaymentMethod,
        "requires_confirmation" => PaymentIntentStatus.RequiresConfirmation,
        "requires_action" => PaymentIntentStatus.RequiresAction,
        "processing" => PaymentIntentStatus.Processing,
        "succeeded" => PaymentIntentStatus.Succeeded,
        "canceled" => PaymentIntentStatus.Canceled,
        _ => PaymentIntentStatus.RequiresPaymentMethod
    };

    // Other implementations...
}