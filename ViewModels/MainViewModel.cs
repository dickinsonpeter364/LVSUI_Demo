using WpfMvvmApp.Core;
using WpfMvvmApp.Services;

namespace WpfMvvmApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}
