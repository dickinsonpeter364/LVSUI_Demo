using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfMvvmApp.Views
{
    public partial class PreInspectionWindow : Window
    {
        private bool _hasChanged;

        public double Speed
        {
            get => (double)GetValue(SpeedProperty);
            set => SetValue(SpeedProperty, value);
        }
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register(nameof(Speed), typeof(double), typeof(PreInspectionWindow), new PropertyMetadata(80.0));

        public string RewindReelSize { get; private set; } = "Small";
        public string WindReelSize { get; private set; } = "Small";
        public bool Accepted { get; private set; }

        public PreInspectionWindow()
        {
            InitializeComponent();
            UpdateCoreButtonStyles();
        }

        private void MarkChanged()
        {
            _hasChanged = true;
            BtnOk.IsEnabled = true;
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
                MarkChanged();
        }

        private void OnRewindSmallClick(object sender, RoutedEventArgs e)
        {
            RewindReelSize = "Small";
            UpdateCoreButtonStyles();
            MarkChanged();
        }

        private void OnRewindLargeClick(object sender, RoutedEventArgs e)
        {
            RewindReelSize = "Large";
            UpdateCoreButtonStyles();
            MarkChanged();
        }

        private void OnWindSmallClick(object sender, RoutedEventArgs e)
        {
            WindReelSize = "Small";
            UpdateCoreButtonStyles();
            MarkChanged();
        }

        private void OnWindLargeClick(object sender, RoutedEventArgs e)
        {
            WindReelSize = "Large";
            UpdateCoreButtonStyles();
            MarkChanged();
        }

        private void UpdateCoreButtonStyles()
        {
            if (RewindSmall == null) return; // not yet initialized

            var selectedBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007ACC"));
            var defaultBg = new SolidColorBrush(Colors.White);
            var selectedFg = new SolidColorBrush(Colors.White);
            var defaultFg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));

            RewindSmall.Background = RewindReelSize == "Small" ? selectedBg : defaultBg;
            RewindSmall.Foreground = RewindReelSize == "Small" ? selectedFg : defaultFg;
            RewindLarge.Background = RewindReelSize == "Large" ? selectedBg : defaultBg;
            RewindLarge.Foreground = RewindReelSize == "Large" ? selectedFg : defaultFg;

            WindSmall.Background = WindReelSize == "Small" ? selectedBg : defaultBg;
            WindSmall.Foreground = WindReelSize == "Small" ? selectedFg : defaultFg;
            WindLarge.Background = WindReelSize == "Large" ? selectedBg : defaultBg;
            WindLarge.Foreground = WindReelSize == "Large" ? selectedFg : defaultFg;
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            Accepted = true;
            DialogResult = true;
        }
    }
}
