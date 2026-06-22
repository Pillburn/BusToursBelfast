// Application/DTOs/Payment/PaymentIntentResult.cs
namespace ToursApp.Application.DTOs.Payment
{
    /// <summary>
    /// Result of a payment intent operation
    /// </summary>
    public class PaymentIntentResult
    {
        //Required properties 
        public string PaymentIntentId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Additional properties
        public bool Success { get; set; } = true;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "GBP";
        public Dictionary<string, string>? Metadata { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }

        // ✅ Constructors
        public PaymentIntentResult() { }

        public PaymentIntentResult(string paymentIntentId, string clientSecret, string status)
        {
            PaymentIntentId = paymentIntentId;
            ClientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
            Status = status;
            Success = true;
        }

        public PaymentIntentResult(string errorMessage, string errorCode)
        {
            Success = false;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            Status = "failed";
        }

        // ✅ Factory methods for common scenarios
        public static PaymentIntentResult Succeed(string paymentIntentId, string clientSecret, string status)
        {
            return new PaymentIntentResult(paymentIntentId, clientSecret, status);
        }

        public static PaymentIntentResult Failure(string errorMessage, string? errorCode = null)
        {
            return new PaymentIntentResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                Status = "failed"
            };
        }
    }

    /// <summary>
    /// Result of a refund operation
    /// </summary>
    public class RefundResult
    {
        public bool Success { get; set; }
        public string RefundId { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }

        public static RefundResult Succeed(string refundId, string paymentIntentId, decimal amount, string status)
        {
            return new RefundResult
            {
                Success = true,
                RefundId = refundId,
                PaymentIntentId = paymentIntentId,
                Amount = amount,
                Status = status
            };
        }

        public static RefundResult Failure(string errorMessage)
        {
            return new RefundResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }

    /// <summary>
    /// Result of a webhook processing operation
    /// </summary>
    public class WebhookResult
    {
        public bool Success { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? BookingId { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? ErrorMessage { get; set; }

        public static WebhookResult Succeed(string eventType, string paymentIntentId, string status)
        {
            return new WebhookResult
            {
                Success = true,
                EventType = eventType,
                PaymentIntentId = paymentIntentId,
                Status = status
            };
        }

        public static WebhookResult Failure(string errorMessage)
        {
            return new WebhookResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}