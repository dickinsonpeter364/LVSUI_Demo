using System.Windows.Input;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;
using WpfMvvmApp.Views;

namespace WpfMvvmApp.ViewModels
{
    public class QcReviewViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly int _lafCount;
        private readonly InspectionViewModel _inspectionViewModel;

        public int LafCount => _lafCount;
        public bool ShowLaf2 => _lafCount == 2;

        public ICommand AcceptCommand { get; }
        public ICommand RejectCommand { get; }

        public QcReviewViewModel(INavigationService navigationService, int lafCount, InspectionViewModel inspectionViewModel)
        {
            _navigationService = navigationService;
            _lafCount = lafCount;
            _inspectionViewModel = inspectionViewModel;

            AcceptCommand = new RelayCommand(OnAccept);
            RejectCommand = new RelayCommand(OnReject);
        }

        private void OnAccept(object? parameter)
        {
            _navigationService.Navigate(new AuthoriseView(new AuthoriseViewModel(_navigationService, () =>
            {
                // On success, start inspection and return to InspectionView
                _inspectionViewModel.CompleteStartInspection();
                _navigationService.Navigate(new InspectionView(_inspectionViewModel));
            }, "Authorise QC Review", string.Empty, false)));
        }

        private void OnReject(object? parameter)
        {
            // Just return to InspectionView without starting
            _navigationService.Navigate(new InspectionView(_inspectionViewModel));
        }
    }
}
