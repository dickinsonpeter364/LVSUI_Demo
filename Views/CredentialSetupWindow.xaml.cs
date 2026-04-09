using System.Windows;
using CONSTANTS;
using Oracle.ManagedDataAccess.Client;

namespace WpfMvvmApp.Views;

public partial class CredentialSetupWindow : Window
{
    public CredentialSetupWindow()
    {
        InitializeComponent();
    }

    private void OnTestConnection(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Testing connection...";
        StatusText.Foreground = System.Windows.Media.Brushes.Gray;
        BtnSave.IsEnabled = false;

        try
        {
            var connString = BuildConnectionString();
            using var conn = new OracleConnection(connString);
            conn.Open();
            conn.Close();

            StatusText.Text = "Connection successful.";
            StatusText.Foreground = System.Windows.Media.Brushes.Green;
            BtnSave.IsEnabled = true;
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Connection failed: {ex.Message}";
            StatusText.Foreground = System.Windows.Media.Brushes.Red;
            BtnSave.IsEnabled = false;
        }
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        SecretStore.SetSecret(SecretStore.Keys.OracleIP, TxtIP.Text.Trim());
        SecretStore.SetSecret(SecretStore.Keys.OraclePort, TxtPort.Text.Trim());
        SecretStore.SetSecret(SecretStore.Keys.OracleServiceName, TxtServiceName.Text.Trim());
        SecretStore.SetSecret(SecretStore.Keys.OracleUsername, TxtUsername.Text.Trim());
        SecretStore.SetSecret(SecretStore.Keys.OraclePassword, TxtPassword.Password);

        DialogResult = true;
        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private string BuildConnectionString()
    {
        var ip = TxtIP.Text.Trim();
        var port = TxtPort.Text.Trim();
        var serviceName = TxtServiceName.Text.Trim();
        var username = TxtUsername.Text.Trim();
        var password = TxtPassword.Password;

        return $"user id={username};password={password};data source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST={ip})(PORT={port}))(CONNECT_DATA=(SERVICE_NAME={serviceName})))";
    }
}
