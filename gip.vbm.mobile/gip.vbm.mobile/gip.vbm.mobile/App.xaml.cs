using System;
using gip.vbm.mobile.Services;
using gip.vbm.mobile.Views;
using gip.core.webservices;
using gip.vbm.mobile.ViewModels;
using gip.vbm.mobile.Helpers;
using gip.core.datamodel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using gip.vbm.mobile.Controls;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace gip.vbm.mobile
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

        private static App _Root;
        public static App Root
        {
            get
            {
                return _Root;
            }
        }

        public App()
        {
            _Root = this;
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
            //MainPage = new AboutPage();
            //MainPage = new AppShell();
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

        public async Task<object?> OpenBarcodeCamera()
        {
            return await (MainPage as MainPage).OpenBarcodeCamera();
        }

        public static SettingsViewModel SettingsViewModel
        {
            get
            {
                return SettingsViewModel.StaticSettingModel;
            }
        }

        private static PerformanceLogger _PerfLogger = null;// new PerformanceLogger("Code-Analysis");
        public static PerformanceLogger PerfLogger
        {
            get
            {
                if (_PerfLogger == null)
                {
                    _PerfLogger = new PerformanceLogger("Code-Analysis");
                    _PerfLogger.Active = SettingsViewModel.PerfLoggingOn;
                }
                return _PerfLogger;
            }
        }
    }
}
