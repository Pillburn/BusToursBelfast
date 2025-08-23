public record CreatePaymentIntentRequest(  // Record type (immutable, value-based equality)
    decimal Amount,      // Payment amount (e.g., 100.00)
    string Currency,     // Currency code (e.g., "gbp", "usd")
    Guid? BookingId = null)  // Optional booking reference (nullable GUID)
{
    // Records automatically generate:
    // - Constructor with parameters
    // - Properties with getters
    // - Equality members
    // - ToString() method
};