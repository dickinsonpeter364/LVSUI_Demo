using System.IO;
using System.Windows;
using CONSTANTS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using LVS3;
using mxClient;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;
using WpfMvvmApp.Views;
using static LVS3.Enums;

namespace WpfMvvmApp;

public partial class App : Application
{
    public static bool IsInspecting { get; set; }
    public static bool IsDummyMode { get; private set; }
    public static bool BypassSecurity { get; private set; }
    public static bool CaptureOnly { get; private set; }
    public static bool LafCaptureTest { get; private set; }

    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Ensure Logs folder exists
        if (!Directory.Exists("Logs"))
            Directory.CreateDirectory("Logs");

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(@"Logs\log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Logger.Information("Application starting");

        // Read configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var appSettings = configuration.GetSection("App").Get<AppSettings>() ?? new AppSettings();
        var dbSettings = configuration.GetSection("Database").Get<DatabaseSettings>() ?? new DatabaseSettings();

        // Populate Defaults from config and DPAPI secrets
        ConfigLoader.LoadDefaults(
            appSettings.StationID,
            appSettings.IPAddressPLC,
            appSettings.DebugMode,
            dbSettings.SqlitePath);

        if (!string.IsNullOrEmpty(appSettings.SavedImagesPath))
            AppData.SavedImagesPath = appSettings.SavedImagesPath;
        if (appSettings.SaveImages)
            AppData.SaveImages = true;

        bool isDummyMode = appSettings.MxClient.Equals("dummy", StringComparison.OrdinalIgnoreCase);
        IsDummyMode = isDummyMode;
        BypassSecurity = appSettings.BypassSecurity;
        CaptureOnly = appSettings.CaptureOnly;
        LafCaptureTest = appSettings.LafCaptureTest;

        Log.Logger.Information(
            "Config read: DatabaseType={DB}, MxClient={Mx}, BypassSecurity={Bypass}, CaptureOnly={Cap}, SaveImages={Save}, SavedImagesPath='{Path}'",
            appSettings.DatabaseType, appSettings.MxClient,
            appSettings.BypassSecurity, appSettings.CaptureOnly,
            appSettings.SaveImages, appSettings.SavedImagesPath);

        if (!isDummyMode)
        {
            // Production: ensure Oracle credentials are available
            if (appSettings.DatabaseType.Equals("oracle", StringComparison.OrdinalIgnoreCase)
                && !SecretStore.HasAllOracleCredentials())
            {
                var setupWindow = new CredentialSetupWindow();
                if (setupWindow.ShowDialog() != true)
                {
                    Shutdown();
                    return;
                }
            }
            ConfigLoader.LoadOracleSecrets();
        }

        // Build DI container
        var services = new ServiceCollection();

        // Logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog();
        });

        // Core services
        services.AddSingleton<IMessagingService, MessagingService>();
        services.AddSingleton<IAlarmService, AlarmService>();
        services.AddSingleton<ICameraService, CameraService>();
        services.AddSingleton<IUtilityFunctions, WpfUtilityFunctions>();

        if (isDummyMode)
        {
            // Dummy mode: no database, no real PLC
            services.AddSingleton<IDataManager, DummyDataProvider>();
            services.AddSingleton<ImxClient, DummyPLCClient>();
            services.AddSingleton<IInspection, DummyInspection>();
        }
        else
        {
            // Production: config-driven data manager
            switch (appSettings.DatabaseType.ToLowerInvariant())
            {
                case "sqlite":
                    services.AddSingleton<IDataManager, SqLiteDataManager>();
                    break;
                case "oracle":
                    services.AddSingleton<IDataManager, OracleDataManager>();
                    break;
                case "capturing":
                    services.AddSingleton<IDataManager, CapturingDataManager>();
                    break;
                default:
                    services.AddSingleton<IDataManager, DummyDataProvider>();
                    break;
            }
            services.AddSingleton<ImxClient, LVS3.mxClient>();
            services.AddSingleton<IInspection, DummyInspection>();
        }

        Services = services.BuildServiceProvider();

        Defaults.UserLoggedIn = Environment.UserName;
        Defaults.UserName = Environment.UserName;

        if (!isDummyMode)
        {
            // Production: full initialisation sequence
            var dataManager = Services.GetRequiredService<IDataManager>();

            if (dataManager.OpenConnection(Defaults.SchemaToUse) == false)
            {
                Log.Logger.Error("Connection Failed. Cannot establish database connection. {Error}",
                    dataManager.ErrorDesription);
                MessageBox.Show(
                    "Connection Failed.\nThe system cannot continue because a database connection could not be established.\n\n" +
                    dataManager.ErrorDesription,
                    "Application Startup", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            Messaging.Init();

            // Initialize Active Directory groups (unless security bypassed)
            if (!BypassSecurity)
            {
                AD.Init(dataManager);
                Log.Logger.Information("AD.Init called. AD.ADGroups count = {Count}",
                    AD.ADGroups == null ? -1 : AD.ADGroups.Count);
            }
            else
            {
                Log.Logger.Information("AD.Init skipped because BypassSecurity=true");
            }

            dataManager.SaveAction("Application Start", "LVS3", "", Defaults.UserLoggedIn,
                "Application", "starting application", "SUCCESS");

            var utilityFunctions = Services.GetRequiredService<IUtilityFunctions>();
            if (SYSTEM_IO.Init(dataManager, utilityFunctions) == false)
            {
                Log.Logger.Error("SYSTEM_IO.Init failed: {Error}", SYSTEM_IO.FailDescription);
                Shutdown();
                return;
            }

            var mxClient = Services.GetRequiredService<ImxClient>();
            if (mxClient.INIT())
            {
                mxClient.ResetAlarm(1);
            }
            mxClient.Stop(1);

            Defaults.VAMImageCount = dataManager.ImageCount(VAMImageTypes.VAM);
            Defaults.TestImageCount = dataManager.ImageCount(VAMImageTypes.TEST);

            // Load PLC fail codes for user-friendly alarm descriptions
            var failCodes = dataManager.LoadFailCodes();
            if (failCodes != null)
                PLCFailCodes.FailCodes = failCodes;

            mxClient.InspectionLampOn();
        }

        // Load cameras in production or CaptureOnly mode (dummy mode skips hardware)
        if (!isDummyMode || CaptureOnly)
        {
            try
            {
                var cameraService = Services.GetRequiredService<ICameraService>();
                bool camsOk = cameraService.LoadCameras();
                Log.Logger.Information("LoadCameras() returned {Ok}, CamerasReady={Ready}",
                    camsOk, cameraService.CamerasReady);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "LoadCameras failed: {Message}", ex.Message);
            }
        }

        Log.Logger.Information("Initialisation complete{Mode}, showing main window",
            isDummyMode ? " (dummy mode)" : "");

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Audit trail: application exit
        try
        {
            var dataManager = Services?.GetService<IDataManager>();
            dataManager?.SaveAction("Application Exit", "LVS3", "", Defaults.UserName,
                "Application", "exit application", "");
        }
        catch { }

        // Turn off lamp and stop PLC
        try
        {
            var mxClient = Services?.GetService<ImxClient>();
            mxClient?.InspectionLampOff();
            mxClient?.Stop(1);
        }
        catch { }

        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
