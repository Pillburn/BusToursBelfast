// Application/Interfaces/IApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using ToursApp.Domain.Entities;

namespace ToursApp.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Booking> Bookings { get; }
    DbSet<Tour> Tours { get; }
    DbSet<PaymentIntent> PaymentIntents { get; }
    DbSet<Charge> Charges { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}