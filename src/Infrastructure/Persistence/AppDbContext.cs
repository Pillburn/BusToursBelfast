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
namespace ToursApp.Infrastructure.Persistence.Configurations;

public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Name)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(t => t.Description)
            .HasMaxLength(1000);
            
        builder.Property(t => t.Price)
            .HasColumnType("decimal(18,2)");
    }
}