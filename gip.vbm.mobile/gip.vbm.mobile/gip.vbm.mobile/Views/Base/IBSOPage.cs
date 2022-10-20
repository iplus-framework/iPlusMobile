using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
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
