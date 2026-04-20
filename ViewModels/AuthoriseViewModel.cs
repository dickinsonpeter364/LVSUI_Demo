using System.Windows.Input;
using System.Windows;
using CONSTANTS;
using LVS3;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;
using static LVS3.Enums;

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
            string? password = (parameter as System.Windows.Controls.PasswordBox)?.Password;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter username and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (App.BypassSecurity || App.IsDummyMode)
            {
                // Security bypassed — accept without AD validation
                Defaults.UserLoggedIn = Username;
                Defaults.UserName = Username;
                NavigateOnSuccess();
                return;
            }

            // Active Directory authentication
            try
            {
                string domain = Environment.UserDomainName;
                string user = Username;

                // Split domain\username if provided
                if (Username.Contains('\\') || Username.Contains('/'))
                {
                    var parts = Username.Split(new[] { '\\', '/' });
                    domain = parts[0];
                    user = parts[1];
                }

                if (!AD.AuthenticateUser(domain, user, password))
                {
                    MessageBox.Show("Authentication failed. Please check your credentials.",
                        "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Check AD group membership
                var userGroups = AD.ADUserGroups(user, domain);

                var userGroupsList = string.Join(", ", userGroups ?? new List<string>());
                var expectedList = AD.ADGroups == null
                    ? "(AD.ADGroups is null — AD.Init not run or DB returned no rows)"
                    : AD.ADGroups.Count == 0
                        ? "(AD.ADGroups is empty)"
                        : string.Join(", ", AD.ADGroups.Select(g => g.ADGroupName));
                Serilog.Log.Information("AD check for user '{User}@{Domain}'\n  userGroups: {UserGroups}\n  expected:   {Expected}",
                    user, domain, userGroupsList, expectedList);

                bool isMember = false;
                if (AD.ADGroups != null)
                {
                    foreach (var adGroup in AD.ADGroups)
                    {
                        if (userGroups.Any(g => g.Equals(adGroup.ADGroupName, StringComparison.OrdinalIgnoreCase)))
                        {
                            isMember = true;
                            break;
                        }
                    }
                }

                if (!isMember)
                {
                    MessageBox.Show(
                        $"You are not a member of an authorised group.\n\n" +
                        $"Your groups:\n  {userGroupsList}\n\n" +
                        $"Expected (any of):\n  {expectedList}",
                        "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Set logged in user details
                Defaults.UserLoggedIn = user;
                Defaults.UserName = AD.GetUserInfo(user, UserInfo.name);
                if (string.IsNullOrEmpty(Defaults.UserName))
                    Defaults.UserName = user;

                NavigateOnSuccess();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Authentication error: {ex.Message}",
                    "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateOnSuccess()
        {
            if (_onSuccessNavigation != null)
                _onSuccessNavigation();
            else
                _navigationService.Navigate(new Views.LpnEntryView(new LpnEntryViewModel(_navigationService)));
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
