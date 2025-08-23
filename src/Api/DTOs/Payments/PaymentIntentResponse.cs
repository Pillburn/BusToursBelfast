// API/DTOs/PaymentIntentResponse.cs  
public record PaymentIntentResponse(
    string PaymentIntentId,   // Your domain's PaymentIntent ID (as string)
    string ClientSecret,      // Stripe client secret for frontend confirmation
    string Status)            // Payment status (e.g., "RequiresPaymentMethod")
{
    // This will serialize to JSON as:
    // {
    //   "paymentIntentId": "string",
    //   "clientSecret": "string", 
    //   "status": "string"
    // }
};