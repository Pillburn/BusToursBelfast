// Infrastructure/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using ToursApp.Application.Common.Interfaces;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Interfaces;

namespace ToursApp.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for all your entities
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Charge> Charges { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<PaymentIntent> PaymentIntents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Booking configuration
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Id).ValueGeneratedOnAdd();
                
                entity.Property(b => b.ReferenceNumber)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(b => b.CustomerName)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(b => b.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(b => b.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.Property(b => b.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);
                
                entity.Property(b => b.TourId)
                    .IsRequired();
                
                entity.HasOne(b => b.Tour)
                    .WithMany(t => t.Bookings)
                    .HasForeignKey(b => b.TourId);
            });

            // Payment configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                
                entity.Property(p => p.Currency)
                    .IsRequired()
                    .HasMaxLength(3);
                
                entity.Property(p => p.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);
                
                entity.Property(p => p.ChargeId)
                    .HasMaxLength(100);
                
                entity.HasOne(p => p.Booking)
                    .WithMany()
                    .HasForeignKey(p => p.BookingId);
            });

            // Charge configuration
            modelBuilder.Entity<Charge>(entity =>
            {
                entity.HasKey(c => c.ChargeId);
                entity.Property(c => c.ChargeId).ValueGeneratedOnAdd();
                
                entity.Property(c => c.StripeChargeId)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(c => c.StripePaymentIntentId)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(c => c.PaymentMethodType)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(c => c.Currency)
                    .IsRequired()
                    .HasMaxLength(3);
                
                entity.HasOne(c => c.Payment)
                    .WithOne(p => p.Charge)
                    .HasForeignKey<Charge>(c => c.ChargeId);
            });

            // Tour configuration
            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();
                
                entity.Property(t => t.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(t => t.Description)
                    .HasMaxLength(1000);
                
                entity.Property(t => t.Price)
                    .HasPrecision(18, 2);
            });

            // PaymentIntent configuration
            modelBuilder.Entity<PaymentIntent>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                
                entity.Property(p => p.StripePaymentIntentId)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(p => p.Currency)
                    .IsRequired()
                    .HasMaxLength(3);
            });

            // Add indexes for performance
            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.ReferenceNumber)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.Email);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.PreferredDate);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.PaymentIntentId);

            modelBuilder.Entity<Charge>()
                .HasIndex(c => c.StripeChargeId)
                .IsUnique();
        }
    }
}