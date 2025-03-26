using Microsoft.EntityFrameworkCore;

namespace WebAPI.Data;

public static class DbInitializer
{
    public static void Initialize(ExchangeDbContext context)
    {
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }

}