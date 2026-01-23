using System.Windows.Controls;
using WpfMvvmApp.ViewModels;

namespace WpfMvvmApp.Views
{
    public partial class LafLoaderView : Page
    {
        public LafLoaderView(LafLoaderViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
