using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToursApp.Domain.Entities;
public class PaymentIntentConfiguration : IEntityTypeConfiguration<PaymentIntent>
{
    public void Configure(EntityTypeBuilder<PaymentIntent> builder)
    {
        builder.Property(pi => pi.Amount)
               .HasPrecision(18, 2);
        
        builder.Property(pi => pi.Status)
               .HasConversion<string>(); // Store enum as string
        
        builder.HasIndex(pi => pi.StripePaymentIntentId)
               .IsUnique();
        
        builder.Property(pi => pi.ApplicationReference)
               .HasMaxLength(50);
    }
}