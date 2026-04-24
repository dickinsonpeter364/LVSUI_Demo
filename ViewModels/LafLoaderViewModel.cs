using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
        private BitmapImage? _laf1Preview;
        private BitmapImage? _laf2Preview;
        private bool _isProcessing;

        public string Laf1Path
        {
            get => _laf1Path;
            set
            {
                if (SetProperty(ref _laf1Path, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                    if (!string.IsNullOrWhiteSpace(value))
                        _ = ProcessLaf1Async(value);
                }
            }
        }

        /// <summary>Annotated preview image shown after L1 is selected.</summary>
        public BitmapImage? Laf1Preview
        {
            get => _laf1Preview;
            private set => SetProperty(ref _laf1Preview, value);
        }

        /// <summary>True while the PDF is being rendered and mapped.</summary>
        public bool IsProcessing
        {
            get => _isProcessing;
            private set => SetProperty(ref _isProcessing, value);
        }

        /// <summary>Annotated preview image shown after L2 is selected.</summary>
        public BitmapImage? Laf2Preview
        {
            get => _laf2Preview;
            private set => SetProperty(ref _laf2Preview, value);
        }

        public string Laf2Path
        {
            get => _laf2Path;
            set
            {
                if (SetProperty(ref _laf2Path, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                    if (App.LafCaptureTest && !string.IsNullOrWhiteSpace(value))
                        _ = ProcessLaf2Async(value);
                }
            }
        }

        public ICommand SelectLaf1Command { get; }
        public ICommand SelectLaf2Command { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public LafLoaderViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _laf1Path = string.Empty;
            _laf2Path = string.Empty;
            SelectLaf1Command = new RelayCommand(o => SelectLaf(1));
            SelectLaf2Command = new RelayCommand(o => SelectLaf(2));
            OkCommand = new RelayCommand(OnOk, CanExecuteOk);
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

        private bool CanExecuteOk(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Laf1Path);
        }

        private void OnOk(object? parameter)
        {
            int lafCount = string.IsNullOrWhiteSpace(Laf2Path) ? 1 : 2;
            // Navigate to Inspection Page with the selected LAF count
            _navigationService.Navigate(new InspectionView(new InspectionViewModel(_navigationService, lafCount)));
        }

        private async Task ProcessLaf1Async(string l1Path)
        {
            IsProcessing = true;
            Laf1Preview  = null;
            try
            {
                if (App.LafCaptureTest)
                {
                    // L1 is always clipped by largest rectangle, and runs CreateAbsoluteMap
                    // + draws element boxes on top.
                    Laf1Preview = await Task.Run(() =>
                        LabelMatcher.CaptureClippedLaf(l1Path, ClipMode.LargestRectangle));
                }
                else
                {
                    string l2 = Laf2Path ?? "";
                    Laf1Preview = await Task.Run(() => LabelMatcher.ProcessLaf1(l1Path, l2));
                }
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task ProcessLaf2Async(string l2Path)
        {
            // L2 is always clipped by trim lines — clip-only preview, no element annotations.
            Laf2Preview = null;
            Laf2Preview = await Task.Run(() =>
                LabelMatcher.CaptureClippedLaf(l2Path, ClipMode.TrimLines));
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
