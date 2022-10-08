using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    public class NavigationPageBSO : NavigationPage
    {
        public NavigationPageBSO()
            : base()
        {
        }

        public NavigationPageBSO(Page root)
            : base(root)
        {
        }

        protected override void OnAppearing()
        {
            if (this.CurrentPage != null)
            {
                SetHasNavigationBar(this.CurrentPage, false);
            }
            base.OnAppearing();
        }
    }
}
