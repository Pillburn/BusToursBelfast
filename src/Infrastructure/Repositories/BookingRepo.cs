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

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
        => await _context.Bookings.FindAsync(id);

    public async Task<Booking?> GetByPaymentIntentIdAsync(string paymentIntentId, CancellationToken cancellationToken)
    {
        return await _context.Bookings
            .Include(b => b.Payment) // Ensure you have this navigation property
            .FirstOrDefaultAsync(b => b.Payment != null &&
                                  b.Payment.PaymentIntentId == paymentIntentId);
    }

    public async Task<IEnumerable<Booking>> GetByTourIdAsync(Guid tourId)
        => await _context.Bookings
            .Where(b => b.TourId == tourId)
            .ToListAsync();

    public async Task AddAsync(Booking booking)
        => await _context.Bookings.AddAsync(booking);

    public async Task UpdateAsync(Booking booking)
        => _context.Bookings.Update(booking);

    public async Task DeleteAsync(Guid id)
    {
        var booking = await GetByIdAsync(id);
        if (booking != null)
            _context.Bookings.Remove(booking);
    }
    
   public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}