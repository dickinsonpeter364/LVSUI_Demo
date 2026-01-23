using System.Windows.Input;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;
using WpfMvvmApp.Views;

namespace WpfMvvmApp.ViewModels
{
    public class LpnEntryViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private string _lpnNumber;

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

        private bool CanExecuteOk(object parameter)
        {
            return !string.IsNullOrWhiteSpace(LpnNumber);
        }

        private void OnOk(object parameter)
        {
            // Proceed to Inspection Page
            // In a real scenario, we might pass the LpnNumber to the InspectionViewModel
            _navigationService.Navigate(new InspectionView(new InspectionViewModel(_navigationService)));
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
