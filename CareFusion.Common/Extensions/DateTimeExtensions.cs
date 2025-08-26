// Placeholder for Extensions/DateTimeExtensions.cs
namespace CareFusion.Common.Extensions;

public static class DateTimeExtensions
{
    public static string ToShortIso(this DateTime dt) => dt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
}
