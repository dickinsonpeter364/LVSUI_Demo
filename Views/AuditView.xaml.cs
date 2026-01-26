using System.Windows.Controls;
using WpfMvvmApp.ViewModels;

namespace WpfMvvmApp.Views
{
    public partial class AuditView : Page
    {
        public AuditView(AuditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        // Default constructor for XAML designer (optional but good practice)
        public AuditView()
        {
            InitializeComponent();
        }
    }
}
