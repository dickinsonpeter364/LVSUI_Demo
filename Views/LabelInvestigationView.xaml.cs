using System.Windows.Controls;
using WpfMvvmApp.ViewModels;

namespace WpfMvvmApp.Views
{
    public partial class LabelInvestigationView : Page
    {
        public LabelInvestigationView(LabelInvestigationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
