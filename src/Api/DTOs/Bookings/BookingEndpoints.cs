// API/Endpoints/BookingEndpoints.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToursApp.Application.Bookings.Commands.CreateBooking;
using ToursApp.Application.Bookings.Commands.EditBooking;
using ToursApp.Application.Bookings.Commands.DeleteBooking;
using ToursApp.Application.Bookings.Queries.GetBooking;
using ToursApp.Application.Bookings.Queries.GetBookingsByUser;
using ToursApp.Application.Bookings.Queries.GetBookingsByTour;
using ToursApp.Application.Bookings.Queries.GetBookingStatus;
using ToursApp.Application.DTOs.Booking;
using ToursApp.Domain.Exceptions;
using ToursApp.Application.Bookings.Queries.GetBookingByReference;
using ToursApp.Application.Bookings.Queries.GetAllBookings;

namespace ToursApp.API.Endpoints
{
    public static class BookingEndpoints
    {
        public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/bookings")
                .WithTags("Bookings")
                .WithOpenApi();

            // Booking CRUD Operations
            group.MapPost("/", CreateBooking)
                .WithName("CreateBooking")
                .WithSummary("Create a new booking")
                .WithDescription("Creates a new booking for a tour with the provided details")
                .Produces<BookingResponseDto>(StatusCodes.Status201Created)
                .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            group.MapGet("/{id:guid}", GetBookingById)
                .WithName("GetBookingById")
                .WithSummary("Get booking by ID")
                .WithDescription("Retrieves detailed information about a specific booking")
                .Produces<BookingDetailsDto>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            group.MapGet("/reference/{referenceNumber}", GetBookingByReference)
                .WithName("GetBookingByReference")
                .WithSummary("Get booking by reference number")
                .WithDescription("Retrieves booking details using the user-friendly reference number")
                .Produces<BookingDetailsDto>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            group.MapGet("/user/{email}", GetBookingsByUser)
                .WithName("GetBookingsByUser")
                .WithSummary("Get bookings by user email")
                .WithDescription("Retrieves all bookings for a specific user")
                .Produces<List<BookingListDto>>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            group.MapGet("/tour/{tourId:guid}", GetBookingsByTour)
                .WithName("GetBookingsByTour")
                .WithSummary("Get bookings by tour")
                .WithDescription("Retrieves all bookings for a specific tour")
                .Produces<List<BookingListDto>>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            group.MapGet("/{referenceNumber}/status", GetBookingStatus)
                .WithName("GetBookingStatus")
                .WithSummary("Get booking status by reference")
                .WithDescription("Quickly checks the status of a booking using the reference number")
                .Produces<BookingStatusDto>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            group.MapPut("/{id:guid}", EditBooking)
                .WithName("EditBooking")
                .WithSummary("Update an existing booking")
                .WithDescription("Updates the details of a specific booking (partial update supported)")
                .Produces<BookingResponseDto>(StatusCodes.Status200OK)
                .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            group.MapDelete("/{id:guid}", DeleteBooking)
                .WithName("DeleteBooking")
                .WithSummary("Delete a booking")
                .WithDescription("Deletes (soft deletes) a specific booking")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            // Admin endpoint - internal use only
            group.MapGet("/admin/all", GetAllBookings)
                .WithName("GetAllBookings")
                .WithSummary("Get all bookings (Admin)")
                .WithDescription("Retrieves all bookings in the system. Admin only.")
                .Produces<List<BookingListDto>>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization("Admin");
        }

        /// <summary>
        /// Creates a new booking
        /// </summary>
        private static async Task<IResult> CreateBooking(
            [FromBody] CreateBookingCommand command,
            IMediator mediator,
            ILogger<CreateBookingCommand> logger)
        {
            try
            {
                logger.LogInformation("Creating new booking for tour: {TourId}", command.TourId);
                
                var result = await mediator.Send(command);
                
                logger.LogInformation("Booking created successfully with ID: {BookingId}", result.BookingId);
                
                return Results.Created($"/api/bookings/{result.BookingId}", result);
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex, "Tour not found: {TourId}", command.TourId);
                return Results.NotFound(new ProblemDetails
                {
                    Title = "Resource Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (ValidationException ex)
            {
                logger.LogWarning(ex, "Validation failed for booking creation");
                
                // ✅ Correct way to handle ValidationException
                var errors = ex.Errors?
                    .ToDictionary(
                        error => error.Key,           // ← Use Key (PropertyName)
                        error => error.Value          // ← Use Value (string[])
                    ) ?? new Dictionary<string, string[]>();
                
                return Results.BadRequest(new ValidationProblemDetails
                {
                    Title = "Validation Failed",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Errors = errors
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating booking for tour: {TourId}", command.TourId);
                return Results.Problem(
                    title: "An error occurred",
                    detail: "Unable to create booking. Please try again later.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        /// <summary>
        /// Gets a booking by its ID
        /// </summary>
        private static async Task<IResult> GetBookingById(
            Guid id,
            IMediator mediator,
            ILogger<GetBookingQuery> logger)
        {
            try
            {
                logger.LogInformation("Retrieving booking with ID: {BookingId}", id);
                
                var query = new GetBookingQuery { BookingId = id };
                var result = await mediator.Send(query);
                
                return Results.Ok(result);
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex, "Booking not found: {BookingId}", id);
                return Results.NotFound(new ProblemDetails
                {
                    Title = "Booking Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving booking: {BookingId}", id);
                return Results.Problem(
                    title: "An error occurred",
                    detail: "Unable to retrieve booking. Please try again later.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        /// <summary>
        /// Gets a booking by its reference number
        /// </summary>
        private static async Task<IResult> GetBookingByReference(
            string referenceNumber,
            IMediator mediator,
            ILogger<GetBookingByReferenceQuery> logger)
        {
            try
            {
                logger.LogInformation("Retrieving booking with reference: {ReferenceNumber}", referenceNumber);
                
                var query = new GetBookingByReferenceQuery { ReferenceNumber = referenceNumber };
                var result = await mediator.Send(query);
                
                return Results.Ok(result);
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex, "Booking not found with reference: {ReferenceNumber}", referenceNumber);
                return Results.NotFound(new ProblemDetails
                {
                    Title = "Booking Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving booking by reference: {ReferenceNumber}", referenceNumber);
                return Results.Problem(
                    title: "An error occurred",
                    detail: "Unable to retrieve booking. Please try again later.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        /// <summary>
        /// Gets all bookings for a specific user
        /// </summary>
        private static async Task<IResult> GetBookingsByUser(
            string email,
            IMediator mediator,
            ILogger<GetBookingsByUserQuery> logger)
        {
            try
            {
                logger.LogInformation("Retrieving bookings for user: {Email}", email);
                
                var query = new GetBookingsByUserQuery { Email = email };
                var result = await mediator.Send(query);
                
                if (result == null || !result.Any())
                {
                    return Results.Ok(new List<BookingListDto>());
                }
                
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving bookings for user: {Email}", email);
                return Results.Problem(
                    title: "An error occurred",
                    detail: "Unable to retrieve bookings. Please try again later.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        /// <summary>
        /// Gets all bookings for a specific tour
        /// </summary>
        private static async Task<IResult> GetBookingsByTour(
            Guid tourId,
            IMediator mediator,
            ILogger<GetBookingsByTourQuery> logger)
        {
            try
            {
                logger.LogInformation("Retrieving bookings for tour: {TourId}", tourId);
                
                var query = new GetBookingsByTourQuery { TourId = tourId };
                var result = await mediator.Send(query);
                
                if (result == null || !result.Any())
                {
                    return Results.Ok(new List<BookingListDto>());
                }
                
                return Results.Ok(result);
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex, "Tour not found: {TourId}", tourId);
                return Results.NotFound(new ProblemDetails
                {
                    Title = "Tour Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving bookings for tour: {TourId}", tourId);
                return Results.Problem(
                    title: "An error occurred",
                    detail: "Unable to retrieve bookings. Please try again later.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        /// <summary>
        /// Gets the status of a booking by reference number
        /// </summary>
        private static async Task<IResult> GetBookingStatus(
            string referenceNumber,
            IMediator mediator,
            ILogger<GetBookingStatusQuery> logger)
        {
            try
            {
                logger.LogInformation("Checking status for booking: {ReferenceNumber}", referenceNumber);
                
                var query = new GetBookingStatusQuery { ReferenceNumber = referenceNumber };
                var result = await mediator.Send(query);
                
                return Results.Ok(result);
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex, "Booking not found with reference: {ReferenceNumber}", referenceNumber);
                return Results.NotFound(new ProblemDetails
                {
                    Title = "Booking Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking booking status for: {ReferenceNumber}", referenceNumber);
                return Results.Problem(
                    title: "An error occurred",
                    detail: "Unable to check booking status. Please try again later.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        /// <summary>
        /// Updates an existing booking
        /// </summary>
        private static async Task<IResult> EditBooking(
            Guid id,
            [FromBody] EditBookingCommand command,
            IMediator mediator,
            ILogger<EditBookingCommand> logger)
        {
            try
            {
                // Ensure the ID in the route matches the command
                command.BookingId = id;
                
                logger.LogInformation("Updating booking with ID: {BookingId}", id);
                
                var result = await mediator.Send(command);
                
                logger.LogInformation("Booking updated successfully: {BookingId}", id);
                
                return Results.Ok(result);
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex, "Booking not found: {BookingId}", id);
                return Results.NotFound(new ProblemDetails
                {
                    Title = "Booking Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Invalid operation for booking: {BookingId}", id);
                return Results.Conflict(new ProblemDetails
                {
                    Title = "Cannot Update Booking",
                    Detail = ex.Message,
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (ValidationException ex)
            {
                logger.LogWarning(ex, "Validation failed for booking creation");
                
                // ✅ Correct way to handle ValidationException
                var errors = ex.Errors?
                    .ToDictionary(
                        error => error.Key,           // ← Use Key (PropertyName)
                        error => error.Value          // ← Use Value (string[])
                    ) ?? new Dictionary<string, string[]>();
                
                return Results.BadRequest(new ValidationProblemDetails
                {
                    Title = "Validation Failed",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Errors = errors
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating booking: {BookingId}", id);
                return Results.Problem(
                    title: "An error occurred",
                    detail: "Unable to update booking. Please try again later.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        /// <summary>
        /// Deletes a booking
        /// </summary>
        private static async Task<IResult> DeleteBooking(
            Guid id,
            IMediator mediator,
            ILogger<DeleteBookingCommand> logger)
        {
            try
            {
                logger.LogInformation("Deleting booking with ID: {BookingId}", id);
                
                var command = new DeleteBookingCommand { BookingId = id };
                var result = await mediator.Send(command);
                
                if (!result)
                {
                    logger.LogWarning("Booking not found for deletion: {BookingId}", id);
                    return Results.NotFound(new ProblemDetails
                    {
                        Title = "Booking Not Found",
                        Detail = $"Booking with ID {id} was not found.",
                        Status = StatusCodes.Status404NotFound
                    });
                }
                
                logger.LogInformation("Booking deleted successfully: {BookingId}", id);
                
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Invalid deletion for booking: {BookingId}", id);
                return Results.Conflict(new ProblemDetails
                {
                    Title = "Cannot Delete Booking",
                    Detail = ex.Message,
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting booking: {BookingId}", id);
                return Results.Problem(
                    title: "An error occurred",
                    detail: "Unable to delete booking. Please try again later.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        /// <summary>
        /// Gets all bookings (Admin only)
        /// </summary>
        private static async Task<IResult> GetAllBookings(
            IMediator mediator,
            ILogger<GetAllBookingsQuery> logger)
        {
            try
            {
                logger.LogInformation("Retrieving all bookings");
                
                var query = new GetAllBookingsQuery();
                var result = await mediator.Send(query);
                
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all bookings");
                return Results.Problem(
                    title: "An error occurred",
                    detail: "Unable to retrieve bookings. Please try again later.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}