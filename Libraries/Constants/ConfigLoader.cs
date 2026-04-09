namespace CONSTANTS;

public static class ConfigLoader
{
    public static void LoadDefaults(
        int stationId,
        string ipAddressPlc,
        bool debugMode,
        string sqlitePath)
    {
        Defaults.StationID = stationId;
        Defaults.IPAddressPLC = ipAddressPlc;
        Defaults.DebugMode = debugMode;
        Defaults.SqlitePath = sqlitePath;
    }

    public static void LoadOracleSecrets()
    {
        Defaults.IP = SecretStore.GetSecret(SecretStore.Keys.OracleIP) ?? "";
        Defaults.PORT = SecretStore.GetSecret(SecretStore.Keys.OraclePort) ?? "";
        Defaults.DB_ServiceName = SecretStore.GetSecret(SecretStore.Keys.OracleServiceName) ?? "";
        Defaults.DB_UserName = SecretStore.GetSecret(SecretStore.Keys.OracleUsername) ?? "";
        Defaults.DB_Password = SecretStore.GetSecret(SecretStore.Keys.OraclePassword) ?? "";
    }
}
