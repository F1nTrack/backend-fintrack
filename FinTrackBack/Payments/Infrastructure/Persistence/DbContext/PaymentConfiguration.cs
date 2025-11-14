using FinTrackBack.Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrackBack.Payments.Infrastructure.Persistence.DbContext;


public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        
        builder.HasKey(p => p.Id);
        builder.Property(p => p.UserId).IsRequired();
        
        builder.Property(p => p.Servicio).IsRequired();
        
        builder.Property(p =>p.Monto).IsRequired();
        
        builder.Property(p => p.Fecha).IsRequired().IsRequired();
        
    }
}