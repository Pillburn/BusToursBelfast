public interface IStripeService
{
    Task<string> CreatePaymentIntentAsync(decimal amount, string currency);
}