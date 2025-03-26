namespace WebAPI.Models;

public class Currency
{
    public string Code { get; init; }
    public int Nominal { get; init; }

    public Currency(string code, int nominal)
    {
        Code = code;
        Nominal = nominal;
    }
}