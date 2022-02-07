using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : BSOPageBase
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            DisplayVersionInfo();
            base.OnAppearing();
        }

        public void DisplayVersionInfo()
        {
            lblVersion.Text = VersionTracking.CurrentVersion;
            lblBuild.Text = VersionTracking.CurrentBuild;

            lblCurrentUser.Text = App.UserRights.UserName;
        }

        protected override bool OnBackButtonPressed()
        {
            ExitOnBackButtonPressed();
            return true;
        }
    }
}