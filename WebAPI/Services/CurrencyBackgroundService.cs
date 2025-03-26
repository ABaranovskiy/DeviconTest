using WebAPI.Data;

namespace WebAPI.Services;

public class CurrencyBackgroundService(IServiceProvider services) : BackgroundService
{
    // Фоновая задача для ежедневного обновления курсов
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = services.CreateScope();
            var cbrService = scope.ServiceProvider.GetRequiredService<CbrCurrencyService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ExchangeDbContext>();

            // Заполняем базу значениями курсов валют за последний месяц
            var rates = await cbrService.GetRatesForThisMonthAsync();
            await CurrencyService.ReplaceRates(rates, dbContext, cancellationToken);

            await Task.Delay(TimeSpan.FromHours(24), cancellationToken);
        }
    }
}