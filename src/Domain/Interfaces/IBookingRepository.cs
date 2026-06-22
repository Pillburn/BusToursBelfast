using System;
using ToursApp.Domain.Entities;

namespace ToursApp.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetBookingByIdAsync(Guid id);
    Task<IEnumerable<Booking>> GetBookingByTourIdAsync(Guid tourId);
    Task<IEnumerable<Booking>> GetUserBookingsAsync(string email);
    Task CreateBookingAsync(Booking booking);
    Task AddBookingAsync(Booking booking);
    Task UpdateBookingAsync(Booking booking);
    Task DeleteBookingAsync(Guid id);
    Task<bool> BookingExistsAsync(Guid id);
    Task<Booking?> GetBookingByPaymentIntentIdAsync(string paymentIntentId, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

