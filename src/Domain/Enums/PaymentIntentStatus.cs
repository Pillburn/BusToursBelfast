// Domain/Enums/PaymentIntentStatus.cs
public enum PaymentIntentStatus
{
    RequiresPaymentMethod, // Initial state
    RequiresConfirmation,
    RequiresAction,       // 3D Secure or other auth needed
    Processing,           // Asynchronous processing
    Succeeded,            // Payment completed
    Canceled,             // Customer canceled
    RequiresCapture       // For delayed capture flows
}