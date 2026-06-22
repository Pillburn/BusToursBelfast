// Infrastructure/Services/StripeSettings.cs
namespace ToursApp.Infrastructure.Services;

public class StripeSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}