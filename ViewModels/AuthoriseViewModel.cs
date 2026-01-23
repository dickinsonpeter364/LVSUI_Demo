using System.Windows.Input;
using System.Windows;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;

namespace WpfMvvmApp.ViewModels
{
    public class AuthoriseViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private string _username;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public AuthoriseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private void OnOk(object parameter)
        {
            // In a real app, we would validate credentials here.
            // parameter can be the PasswordBox to retrieve the password securely.
            string password = (parameter as System.Windows.Controls.PasswordBox)?.Password;

            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(password))
            {
               MessageBox.Show($"Authorising user: {Username}", "Authorisation", MessageBoxButton.OK, MessageBoxImage.Information);
               // Navigate back or to home after successful auth
               // if (_navigationService.CanGoBack)
               //     _navigationService.GoBack();
               
               // Navigate to LpnEntryView as requested
               _navigationService.Navigate(new Views.LpnEntryView(new LpnEntryViewModel(_navigationService)));
            }
            else
            {
                MessageBox.Show("Please enter username and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnCancel(object parameter)
        {
            if (_navigationService.CanGoBack)
            {
                _navigationService.GoBack();
            }
        }
    }
}
