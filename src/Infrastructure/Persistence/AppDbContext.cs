using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Interfaces; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToursApp.Infrastructure.Persistence;
using ToursApp.Infrastructure.Repositories; 
// src/Infrastructure/Persistence/AppDbContext.cs
namespace ToursApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Booking> Bookings => Set<Booking>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}

// src/Infrastructure/Persistence/Configurations/TourConfiguration.cs
