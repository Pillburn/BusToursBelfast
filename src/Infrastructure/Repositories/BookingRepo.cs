// Infrastructure/Persistence/Repositories/BookingRepository.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Interfaces;
using ToursApp.Infrastructure.Persistence;

namespace ToursApp.Infrastructure.Repositories;

public class BookingRepo : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepo(AppDbContext context)
    {
        _context = context;
    }
    public async Task CreateBookingAsync(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));
            
            if (string.IsNullOrWhiteSpace(booking.CustomerName))
                throw new ArgumentException("CustomerName is required");
            
            if (string.IsNullOrWhiteSpace(booking.Email))
                throw new ArgumentException("Email is required");
            
            if (string.IsNullOrWhiteSpace(booking.PhoneNumber))
                throw new ArgumentException("PhoneNumber is required");

            // Set booking date if not set
            if (booking.BookingDate == default)
                booking.BookingDate = DateTime.UtcNow;

            // Set default status if not set
            

            // Calculate total participants if not set
            if (booking.TotalParticipants == 0)
            {
                booking.TotalParticipants = booking.NumberOfAdults + 
                                            booking.NumberOfChildren + 
                                            booking.NumberOfInfants;
            }

            // Calculate total price if not set
            if (booking.TotalPrice == 0)
            {
                // Adults and children pay, infants might be free
                booking.TotalPrice = booking.TotalPrice * (booking.NumberOfAdults + booking.NumberOfChildren);
            }

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }
    public async Task<Booking?> GetBookingByIdAsync(Guid id)
        => await _context.Bookings.FindAsync(id);

    public async Task<Booking?> GetBookingByPaymentIntentIdAsync(string paymentIntentId, CancellationToken cancellationToken)
    {
        return await _context.Bookings
            .Include(b => b.Payment) // Ensure you have this navigation property
            .FirstOrDefaultAsync(b => b.Payment != null &&
                                  b.Payment.PaymentIntentId == paymentIntentId);
    }

    public async Task<IEnumerable<Booking>> GetBookingByTourIdAsync(Guid tourId)
        => await _context.Bookings
            .Where(b => b.TourId == tourId)
            .ToListAsync();

    public async Task AddBookingAsync(Booking booking)
        => await _context.Bookings.AddAsync(booking);

    public  Task UpdateBookingAsync(Booking booking)
        =>  Task.FromResult(_context.Bookings.Update(booking));

    public async Task DeleteBookingAsync(Guid id)
    {
        var booking = await GetBookingByIdAsync(id);
        if (booking != null)
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
    }
    
   public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetUserBookingsAsync(string email)
    {
        return await _context.Bookings
                .Where(b => b.Email == email)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
    }

    public async Task<bool> BookingExistsAsync(Guid id)
    {
        return await _context.Bookings.AnyAsync(b => b.Id == id);
    }
}