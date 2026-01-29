using System.Windows.Input;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;

namespace WpfMvvmApp.ViewModels
{
    public class LabelInvestigationViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private bool _isPatchyPrint;
        private bool _isMarkOnLabel;
        private bool _isRibbonWrinkle;
        private bool _isBarcodeScannedManually;
        private bool _isTextMovement;
        private bool _isOther;
        private string _description;

        public bool IsPatchyPrint
        {
            get => _isPatchyPrint;
            set
            {
                if (SetProperty(ref _isPatchyPrint, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsMarkOnLabel
        {
            get => _isMarkOnLabel;
            set
            {
                if (SetProperty(ref _isMarkOnLabel, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsRibbonWrinkle
        {
            get => _isRibbonWrinkle;
            set
            {
                if (SetProperty(ref _isRibbonWrinkle, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsBarcodeScannedManually
        {
            get => _isBarcodeScannedManually;
            set
            {
                if (SetProperty(ref _isBarcodeScannedManually, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsTextMovement
        {
            get => _isTextMovement;
            set
            {
                if (SetProperty(ref _isTextMovement, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsOther
        {
            get => _isOther;
            set
            {
                if (SetProperty(ref _isOther, value))
                {
                    OnPropertyChanged(nameof(DescriptionLabel));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (SetProperty(ref _description, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public string DescriptionLabel => IsOther ? "Description - Mandatory" : "Description";

        private string _labelBackingNo;
        public string LabelBackingNo
        {
            get => _labelBackingNo;
            set
            {
                if (SetProperty(ref _labelBackingNo, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand AcceptCommand { get; }
        public ICommand RejectCommand { get; }
        public ICommand MissingCommand { get; }

        public LabelInvestigationViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _description = string.Empty;
            _labelBackingNo = string.Empty;
            AcceptCommand = new RelayCommand(OnAccept, CanAccept);
            RejectCommand = new RelayCommand(OnReject, CanReject);
            MissingCommand = new RelayCommand(OnMissing);
        }

        private bool IsLabelBackingNoValid()
        {
            if (string.IsNullOrWhiteSpace(LabelBackingNo))
                return false;
            
            // Check if all characters are digits
            return LabelBackingNo.All(char.IsDigit);
        }

        private bool CanAccept(object? parameter)
        {
            return IsLabelBackingNoValid();
        }

        private void OnAccept(object? parameter)
        {
            if (_navigationService.CanGoBack)
                _navigationService.GoBack();
        }

        private void OnMissing(object? parameter)
        {
            if (_navigationService.CanGoBack)
                _navigationService.GoBack();
        }


        private void OnReject(object? parameter)
        {
            if (_navigationService.CanGoBack)
                _navigationService.GoBack();
        }

        private bool CanReject(object? parameter)
        {
            // Check Label Backing No is valid
            if (!IsLabelBackingNoValid())
                return false;

            bool anyChecked = IsPatchyPrint || IsMarkOnLabel || IsRibbonWrinkle || 
                              IsBarcodeScannedManually || IsTextMovement || IsOther;

            if (IsOther && string.IsNullOrWhiteSpace(Description))
            {
                return false;
            }

            return anyChecked;
        }
    }
}
