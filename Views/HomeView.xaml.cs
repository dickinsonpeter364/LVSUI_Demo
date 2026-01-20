using System.Windows.Controls;
using WpfMvvmApp.ViewModels;

namespace WpfMvvmApp.Views
{
    public partial class HomeView : Page
    {
        public HomeView(HomeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public HomeView()
        {
            InitializeComponent();
        }
    }
}
