using Microsoft.Win32;
using System.Windows.Input;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;
using WpfMvvmApp.Views;

namespace WpfMvvmApp.ViewModels
{
    public class LafLoaderViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private string _laf1Path;
        private string _laf2Path;

        public string Laf1Path
        {
            get => _laf1Path;
            set => SetProperty(ref _laf1Path, value);
        }

        public string Laf2Path
        {
            get => _laf2Path;
            set => SetProperty(ref _laf2Path, value);
        }

        public ICommand SelectLaf1Command { get; }
        public ICommand SelectLaf2Command { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public LafLoaderViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SelectLaf1Command = new RelayCommand(o => SelectLaf(1));
            SelectLaf2Command = new RelayCommand(o => SelectLaf(2));
            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private void SelectLaf(int index)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf|All files (*.*)|*.*",
                Title = $"Select LAF{index} File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (index == 1)
                    Laf1Path = openFileDialog.FileName;
                else
                    Laf2Path = openFileDialog.FileName;
            }
        }

        private void OnOk(object parameter)
        {
            // Navigate to Inspection Page
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
