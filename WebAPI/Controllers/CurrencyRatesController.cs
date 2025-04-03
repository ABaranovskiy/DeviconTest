using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebAPI.Data;
using WebAPI.Dto;
using WebAPI.Services;

namespace WebAPI.Controllers;

public class CurrencyRatesController(ExchangeDbContext context, CbrCurrencyService cbrCurrencyService) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<CurrencyRateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] IMemoryCache cache, DateTime? startDate, DateTime? endDate)
    {
        // Кэширование результатов на 10 минут для одинаковых диапазонов дат
        var cacheKey = $"{startDate}-{endDate}";
        if (cache.TryGetValue(cacheKey, out List<CurrencyRateDto>? cachedRates))
        {
            return Ok(cachedRates);
        }

        // Базовый запрос с фильтрацией по разрешенным валютам
        var query = context.CurrencyRates
            .AsNoTracking()
            .Where(c => CbrCurrencyService.AcceptableCodes.Contains(c.CurrencyCode))
            .Join(context.Currencies,
                rate => rate.CurrencyCode,
                currency => currency.Code,
                (rate, currency) => new { Rate = rate, Currency = currency });


        if (startDate.HasValue)
        {
            query = query.Where(x => x.Rate.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(x => x.Rate.Date <= endDate.Value);
        }

        var orderedQuery = query
            .OrderByDescending(x => x.Rate.Date)
            .Take(10 * CbrCurrencyService.AcceptableCurrenciesCount);

        var rates = await orderedQuery
            .Select(x => new CurrencyRateDto(
                x.Rate.CurrencyCode,
                x.Rate.Date,
                x.Rate.Value,
                x.Currency.Nominal
            ))
            .ToListAsync();

        // Изменение порядка для возврата от старых к новым
        rates.Reverse();

        // Если в БД нет значений за указанный период
        if (startDate.HasValue
            && endDate.HasValue
            && rates.Count / CbrCurrencyService.AcceptableCurrenciesCount < (endDate - startDate).Value.Days)
        {
            var uploadedRates = await cbrCurrencyService.GetRatesForDateRangeAsync(startDate.Value, endDate.Value);

            return Ok(uploadedRates);
        }
        
        cache.Set(cacheKey, rates, TimeSpan.FromMinutes(10));
        return Ok(rates);
    }

    [HttpPost("update")]
    [ProducesResponseType(typeof(UpdateResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update()
    {
        var cancellationTokenSource = new CancellationTokenSource();

        try
        {
            var rates = await cbrCurrencyService.GetRatesForLastMonthAsync();
            await CurrencyService.ReplaceRates(rates, context, cancellationTokenSource.Token);

            return Ok(new UpdateResponseDto(true, DateTime.UtcNow));
        }
        catch (Exception ex)
        {
            await cancellationTokenSource.CancelAsync();
            return StatusCode(500, new
            {
                error = ex.Message
            });
        }
    }
}