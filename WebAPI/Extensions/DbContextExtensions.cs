using Microsoft.EntityFrameworkCore;
using WebAPI.Data;

namespace WebAPI.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Connection string is null or empty");
        
        services.AddDbContext<ExchangeDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }
}