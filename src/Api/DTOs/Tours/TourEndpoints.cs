using Microsoft.AspNetCore.Mvc;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Enums;
using ToursApp.Domain.Interfaces;
using ToursApp.Application.Common.Interfaces;
using ToursApp.Application.DTOs.Booking;

namespace ToursApp.API.Endpoints
{
    public static class TourEndpoints
    {
        public static void MapTourEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/tour").WithTags("Tour");

            group.MapGet("/", GetAllTours);
            group.MapGet("/{id}", GetTourById);
            group.MapGet("/first", GetFirstTour);
            group.MapGet("/seed", SeedTours);
            group.MapPost("/bookings", CreateBooking);
        }

        private static async Task<IResult> GetAllTours(ITourRepository tourRepository)
        {
            var tours = await tourRepository.GetAllToursAsync();
            return Results.Ok(tours);
        }

        private static async Task<IResult> GetTourById(Guid id, ITourRepository tourRepository)
        {
            var tour = await tourRepository.GetTourByIdAsync(id);
            return tour is not null ? Results.Ok(tour) : Results.NotFound();
        }

        private static async Task<IResult> GetFirstTour(
            ITourRepository tourRepository,
            ILogger<Program> logger)
        {
            try
            {
                logger.LogInformation("🔍 Getting first tour...");
                
                var tours = await tourRepository.GetAllToursAsync();
                
                if (tours == null || !tours.Any())
                {
                    logger.LogWarning("⚠️ No tours found");
                    return Results.NotFound(new 
                    { 
                        message = "No tours found. Please seed some tours first.",
                        hint = "Try calling /api/tour/seed to create sample tours."
                    });
                }
                
                var firstTour = tours.First();
                
                logger.LogInformation("✅ Found first tour: {Id} - {Title}", firstTour.Id, firstTour.Title);
                
                return Results.Ok(new
                {
                    id = firstTour.Id,
                    name = firstTour.Title,
                    title = firstTour.Title,
                    description = firstTour.Description,
                    price = firstTour.Price,
                    location = firstTour.Location,
                    groupsize = firstTour.GroupSize,
                    tourDate = firstTour.TourDate,
                    maxCapacity = firstTour.MaxCapacity,
                    isActive = firstTour.IsActive
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error getting first tour: {Message}", ex.Message);
                return Results.Problem($"Error retrieving tour: {ex.Message}");
            }
        }

        private static async Task<IResult> SeedTours(
            ITourRepository tourRepository,
            ILogger<Program> logger)
        {
            try
            {
                logger.LogInformation("🌱 Starting tour seeding...");
                
                var existingTours = await tourRepository.GetAllToursAsync();
                
                if (existingTours != null && existingTours.Any())
                {
                    logger.LogInformation("📊 Tours already exist: {Count}", existingTours.Count());
                    return Results.Ok(new 
                    { 
                        message = "Tours already exist", 
                        count = existingTours.Count(),
                        tours = existingTours.Select(t => new { t.Id, t.Title })
                    });
                }
                
                logger.LogInformation("🌱 No tours found, seeding sample tours...");
                
                var seedTours = new[]
                {
                    new Tour(
                        description: "Discover the rich history and culture of Belfast, from the Titanic Quarter to the City Hall.",
                        name: "Belfast City Explorer",
                        price: 25m,
                        groupSize: 1,
                        tourDate: DateTime.UtcNow.AddDays(7),
                        createdBy: "system"
                    )
                    {
                        Id = Guid.NewGuid(),
                        Location = "Belfast, Northern Ireland",
                        IsActive = true,
                        MaxCapacity = 20
                    },
                    new Tour(
                        description: "Experience the Giants Causeway, one of Ireland's most famous landmarks.",
                        name: "Causeway Coast Adventure",
                        price: 20m,
                        groupSize: 1,
                        tourDate: DateTime.UtcNow.AddDays(14),
                        createdBy: "system"
                    )
                    {
                        Id = Guid.NewGuid(),
                        Location = "Causeway Coast, Northern Ireland",
                        IsActive = true,
                        MaxCapacity = 15
                    },
                    new Tour(
                        description: "Explore the beautiful coastal routes and historic sites along the Antrim Coast.",
                        name: "Antrim Coast & Glens",
                        price: 35m,
                        groupSize: 1,
                        tourDate: DateTime.UtcNow.AddDays(21),
                        createdBy: "system"
                    )
                    {
                        Id = Guid.NewGuid(),
                        Location = "Antrim Coast, Northern Ireland",
                        IsActive = true,
                        MaxCapacity = 12
                    }
                };
                
                foreach (var tour in seedTours)
                {
                    logger.LogInformation("📝 Creating tour: {Title}", tour.Title);
                    await tourRepository.CreateTourAsync(tour);
                }
                
                logger.LogInformation("✅ Successfully seeded {Count} tours", seedTours.Length);
                
                return Results.Ok(new 
                { 
                    message = $"Successfully seeded {seedTours.Length} tours",
                    count = seedTours.Length,
                    tours = seedTours.Select(t => new { t.Id, t.Title })
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error seeding tours: {Message}", ex.Message);
                return Results.Problem($"Error seeding tours: {ex.Message}");
            }
        }

        public static async Task<IResult> CreateBooking(
            CreateBookingRequest request,
            IBookingRepository bookingRepository,
            ITourRepository tourRepository,
            IPaymentGateway paymentGateway,
            IUnitOfWork unitOfWork,
            ILogger<Program> logger)
        {
            try
            {
                logger.LogInformation("📝 CreateBooking request: {@Request}", new
                {
                    request.TourId,
                    request.TourName,
                    request.CustomerName,
                    request.Email,
                    request.PhoneNumber,
                    request.PreferredDate,
                    request.PickupLocation,
                    Participants = request.NumberOfParticipants
                });

                if (!Guid.TryParse(request.TourId, out var tourGuid))
                {
                    logger.LogWarning("❌ Invalid TourId format: {TourId}", request.TourId);
                    return Results.BadRequest($"Invalid TourId format: {request.TourId}");
                }

                var tour = await tourRepository.GetTourByIdAsync(tourGuid);
                if (tour is null)
                {
                    logger.LogWarning("❌ Invalid TourId format: {TourId}", request.TourId);
                    return Results.NotFound($"Tour with ID {request.TourId} not found");
                }

                logger.LogWarning("❌ Invalid TourId format: {TourId}", request.TourId);

                var totalParticipants = request.NumberOfParticipants.Adults + 
                                       request.NumberOfParticipants.Children + 
                                       request.NumberOfParticipants.Infants;
                var totalAmount = tour.Price * (request.NumberOfParticipants.Adults + request.NumberOfParticipants.Children);

                logger.LogInformation("💰 Total: {TotalParticipants} participants, {TotalAmount} amount", 
                    totalParticipants, totalAmount);

                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    TourId = tourGuid,
                    TourName = request.TourName,
                    CustomerName = request.CustomerName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    NumberOfAdults = request.NumberOfParticipants.Adults,
                    NumberOfChildren = request.NumberOfParticipants.Children,
                    NumberOfInfants = request.NumberOfParticipants.Infants,
                    PreferredDate = DateOnly.Parse(request.PreferredDate),
                    PickupLocation = request.PickupLocation,
                    SpecialRequests = request.SpecialRequests,
                    PassportNumber = request.PassportNumber,
                    DateOfBirth = request.DateOfBirth != null ? DateOnly.Parse(request.DateOfBirth) : null,
                    EmergencyContact = request.EmergencyContact,
                    TravelInsuranceDetails = request.TravelInsuranceDetails,
                    BookingDate = DateTime.UtcNow,
                    Status = BookingStatus.Pending,
                    TotalParticipants = totalParticipants,
                    TotalPrice = totalAmount
                };

                await bookingRepository.CreateBookingAsync(booking);
                await unitOfWork.SaveChangesAsync();

                logger.LogInformation("✅ Booking created: {BookingId}", booking.Id);

                var paymentIntentResult = await paymentGateway.CreatePaymentIntentAsync(
                    amount: totalAmount,
                    currency: "GBP",
                    metadata: new Dictionary<string, string>
                    {
                        { "bookingId", booking.Id.ToString() },
                        { "referenceNumber", booking.ReferenceNumber ?? "N/A" },
                        { "customerEmail", booking.Email },
                        { "customerName", booking.CustomerName }
                    }
                );

                if (!paymentIntentResult.Success)
                {
                    logger.LogError("❌ Payment intent creation failed: {ErrorMessage}", paymentIntentResult.ErrorMessage);
                    return Results.Problem("Payment processing failed. Please try again.");
                }

                booking.PaymentIntentId = paymentIntentResult.PaymentIntentId ?? string.Empty;
                await bookingRepository.UpdateBookingAsync(booking);
                await unitOfWork.SaveChangesAsync();

                logger.LogInformation("✅ Payment intent created: {PaymentIntentId}", paymentIntentResult.PaymentIntentId);

                return Results.Ok(new 
                { 
                    BookingId = booking.Id, 
                    ReferenceNumber = booking.ReferenceNumber,
                    Status = booking.Status.ToString(),
                    Message = "Booking created successfully",
                    ClientSecret = paymentIntentResult.ClientSecret,
                    PaymentIntentId = paymentIntentResult.PaymentIntentId,
                    TotalAmount = totalAmount,
                    Currency = "GBP",
                    RequiresPaymentMethod = true
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error creating booking: {Message}", ex.Message);
                logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
                return Results.Problem($"Error: {ex.Message}");
            }
        }
    }
}