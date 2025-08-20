// Application/Common/Interfaces/IPaymentWebhookService.cs
namespace ToursApp.Application.Common.Interfaces;

public interface IPaymentWebhookService
{
    Task<bool> ProcessWebhookAsync(string json, string stripeSignature);
}