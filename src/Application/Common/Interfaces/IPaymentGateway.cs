// Application/Common/Interfaces/IPaymentGateway.cs
using ToursApp.Application.DTOs.Payment;
using ToursApp.Application.Payments;

namespace ToursApp.Application.Common.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaymentIntentResult> CreatePaymentIntentAsync(
            decimal amount,
            string currency,
            Dictionary<string, string>? metadata = null,
            CancellationToken cancellationToken = default);

        Task<PaymentIntentResult> GetPaymentIntentAsync(
            string paymentIntentId,
            CancellationToken cancellationToken = default);

        Task<PaymentIntentResult> ConfirmPaymentIntentAsync(
            string paymentIntentId,
            string paymentMethodId,
            CancellationToken cancellationToken = default);

        Task<RefundResult> CreateRefundAsync(
            string paymentIntentId,
            decimal? amount = null,
            string? reason = null,
            CancellationToken cancellationToken = default);
        
        Task<WebhookResult> ProcessWebhookAsync(
            string jsonPayload,
            string signature,
            CancellationToken cancellationToken = default);
    }
}