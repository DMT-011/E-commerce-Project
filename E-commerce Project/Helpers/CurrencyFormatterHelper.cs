namespace E_commerce_Project.Helpers;
using System.Globalization;

public class CurrencyFormatterHelper
{
    public static decimal RawValue(string? value)
    {
        var result = value.Replace(".", "").Trim();
        return decimal.Parse(result);
    }

    public static string Format(decimal value)
    {
        var culture = CultureInfo.GetCultureInfo("vi-VN");
        var result = value.ToString("N0", culture);
        return result;
    }
}    