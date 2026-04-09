using System.Windows;
using System.Windows.Input;
using CONSTANTS;
using LVS3;
using Microsoft.Extensions.DependencyInjection;

namespace WpfMvvmApp.Views;

public partial class ESignatureWindow : Window
{
    private readonly Enums.ESigReason _reason;
    private readonly IDataManager _dataManager;
    private readonly bool _reasonRequired;
    private readonly bool _canCancel;

    public bool SignatureAccepted { get; private set; }
    public string LastUserName { get; private set; } = "";
    public string UserReason { get; private set; } = "";

    public ESignatureWindow(
        Enums.ESigReason reason,
        string reasonDescription,
        string errorDescription,
        bool reasonRequired = true,
        bool canCancel = false)
    {
        InitializeComponent();

        _reason = reason;
        _dataManager = App.Services.GetRequiredService<IDataManager>();
        _reasonRequired = reasonRequired;
        _canCancel = canCancel;

        // Set title based on reason
        Title = reason switch
        {
            Enums.ESigReason.EndReel => "Inspection Complete",
            Enums.ESigReason.EndReelAlarm => "Inspection Cancelled - Alarm",
            Enums.ESigReason.EndReelError => "Inspection Cancelled - Error",
            Enums.ESigReason.EndReelUser => "Inspection Cancelled by User",
            Enums.ESigReason.CancelLabelTraining => "Label Training Cancelled by User",
            Enums.ESigReason.CancelTrainingError => "Label Training Cancelled - Error",
            Enums.ESigReason.CancelTrainingAlarm => "Label Training Cancelled - Alarm",
            Enums.ESigReason.SaveTraining => "Save Label Training",
            Enums.ESigReason.LoginUser => "Log In",
            _ => "E-Signature Authorization"
        };

        LblSigMeaning.Text = reasonDescription;
        LblReason.Text = errorDescription;

        TxtUsername.Text = string.IsNullOrEmpty(Defaults.UserLoggedIn)
            ? Environment.UserName
            : Defaults.UserLoggedIn;

        if (!reasonRequired)
        {
            TxtUserReason.IsEnabled = false;
        }

        if (Title == "Inspection Complete")
        {
            LblInformation.Text = "End Report Information:";
            TxtUserReason.IsEnabled = true;
        }

        BtnCancel.IsEnabled = canCancel;

        Loaded += (_, _) =>
        {
            if (!string.IsNullOrWhiteSpace(TxtUsername.Text))
            {
                if (TxtUserReason.IsEnabled)
                    TxtUserReason.Focus();
                else
                    TxtPassword.Focus();
            }
            else
            {
                TxtUsername.Focus();
            }
        };
    }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        DoESign();
    }

    private void DoESign()
    {
        var username = TxtUsername.Text.Trim();
        var password = TxtPassword.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show(
                "Both your user-name and password are required to e-sign.",
                "E-Signature Details",
                MessageBoxButton.OK, MessageBoxImage.Information);

            if (string.IsNullOrEmpty(username))
                TxtUsername.Focus();
            else
                TxtPassword.Focus();
            return;
        }

        if (_reasonRequired && string.IsNullOrWhiteSpace(TxtUserReason.Text))
        {
            MessageBox.Show(
                $"You must provide {LblInformation.Text.TrimEnd(':')} details.",
                "E-Signature",
                MessageBoxButton.OK, MessageBoxImage.Information);
            TxtUserReason.Focus();
            return;
        }

        // TODO: When AD authentication is ported, validate credentials here.
        // For now, accept any non-empty username/password (matching AuthoriseViewModel pattern).
        SignatureAccepted = true;
        LastUserName = username;
        UserReason = TxtUserReason.Text.Trim();
        Defaults.UserSigning = username;

        _dataManager.SaveAction("Esign", Title, "", username, "DoESign()", "AUTHORIZED", UserReason);

        DialogResult = true;
        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        if (_reasonRequired && !_canCancel)
        {
            MessageBox.Show(
                "It is not possible to cancel because a reason is required.\n\nPlease click OK to submit an e-signature.",
                "E-Signature",
                MessageBoxButton.OK, MessageBoxImage.Information);
            TxtUserReason.IsEnabled = true;
            TxtUserReason.Focus();
            return;
        }

        LastUserName = !string.IsNullOrWhiteSpace(TxtUsername.Text)
            ? TxtUsername.Text.Trim()
            : Environment.UserName;
        SignatureAccepted = false;
        DialogResult = false;
        Close();
    }

    private void TxtUsername_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            TxtPassword.Focus();
            e.Handled = true;
        }
    }

    private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (TxtUserReason.IsEnabled && string.IsNullOrWhiteSpace(TxtUserReason.Text))
            {
                TxtUserReason.Focus();
                return;
            }
            DoESign();
            e.Handled = true;
        }
    }
}
