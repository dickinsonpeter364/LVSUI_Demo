using System.Windows.Input;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;

namespace WpfMvvmApp.ViewModels
{
    public class AuditViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ICommand GoBackCommand { get; }

        public AuditViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            GoBackCommand = new RelayCommand(OnGoBack);
        }

        private void OnGoBack(object? parameter)
        {
            if (_navigationService.CanGoBack)
            {
                _navigationService.GoBack();
            }
        }
    }
}
