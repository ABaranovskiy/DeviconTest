namespace WebAPI.Dto;

public class CurrencyRateDto
{
    public string CurrencyCode { get; init; }
    public DateTime Date { get; init; }
    public decimal Value { get; init; }
    public int Nominal { get; init; }

    public CurrencyRateDto(string currencyCode, DateTime date, decimal value, int nominal)
    {
        CurrencyCode = currencyCode;
        Date = date;
        Value = value;
        Nominal = nominal;
    }
}