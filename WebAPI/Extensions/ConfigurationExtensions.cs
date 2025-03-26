using WebAPI.Services;

namespace WebAPI.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddExchangeServer(this IServiceCollection services)
    {
        services.AddHttpClient<CbrCurrencyService>();
        services.AddHostedService<CurrencyBackgroundService>();

        return services;
    }

}