using gip.core.autocomponent;
using gip.core.webservices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
            MasterBehavior = MasterBehavior.Popover;
            MenuPages.Add(typeof(AboutPage).Name, (NavigationPage)Detail);
        }

        #region Properties
        public VBUserRights UserRights
        {
            get
            {
                return App.UserRights;
            }
        }

        Dictionary<string, NavigationPage> MenuPages = new Dictionary<string, NavigationPage>();
        #endregion

        public async Task NavigateFromMenu(VBMenuItem menuItem)
        {
            await StartNewPage(menuItem.DestPage, menuItem.PageClassName, null);
        }

        public async Task StartNewPage(Type pageType, string pageClassName, NavParameter navParam = null)
        {
            if (String.IsNullOrEmpty(pageClassName))
                pageClassName = pageType.Name;
            if (!MenuPages.ContainsKey(pageClassName))
            {
                MenuPages.Add(pageClassName, new NavigationPage((Page)Activator.CreateInstance(pageType)));
            }

            var newPage = MenuPages[pageClassName];

            if (newPage != null && Detail != newPage)
            {
                NavigationPage currentPage = Detail as NavigationPage;
                if (currentPage != null)
                {
                    currentPage.Pushed -= NavPage_Pushed;
                    currentPage.Popped -= NavPage_Popped;
                    currentPage.PoppedToRoot -= NavPage_PoppedToRoot;
                }

                IBSOPage iPage = newPage.CurrentPage as IBSOPage;
                if (iPage != null && navParam != null)
                    iPage.NavParam = navParam;
                newPage.Pushed += NavPage_Pushed;
                newPage.Popped += NavPage_Popped;
                newPage.PoppedToRoot += NavPage_PoppedToRoot;
                this.NavigationMode = NavigationMode.New;

                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
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

    }
}