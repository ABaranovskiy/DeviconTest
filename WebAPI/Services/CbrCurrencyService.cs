using System.Globalization;
using System.Xml.Linq;
using WebAPI.Dto;

namespace WebAPI.Services;

public class CbrCurrencyService(HttpClient httpClient, IConfiguration configuration)
{
    public static readonly int AcceptableCurrenciesCount = 4;
    // Список обрабатываемых валют (EUR, USD, BYN/BYR, KZT)
    public static readonly HashSet<string> AcceptableCodes = ["EUR", "USD", "BYR", "BYN", "KZT"];

    // Получение курсов из ЦБ РФ за конкретную дату
    public async Task<List<CurrencyRateDto>> GetRatesForDateAsync(DateTime date)
    {
        var response = await httpClient.GetStringAsync(
            $"{configuration["CbrApiSettings:DailyUrl"]}?date_req={date:dd/MM/yyyy}");
        
        var doc = XDocument.Parse(response);
        var rates = new List<CurrencyRateDto>();
        
        // Парсинг XML-ответа с фильтрацией по AcceptableCodes
        foreach (var element in doc.Descendants("Valute"))
        {
            var code = element.Element("CharCode")?.Value;
            if (!string.IsNullOrEmpty(code) && AcceptableCodes.Contains(code))
            {
                rates.Add(new CurrencyRateDto(
                    code,
                    date,
                    decimal.Parse(
                        element.Element("Value")?.Value.Replace(",", ".") ?? "0", 
                        CultureInfo.InvariantCulture
                    ),
                    int.Parse(element.Element("Nominal")?.Value ?? "1")
                ));
            }
        }
        return rates;
    }

    public async Task<List<CurrencyRateDto>> GetRatesForDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var result = new List<CurrencyRateDto>();
        while (startDate <= endDate)
        {
            var rates = await GetRatesForDateAsync(startDate);
            result.AddRange(rates);
            startDate = startDate.AddDays(1);
        }
        return result;
    }
    
    public async Task<List<CurrencyRateDto>> GetRatesForLastMonthAsync()
    {
        var today = DateTime.Today;
        var monthAgo = DateTime.Today.AddDays(-30);
        
        return await GetRatesForDateRangeAsync(monthAgo, today);
    }
}