using System.Windows.Controls;
using WpfMvvmApp.ViewModels;

namespace WpfMvvmApp.Views
{
    public partial class SettingsView : Page
    {
        public SettingsView(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public SettingsView()
        {
            InitializeComponent();
        }
    }
}
