// Application/Features/Payments/PaymentIntentResult.cs
public class PaymentIntentResult
{
    public Guid PaymentIntentId { get; set; }
    public string ClientSecret { get; set; }
    public string Status { get; set; }

    public PaymentIntentResult(Guid paymentIntentId, string clientSecret, string status)
    {
        PaymentIntentId = paymentIntentId;
        ClientSecret = clientSecret;
        Status = status;
    }
}