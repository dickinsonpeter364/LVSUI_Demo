using System.Windows.Controls;
using WpfMvvmApp.ViewModels;

namespace WpfMvvmApp.Views
{
    public partial class QcReviewView : Page
    {
        public QcReviewView(QcReviewViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
