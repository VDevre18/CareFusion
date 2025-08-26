// Placeholder for Extensions/StringExtensions.cs
namespace CareFusion.Common.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? s) => string.IsNullOrWhiteSpace(s);

    public static string Truncate(this string s, int length, string suffix = "…")
    {
        if (string.IsNullOrEmpty(s) || s.Length <= length) return s;
        return s.Substring(0, Math.Max(0, length - suffix.Length)) + suffix;
    }
}
