namespace Bookify.Domain.Shared;
public record Currency
{
    internal static readonly Currency None = new("");
    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");
    public Currency(string code)
    {
        Code = code;
    }
    public string Code { get; init; }
    public static Currency FromCode(string code) =>
        All.FirstOrDefault(x => x.Code == code) ??
        throw new ApplicationException();

    public static readonly IReadOnlyCollection<Currency> All = new[]
    {
        Usd,
        Eur
    };
}
