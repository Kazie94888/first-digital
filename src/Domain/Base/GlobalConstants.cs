namespace SmartCoinOS.Domain.Base;

public static class GlobalConstants
{
    public static class AuditParameters
    {
        public const string ClientId = nameof(ClientId);
        public const string OrderId = nameof(OrderId);
    }

    public static class Currency
    {
        public const string Hkusd = "HKUSD";
        public const string Fdusd = "FDUSD";
        public const string Usd = "USD";

        public static IEnumerable<string> SupportedCurrencies()
        {
            yield return Hkusd;
            yield return Fdusd;
            yield return Usd;
        }

        public static IEnumerable<string> SupportedStablecoins()
        {
            yield return Fdusd;
            yield return Hkusd;
        }
    }

    public static class Country
    {
        public const string Usa = "United States of America";
        public const string Jpn = "Japan";
        public const string Chn = "China";
        public const string Hkg = "Hong Kong";
        public const string Mys = "Malaysia";

        public static IEnumerable<string> SupportedCountries()
        {
            yield return Usa;
            yield return Jpn;
            yield return Chn;
            yield return Hkg;
            yield return Mys;
        }
    }
}
