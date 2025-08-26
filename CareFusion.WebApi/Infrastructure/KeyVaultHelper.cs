// Placeholder for Infrastructure/KeyVaultHelper.cs
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace CareFusion.WebApi.Infrastructure;

public class KeyVaultHelper
{
    private readonly SecretClient _secretClient;

    public KeyVaultHelper(string keyVaultUrl)
    {
        _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
    }

    public async Task<string?> GetSecretAsync(string secretName, CancellationToken ct = default)
    {
        var response = await _secretClient.GetSecretAsync(secretName, cancellationToken: ct);
        return response.Value.Value;
    }
}
