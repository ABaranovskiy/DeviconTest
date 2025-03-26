using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Dto;
using WebAPI.Models;

namespace WebAPI.Services;

public static class CurrencyService
{
    // Атомарная замена всех данных в базе
    public static async Task ReplaceRates(List<CurrencyRateDto> rates, ExchangeDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (rates.Count == 0)
        {
            return;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Полная очистка таблиц перед вставкой новых данных
            await dbContext.Currencies.ExecuteDeleteAsync(cancellationToken);
            await dbContext.CurrencyRates.ExecuteDeleteAsync(cancellationToken);

            var currencies = rates
                .DistinctBy(x => x.CurrencyCode)
                .Select(x => new Currency(x.CurrencyCode, x.Nominal));
            var currencyRates = rates.Select(x => new CurrencyRate(x.CurrencyCode, x.Date, x.Value));

            await dbContext.Currencies.AddRangeAsync(currencies, cancellationToken);
            await dbContext.CurrencyRates.AddRangeAsync(currencyRates, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}