namespace WpfMvvmApp.Services
{
    public interface INavigationService
    {
        void Navigate(object page);
        void GoBack();
        bool CanGoBack { get; }
    }
}
