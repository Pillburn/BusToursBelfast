using ToursApp.Domain.Interfaces;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Enums;
internal static class TourEndpoints
{
    public static void MapTourEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/tour").WithTags("Tour");

        group.MapGet("/", GetAllTours);
        group.MapGet("/{id}", GetTourById);

        //Adding booking endpoints

        group.MapPost("/bookings", CreateBooking);
        group.MapGet("/bookings/{bookingId}",GetBookingById);
        group.MapGet("/user/{userId}/bookings", GetUserBookings);
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

    private static async Task<IResult> CreateBooking(
        CreateBookingRequest request, 
        IBookingRepository bookingRepository,
        ITourRepository tourRepository)
    {
        // Validate tour exists
        var tour = await tourRepository.GetTourByIdAsync(request.TourId);
        if (tour is null)
        {
            return Results.NotFound($"Tour with ID {request.TourId} not found");
        }

        // Create booking entity
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            TourId = request.TourId,
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
            TotalParticipants = request.NumberOfParticipants.Adults + 
                               request.NumberOfParticipants.Children + 
                               request.NumberOfParticipants.Infants,
            TotalPrice = tour.Price * (request.NumberOfParticipants.Adults + request.NumberOfParticipants.Children)
            // Note: Infants might be free or discounted based on your business rules
        };

        await bookingRepository.CreateBookingAsync(booking);
        
        // TODO: Send confirmation email
        // await emailService.SendBookingConfirmation(booking);
        
        return Results.Ok(new { 
            BookingId = booking.Id, 
            Status = booking.Status,
            Message = "Booking created successfully" 
        });
    }

    // Optional: Get booking by ID
   private static async Task<IResult> GetBookingById(
    string bookingId, 
    IBookingRepository bookingRepository)
{
    // Validate and parse the GUID
    if (!Guid.TryParse(bookingId, out var guidId))
    {
        return Results.BadRequest("Invalid booking ID format. Must be a valid GUID.");
    }

    var booking = await bookingRepository.GetBookingByIdAsync(guidId);
    return booking is not null ? Results.Ok(booking) : Results.NotFound();
}

    // Optional: Get all bookings for a user (by email or user ID)
    private static async Task<IResult> GetUserBookings(
        string userId, 
        IBookingRepository bookingRepository)
    {
        var bookings = await bookingRepository.GetUserBookingsAsync(userId);
        return Results.Ok(bookings);
    }
}