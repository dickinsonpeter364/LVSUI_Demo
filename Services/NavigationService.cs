using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfMvvmApp.Services
{
    public class NavigationService : INavigationService
    {
        private Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }

        public void Navigate(object page)
        {
            _frame.Navigate(page);
        }

        public void GoBack()
        {
            if (_frame.CanGoBack)
            {
                _frame.GoBack();
            }
        }

        public bool CanGoBack => _frame.CanGoBack;
    }
}
