using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CONSTANTS;

public static class SecretStore
{
    public static class Keys
    {
        public const string OracleIP = "OracleIP";
        public const string OraclePort = "OraclePort";
        public const string OracleServiceName = "OracleServiceName";
        public const string OracleUsername = "OracleUsername";
        public const string OraclePassword = "OraclePassword";
    }

    private static readonly object _lock = new();
    private static readonly string _secretsPath = Path.Combine(AppContext.BaseDirectory, "secrets.json");

    public static void SetSecret(string key, string plaintext)
    {
        lock (_lock)
        {
            var secrets = LoadSecrets();
            var encrypted = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(plaintext),
                null,
                DataProtectionScope.LocalMachine);
            secrets[key] = Convert.ToBase64String(encrypted);
            SaveSecrets(secrets);
        }
    }

    public static string? GetSecret(string key)
    {
        lock (_lock)
        {
            var secrets = LoadSecrets();
            if (!secrets.TryGetValue(key, out var base64))
                return null;

            var encrypted = Convert.FromBase64String(base64);
            var decrypted = ProtectedData.Unprotect(
                encrypted,
                null,
                DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(decrypted);
        }
    }

    public static bool HasSecret(string key)
    {
        lock (_lock)
        {
            var secrets = LoadSecrets();
            return secrets.ContainsKey(key);
        }
    }

    public static bool HasAllOracleCredentials()
    {
        lock (_lock)
        {
            var secrets = LoadSecrets();
            return secrets.ContainsKey(Keys.OracleIP)
                && secrets.ContainsKey(Keys.OraclePort)
                && secrets.ContainsKey(Keys.OracleServiceName)
                && secrets.ContainsKey(Keys.OracleUsername)
                && secrets.ContainsKey(Keys.OraclePassword);
        }
    }

    private static Dictionary<string, string> LoadSecrets()
    {
        if (!File.Exists(_secretsPath))
            return new Dictionary<string, string>();

        var json = File.ReadAllText(_secretsPath);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
            ?? new Dictionary<string, string>();
    }

    private static void SaveSecrets(Dictionary<string, string> secrets)
    {
        var json = JsonSerializer.Serialize(secrets, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_secretsPath, json);
    }
}
