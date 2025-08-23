// API/Controllers/StripeWebhookController.cs
using Microsoft.AspNetCore.Mvc;
using ToursApp.Application.Common.Interfaces;

[ApiController]
[Route("webhooks/stripe")]  // Base route for webhooks (separate from API)
[ApiExplorerSettings(IgnoreApi = true)]  // Excludes from Swagger/OpenAPI documentation
public class StripeWebhookController : ControllerBase
{
    private readonly IPaymentWebhookService _webhookService;  // Your webhook processing service
    private readonly ILogger<StripeWebhookController> _logger;  // Logger for diagnostics

    // Constructor injection
    public StripeWebhookController(
        IPaymentWebhookService webhookService,
        ILogger<StripeWebhookController> logger)
    {
        _webhookService = webhookService;
        _logger = logger;
    }

    [HttpPost]  // POST /webhooks/stripe (Stripe sends webhooks as POST requests)
    public async Task<IActionResult> HandleWebhook()  // IActionResult for flexible responses
    {
        // Read raw JSON from request body (Stripe sends raw JSON)
        using var reader = new StreamReader(HttpContext.Request.Body);
        var json = await reader.ReadToEndAsync();
        
        // Get Stripe signature from headers for verification
        var stripeSignature = Request.Headers["Stripe-Signature"];

        try
        {
            // Process webhook through your service
            var result = await _webhookService.ProcessWebhookAsync(json, stripeSignature);
            
            // Return 200 OK if successful, 400 Bad Request if failed
            return result ? Ok() : BadRequest();
        }
        catch (Exception ex)
        {
            // Log error but still return 200 to prevent Stripe retries
            _logger.LogError(ex, "Webhook processing failed");
            return Ok();  // Important: Always return 200 to acknowledge receipt
        }
    }
}