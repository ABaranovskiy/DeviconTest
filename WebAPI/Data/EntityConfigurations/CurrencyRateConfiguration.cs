using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Models;

namespace WebAPI.Data.EntityConfigurations;

public class CurrencyRateConfiguration : IEntityTypeConfiguration<CurrencyRate>
{
    public void Configure(EntityTypeBuilder<CurrencyRate> builder)
    {
        builder.HasKey(x => new { x.CurrencyCode, x.Date });
        builder.HasIndex(x => new { x.CurrencyCode, x.Date }).IsUnique();
        builder.Property(x => x.CurrencyCode).HasMaxLength(5);
        
        builder.Property(x => x.Value).HasPrecision(18, 6);
    }
}