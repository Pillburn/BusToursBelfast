// Application/Interfaces/IApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ToursApp.Domain.Entities;

namespace ToursApp.Application.Common.Interfaces;

public interface IApplicationDbContext : IDisposable
{
    DbSet<ToursApp.Domain.Entities.Booking> Bookings { get; }
    DbSet<Tour> Tours { get; }
    DbSet<PaymentIntent> PaymentIntents { get; }
    DbSet<Charge> Charges { get; }
    
    DatabaseFacade Database {get;}
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}