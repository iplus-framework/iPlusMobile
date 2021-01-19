using Xamarin.Forms;

namespace gip.vb.mobile.Views
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
