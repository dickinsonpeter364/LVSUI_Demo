using System.Windows.Input;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;
using WpfMvvmApp.Views;

namespace WpfMvvmApp.ViewModels
{
    public class LpnEntryViewModel : ViewModelBase
    {
        private readonly INavigationService? _navigationService;
        private string _lpnNumber = string.Empty;

        public string LpnNumber
        {
            get => _lpnNumber;
            set
            {
                if (SetProperty(ref _lpnNumber, value))
                {
                    // Trigger re-evaluation of CanExecute for OK command
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public LpnEntryViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            OkCommand = new RelayCommand(OnOk, CanExecuteOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private bool CanExecuteOk(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(LpnNumber);
        }

        private void OnOk(object? parameter)
        {
            // Proceed to LafLoader Page as requested
            _navigationService?.Navigate(new LafLoaderView(new LafLoaderViewModel(_navigationService!)));
        }

        private void OnCancel(object? parameter)
        {
            if (_navigationService != null && _navigationService.CanGoBack)
            {
                _navigationService.GoBack();
            }
        }
    }
}
