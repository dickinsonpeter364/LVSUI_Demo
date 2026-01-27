using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
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
            
            // Composition Root (Simplified)
            _navigationService = new NavigationService(MainFrame);
            MainViewModel mainViewModel = new MainViewModel(_navigationService);
            DataContext = mainViewModel;

            // Inactivity Timer (1 minute)
            _inactivityTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _inactivityTimer.Tick += InactivityTimer_Tick;
            _inactivityTimer.Start();

            // Hook input events to reset timer
            this.PreviewMouseMove += (s, e) => ResetTimer();
            this.PreviewKeyDown += (s, e) => ResetTimer();

            // Navigate to Login/Authorise
            _navigationService.Navigate(new AuthoriseView(new AuthoriseViewModel(_navigationService, title: "Log On")));
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