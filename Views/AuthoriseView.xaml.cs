using System.Windows.Controls;
using WpfMvvmApp.ViewModels;

namespace WpfMvvmApp.Views
{
    public partial class AuthoriseView : Page
    {
        public AuthoriseView(AuthoriseViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            this.Loaded += (s, e) => 
            {
                if (viewModel.IsUsernameReadOnly)
                {
                    UserPasswordBox.Focus();
                }
                else
                {
                    UsernameTextBox.Focus();
                }
            };
        }
    }
}
