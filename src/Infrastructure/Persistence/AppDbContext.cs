using Microsoft.EntityFrameworkCore;
using ToursApp.Domain.Entities;
using ToursApp.Infrastructure.Persistence.Configurations;
// src/Infrastructure/Persistence/AppDbContext.cs
namespace ToursApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<PaymentIntent> paymentIntents => Set<PaymentIntent>();
    public DbSet<Charge> Charges => Set<Charge>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            modelBuilder.ApplyConfiguration(new BookingConfiguration());
            modelBuilder.ApplyConfiguration(new TourConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentIntentConfiguration());
            modelBuilder.ApplyConfiguration(new ChargeConfiguration());
            modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}

// src/Infrastructure/Persistence/Configurations/TourConfiguration.cs
