namespace WebAPI.Models;

public class CurrencyRate
{
    public string CurrencyCode { get; init; } 
    public DateTime Date { get; init; }
    public decimal Value { get; init; }

    public CurrencyRate(string currencyCode, DateTime date, decimal value)
    {
        CurrencyCode = currencyCode;
        Date = date;
        Value = value;
    }
}