// Placeholder for Security/EncryptionHelper.cs
using System.Security.Cryptography;
using System.Text;

namespace CareFusion.Common.Security;

public static class EncryptionHelper
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(hash);
    }

    public static bool VerifyPassword(string password, string storedHash)
        => string.Equals(HashPassword(password), storedHash, StringComparison.OrdinalIgnoreCase);
}
