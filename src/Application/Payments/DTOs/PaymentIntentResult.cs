// Application/Features/Payments/PaymentIntentResult.cs
public class PaymentIntentResult
{
    public string PaymentIntentId { get; set; }
    public string ClientSecret { get; set; }
    public string Status { get; set; }

    public PaymentIntentResult(string paymentIntentId, string clientSecret, string status)
    {
        PaymentIntentId = paymentIntentId;
        ClientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
        Status = status;
    }
}