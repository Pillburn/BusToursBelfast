// Application/Common/Interfaces/IPaymentWebhookService.cs
public interface IPaymentWebhookService
{
    Task ProcessWebhookEventAsync(string json, string stripeSignature);
}