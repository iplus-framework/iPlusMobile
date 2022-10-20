using gip.core.autocomponent;
using gip.core.webservices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using gip.vbm.mobile.Controls;
using CommunityToolkit.Maui.Views;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : FlyoutPage
    {
        public MainPage()
        {
            InitializeComponent();
            MenuPages.Add(typeof(AboutPage).Name, aboutPage);
        }

        #region Properties
        public VBUserRights UserRights
        {
            get
            {
                return App.UserRights;
            }
        }

        Dictionary<string, Page> MenuPages = new Dictionary<string, Page>();
        #endregion

        public async Task NavigateFromMenu(VBMenuItem menuItem)
        {
            await StartNewPage(menuItem.DestPage, menuItem.PageClassName, null);
        }

        public async Task StartNewPage(Type pageType, string pageClassName, NavParameter navParam = null)
        {
            if (String.IsNullOrEmpty(pageClassName))
                pageClassName = pageType.Name;
            Page bso = null;
            if (!MenuPages.ContainsKey(pageClassName))
            {
                bso = (Page)Activator.CreateInstance(pageType);
                MenuPages.Add(pageClassName, bso);
            }
            else
                bso = MenuPages[pageClassName];

            NavigationPage currentNavPage = Detail as NavigationPage;
            if (bso != null && (currentNavPage == null || currentNavPage.CurrentPage != bso))
            {
                if (currentNavPage != null)
                {
                    currentNavPage.Pushed -= NavPage_Pushed;
                    currentNavPage.Popped -= NavPage_Popped;
                    currentNavPage.PoppedToRoot -= NavPage_PoppedToRoot;
                }

                IBSOPage iPage = bso as IBSOPage;
                if (iPage != null && navParam != null)
                    iPage.NavParam = navParam;

                currentNavPage = new NavigationPage(bso);
                currentNavPage.Pushed += NavPage_Pushed;
                currentNavPage.Popped += NavPage_Popped;
                currentNavPage.PoppedToRoot += NavPage_PoppedToRoot;
                this.NavigationMode = NavigationMode.New;

                Detail = currentNavPage;

                //if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }

        public NavigationMode NavigationMode
        {
            get;
            set;
        }

        public void RefreshIcons()
        {
            masterPage.RefreshIcons();
        }

        private void NavPage_PoppedToRoot(object sender, NavigationEventArgs e)
        {
            this.NavigationMode = NavigationMode.Back;
            IBSOPage page = e.Page as IBSOPage;
            if (page != null)
                page.OnNavigatedFrom(e, this.NavigationMode);
            NavigationPage navPage = Detail as NavigationPage;
            if (navPage != null)
            {
                page = navPage.CurrentPage as IBSOPage;
                if (page != null)
                    page.OnNavigatedTo(e, this.NavigationMode);
            }
        }

        private void NavPage_Popped(object sender, NavigationEventArgs e)
        {
            this.NavigationMode = NavigationMode.Back;
            IBSOPage page = e.Page as IBSOPage;
            if (page != null)
                page.OnNavigatedFrom(e, this.NavigationMode);
            NavigationPage navPage = Detail as NavigationPage;
            if (navPage != null)
            {
                page = navPage.CurrentPage as IBSOPage;
                if (page != null)
                    page.OnNavigatedTo(e, this.NavigationMode);
            }
        }

        private void NavPage_Pushed(object sender, NavigationEventArgs e)
        {
            this.NavigationMode = NavigationMode.Forward;
            IBSOPage page = e.Page as IBSOPage;
            if (page != null)
                page.OnNavigatedTo(e, this.NavigationMode);
        }

        public async Task<object?> OpenBarcodeCamera()
        {
            var popup = new BarcodePopup();
            return await this.ShowPopupAsync(popup);
        }
    }
}