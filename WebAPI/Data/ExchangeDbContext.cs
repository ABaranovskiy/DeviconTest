using Microsoft.EntityFrameworkCore;
using WebAPI.Data.EntityConfigurations;
using WebAPI.Models;

namespace WebAPI.Data;

public class ExchangeDbContext(DbContextOptions<ExchangeDbContext> options) : DbContext(options)
{
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<CurrencyRate> CurrencyRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyRateConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}