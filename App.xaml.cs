using System.Configuration;
using System.IO;
using System.Windows;
using CONSTANTS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using LVS3;
using mxClient;
using WpfMvvmApp.Services;
using WpfMvvmApp.Views;
using static LVS3.Enums;

namespace WpfMvvmApp;

public partial class App : Application
{
    public static bool IsInspecting { get; set; }

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

        // Read configuration
        var databaseType = ConfigurationManager.AppSettings["DatabaseType"] ?? "dummy";
        var savedImagesPath = ConfigurationManager.AppSettings["SavedImagesPath"];
        if (!string.IsNullOrEmpty(savedImagesPath))
            AppData.SavedImagesPath = savedImagesPath;
        var saveImages = ConfigurationManager.AppSettings["SaveImages"];
        if (saveImages?.ToLowerInvariant() == "true")
            AppData.SaveImages = true;

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
        services.AddSingleton<ICameraService, CameraService>();

        // Data manager — selected by config
        switch (databaseType.ToLowerInvariant())
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

        // PLC client — selected by config
        switch (ConfigurationManager.AppSettings["MxClient"]?.ToLowerInvariant())
        {
            case "dummy":
                services.AddSingleton<ImxClient, DummyPLCClient>();
                services.AddSingleton<IInspection, DummyInspection>();
                break;
            default:
                services.AddSingleton<ImxClient, LVS3.mxClient>();
                services.AddSingleton<IInspection, DummyInspection>(); // Real Inspection not yet fully decoupled
                break;
        }

        Services = services.BuildServiceProvider();

        // --- Initialisation sequence (matching old Program.cs) ---

        var dataManager = Services.GetRequiredService<IDataManager>();

        // Open database connection
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

        // Initialise messaging (Windows EventLog)
        Messaging.Init();

        // Set current user
        Defaults.UserLoggedIn = Environment.UserName;
        Defaults.UserName = Environment.UserName;

        // Audit trail: application start
        dataManager.SaveAction("Application Start", "LVS3", "", Defaults.UserLoggedIn,
            "Application", "starting application", "SUCCESS");

        // Initialise IO card
        // Note: SYSTEM_IO.Init requires IUtilityFunctions which is not yet implemented in WPF.
        // For production, provide a WPF implementation of IUtilityFunctions and uncomment:
        // var utilityFunctions = Services.GetRequiredService<IUtilityFunctions>();
        // if (SYSTEM_IO.Init(dataManager, utilityFunctions) == false)
        // {
        //     Shutdown();
        //     return;
        // }

        // Initialise PLC and put into safe state
        var mxClient = Services.GetRequiredService<ImxClient>();
        if (mxClient.INIT())
        {
            mxClient.ResetAlarm(1);
        }
        mxClient.Stop(1); // Safety stop on startup

        // Load image counts from database
        Defaults.VAMImageCount = dataManager.ImageCount(VAMImageTypes.VAM);
        Defaults.TestImageCount = dataManager.ImageCount(VAMImageTypes.TEST);

        // Turn on inspection lamp
        mxClient.InspectionLampOn();

        Log.Logger.Information("Initialisation complete, showing main window");

        // Create and show main window
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
