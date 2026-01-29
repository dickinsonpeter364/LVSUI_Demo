using System.Windows.Input;
using System.Windows;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;

namespace WpfMvvmApp.ViewModels
{
    public class AuthoriseViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private string _username = string.Empty;
        private string _title = "Authorise";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isUsernameReadOnly = true;
        public bool IsUsernameReadOnly
        {
            get => _isUsernameReadOnly;
            set => SetProperty(ref _isUsernameReadOnly, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        private readonly Action? _onSuccessNavigation;

        public AuthoriseViewModel(INavigationService navigationService, Action? onSuccessNavigation = null, string title = "Authorise", string? initialUsername = null, bool isUsernameReadOnly = true)
        {
            _navigationService = navigationService;
            _onSuccessNavigation = onSuccessNavigation;
            Title = title;
            IsUsernameReadOnly = isUsernameReadOnly;
            
            // Set current domain and username or use provided
            Username = initialUsername ?? $"{Environment.UserDomainName}\\{Environment.UserName}";
            
            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private void OnOk(object? parameter)
        {
            // In a real app, we would validate credentials here.
            // parameter can be the PasswordBox to retrieve the password securely.
            string? password = (parameter as System.Windows.Controls.PasswordBox)?.Password;

            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(password))
            {
               // Removed message box as requested
               // MessageBox.Show($"Authorising user: {Username}", "Authorisation", MessageBoxButton.OK, MessageBoxImage.Information);
               
               if (_onSuccessNavigation != null)
               {
                   _onSuccessNavigation();
               }
               else
               {
                   // Default behavior if no specific callback provided (legacy support for simple login)
                   // _navigationService.Navigate(new Views.LpnEntryView(new LpnEntryViewModel(_navigationService)));
                   // But wait, the previous logic was specific to startup. 
                   // Let's assume if no callback, we do default startup flow or simple GoBack?
                   // The user didn't specify what to do for standard login, but "Change LabelInvestigationView...".
                   // Let's keep the old default for now if it's not the LabelInvestigation case, OR
                   // better yet, we can pass the specific startup flow in MainWindow.
                   
                   _navigationService.Navigate(new Views.LpnEntryView(new LpnEntryViewModel(_navigationService)));
               }
            }
            else
            {
                MessageBox.Show("Please enter username and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnCancel(object? parameter)
        {
            if (_navigationService.CanGoBack)
            {
                _navigationService.GoBack();
            }
        }
    }
}
