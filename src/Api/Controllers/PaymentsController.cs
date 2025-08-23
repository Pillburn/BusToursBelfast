// API/Controllers/PaymentsController.cs
using MediatR;  // MediatR library for CQRS pattern
using Microsoft.AspNetCore.Authorization;  // ASP.NET Core authorization
using Microsoft.AspNetCore.Mvc;  // ASP.NET Core MVC framework
using ToursApp.Application.Payments.Commands; // Your application command

[ApiController]  // Marks this class as an API controller (automatic model validation, etc.)
[Route("api/[controller]")]  // Sets base route to /api/Payments (derived from class name)
[Authorize]  // Requires authentication for all endpoints in this controller
public class PaymentsController : ControllerBase  // Inherits from ControllerBase (API-specific base class)
{
    private readonly IMediator _mediator;  // Private field to store MediatR instance

    // Constructor dependency injection
    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;  // Assign injected mediator to private field
    }

    // HTTP POST endpoint for creating payment intents
    [HttpPost("intent")]  // Maps to POST /api/payments/intent
    public async Task<ActionResult<PaymentIntentResponse>> CreatePaymentIntent(  // Async method returning ActionResult
        [FromBody] CreatePaymentIntentRequest request)  // Binds JSON request body to DTO
    {
        // Create command object from request DTO
        var command = new CreatePaymentIntentCommand(
            request.Amount,      // Extract amount from request
            request.Currency,    // Extract currency from request  
            request.BookingId);  // Extract optional booking ID

        // Send command to MediatR for processing (returns PaymentIntentResult)
        var result = await _mediator.Send(command);
        
        // Return 200 OK with response DTO
        return Ok(new PaymentIntentResponse(
            result.PaymentIntentId,   // Map domain result to API response
            result.ClientSecret,      // Client secret for Stripe.js
            result.Status));          // Payment status
    }
}