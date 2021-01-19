using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using gip.vb.mobile.Services;
using gip.vb.mobile.Views;
using gip.core.webservices;
using gip.vb.mobile.ViewModels;
using gip.vb.mobile.Helpers;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace gip.vb.mobile
{
    public partial class App : Application
    {
        public static VBUserRights UserRights { get; set; }
        public static bool IsUserLoggedIn
        {
            get
            {
                return UserRights != null;
            }
        }

        public App()
        {
            InitializeComponent();

            if (!IsUserLoggedIn)
            {
                MainPage = new LoginPage();
                //MainPage = new NavigationPage(new LoginPage());
                //MainPage = new NavigationPageBSO(new LoginPage());
            }
            else
            {
                //MainPage = new MainPage();
                //MainPage = new NavigationPage(new MainPage());
            }
        }

        public void DisplayMainPage()
        {
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }


        public static SettingsViewModel SettingsViewModel
        {
            get
            {
                return SettingsViewModel.StaticSettingModel;
            }
        }

    }
}
