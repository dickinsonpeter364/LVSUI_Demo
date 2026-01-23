using System.Windows.Input;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;

namespace WpfMvvmApp.ViewModels
{
    public class LabelInvestigationViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private bool _isPatchPrint;
        private bool _isInvalidData;
        private bool _isOther;

        public bool IsPatchPrint
        {
            get => _isPatchPrint;
            set
            {
                if (SetProperty(ref _isPatchPrint, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsInvalidData
        {
            get => _isInvalidData;
            set
            {
                if (SetProperty(ref _isInvalidData, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsOther
        {
            get => _isOther;
            set
            {
                if (SetProperty(ref _isOther, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand AcceptCommand { get; }
        public ICommand RejectCommand { get; }

        public LabelInvestigationViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            AcceptCommand = new RelayCommand(OnAccept);
            RejectCommand = new RelayCommand(OnReject, CanReject);
        }

        private void OnAccept(object parameter)
        {
            if (_navigationService.CanGoBack)
                _navigationService.GoBack();
        }

        private void OnReject(object parameter)
        {
            // Here you would handle the rejection logic (e.g. logging the reason)
            if (_navigationService.CanGoBack)
                _navigationService.GoBack();
        }

        private bool CanReject(object parameter)
        {
            return IsPatchPrint || IsInvalidData || IsOther;
        }
    }
}
