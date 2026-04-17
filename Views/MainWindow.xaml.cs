using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using WpfMvvmApp.Services;
using WpfMvvmApp.ViewModels;

namespace WpfMvvmApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _inactivityTimer;
        private readonly INavigationService _navigationService;

        public MainWindow()
        {
            InitializeComponent();

            // Create NavigationService (needs the Frame from XAML)
            _navigationService = new NavigationService(MainFrame);

            // Register NavigationService in DI for other components to use
            var mainViewModel = new MainViewModel(_navigationService);
            DataContext = mainViewModel;

            _inactivityTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };

            if (App.IsDummyMode)
            {
                // Dummy mode: skip login, go straight to InspectionView for testing
                _navigationService.Navigate(new InspectionView(new InspectionViewModel(_navigationService)));
            }
            else
            {
                // Production: inactivity timer and full login flow
                _inactivityTimer.Tick += InactivityTimer_Tick;
                _inactivityTimer.Start();

                this.PreviewMouseMove += (s, e) => ResetTimer();
                this.PreviewKeyDown += (s, e) => ResetTimer();

                MainFrame.Navigated += (s, e) =>
                {
                    if (DataContext is MainViewModel mainVm)
                    {
                        mainVm.IsLogOffVisible = !(MainFrame.Content is AuthoriseView);
                    }
                };

                _navigationService.Navigate(new AuthoriseView(new AuthoriseViewModel(_navigationService, () =>
                {
                    _navigationService.Navigate(new DeviceControlView(new DeviceControlViewModel(_navigationService, DeviceControlMode.PostLogin, () =>
                    {
                        _navigationService.Navigate(new LpnEntryView(new LpnEntryViewModel(_navigationService)));
                    })));
                }, title: "Log On")));
            }
        }

        private void ResetTimer()
        {
            _inactivityTimer.Stop();
            _inactivityTimer.Start();
        }

        private void InactivityTimer_Tick(object? sender, EventArgs e)
        {
            // Check current status
            var content = MainFrame.Content as FrameworkElement;
            if (content == null) return;

            // If already on AuthoriseView, do nothing
            if (content is AuthoriseView) return;

            // Check if system is in inspection mode using global flag
            if (!App.IsInspecting)
            {
                // Navigate to Unlock
                _navigationService.Navigate(new AuthoriseView(new AuthoriseViewModel(_navigationService, () =>
                {
                    // On success, go back to where we were
                    if (_navigationService.CanGoBack) _navigationService.GoBack();
                }, title: "Unlock")));
            }
        }
    }
}
