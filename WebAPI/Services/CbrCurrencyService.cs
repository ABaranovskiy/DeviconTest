using System.Xml.Linq;
using WebAPI.Dto;

namespace WebAPI.Services;

public class CbrCurrencyService(HttpClient httpClient, IConfiguration configuration)
{
    // Список обрабатываемых валют (EUR, USD, BYN, KZT)
    public static readonly HashSet<string> AcceptableCodes = ["EUR", "USD", "BYN", "KZT"];

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
                    decimal.Parse(element.Element("Value")?.Value ?? "0"),
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
    
    public async Task<List<CurrencyRateDto>> GetRatesForThisMonthAsync()
    {
        var today = DateTime.Today;
        var startMonth = DateTime.Today.AddDays(-today.Day + 1);
        
        return await GetRatesForDateRangeAsync(startMonth, today);
    }
}