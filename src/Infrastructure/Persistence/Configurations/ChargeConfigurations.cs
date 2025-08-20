// Infrastructure/Persistence/Configurations/ChargeConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToursApp.Domain.Entities;

public class ChargeConfiguration : IEntityTypeConfiguration<Charge>
{
    public void Configure(EntityTypeBuilder<Charge> builder)
    {
        builder.Property(c => c.Amount)
              .HasPrecision(18, 2);
              
        builder.Property(c => c.AmountRefunded)
              .HasPrecision(18, 2)
              .HasDefaultValue(0);
              
        builder.Property(c => c.Status)
              .HasConversion<string>();
              
        builder.HasIndex(c => c.StripeChargeId)
              .IsUnique();
              
        builder.HasIndex(c => c.StripePaymentIntentId);
        
        builder.Property(c => c.PaymentMethodType)
              .HasMaxLength(30);
    }
}