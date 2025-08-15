public class CreatePaymentIntentDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "gbp";
}