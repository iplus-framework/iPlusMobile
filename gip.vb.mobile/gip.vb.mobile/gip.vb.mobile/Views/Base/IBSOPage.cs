using Xamarin.Forms;

namespace gip.vb.mobile.Views
{
    public interface IBSOPage
    {
        NavParameter NavParam
        {
            get; set;
        }

        void OnNavigatedTo(NavigationEventArgs e, NavigationMode navigationMode);
        void OnNavigatedFrom(NavigationEventArgs e, NavigationMode navigationMode);
    }
}
