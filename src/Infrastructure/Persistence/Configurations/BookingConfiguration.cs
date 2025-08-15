// Infrastructure/Persistence/Configurations/BookingConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToursApp.Domain.Entities;

namespace ToursApp.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        // Primary Key
        builder.HasKey(b => b.Id);
        
        // Property Configurations
        builder.Property(b => b.Id)
               .ValueGeneratedOnAdd()
               .IsRequired();

        builder.Property(b => b.TourId)
               .IsRequired();

        builder.Property(b => b.CustomerName)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(b => b.CustomerEmail)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(b => b.BookingDate)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()"); // Or "CURRENT_TIMESTAMP" for other databases

        builder.Property(b => b.NumberOfPeople)
               .IsRequired();

        builder.Property(b => b.TotalPrice)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(b => b.IsConfirmed)
               .HasDefaultValue(false)
               .IsRequired();

        // Relationships
        builder.HasOne(b => b.Tour)
               .WithMany() // Assuming Tour has no collection of Bookings
               .HasForeignKey(b => b.TourId)
               .OnDelete(DeleteBehavior.Restrict); // Prevents cascade delete

        // Indexes
        builder.HasIndex(b => b.CustomerEmail);
        builder.HasIndex(b => b.BookingDate);
        builder.HasIndex(b => b.TourId);

        // Table Naming (optional)
        builder.ToTable("Bookings");
    }
}