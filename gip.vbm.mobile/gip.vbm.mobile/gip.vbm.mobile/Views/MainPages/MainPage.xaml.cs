// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.webservices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using gip.vbm.mobile.Controls;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;

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

            // TabbedPage instances must never be reused across NavigationPage switches on Android.
            // When Detail is replaced, the old ScopedFragment and its child tab-fragments are destroyed.
            // Reusing the same TabbedPage C# object in a new NavigationPage causes:
            // "Fragment ScopedFragment has already been destroyed. Nested fragments should always use the child FragmentManager."
            bool isTabbedPage = typeof(TabbedPage).IsAssignableFrom(pageType);

            Page bso = null;
            if (isTabbedPage || !MenuPages.ContainsKey(pageClassName))
            {
                bso = (Page)Activator.CreateInstance(pageType);
                MenuPages[pageClassName] = bso; // overwrite stale entry for TabbedPage, add for new
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

        public async Task<object> OpenBarcodeCamera()
        {
#if ZEBRA
            // Zebra devices have a hardware barcode scanner. The ZXing camera popup is
            // unreliable here: CameraX HasCamera() can report true while the camera is
            // actually busy/disabled, and ZXing.Net.Maui 0.7.4 then throws an uncatchable
            // IllegalArgumentException ("No available camera can be found") inside a Java
            // Runnable, crashing the app. So never open the camera popup on ZEBRA builds.
            await DisplayAlertAsync("Camera", "No available camera on this device. Use the hardware barcode scanner instead.", "OK");
            return null;
#elif ANDROID
            // ZXing.Net.Maui 0.7.4 throws IllegalArgumentException "No available camera can be found"
            // (inside a Java Runnable -> uncatchable crash) when no back camera is available.
            if (!await HasBackCameraAvailableAsync())
            {
                await DisplayAlertAsync("Camera", "No available camera on this device. Use the hardware barcode scanner instead.", "OK");
                return null;
            }
#endif
            var popup = new BarcodePopup();
            await this.ShowPopupAsync(popup);
            return popup.Results;
        }

#if ANDROID
        private static async Task<bool> HasBackCameraAvailableAsync()
        {
            try
            {
                var future = await Task.Run(() =>
                    AndroidX.Camera.Lifecycle.ProcessCameraProvider.GetInstance(Platform.AppContext));
                var cameraProvider = future?.Get() as AndroidX.Camera.Lifecycle.ProcessCameraProvider;
                if (cameraProvider == null)
                    return false;
                return cameraProvider.HasCamera(AndroidX.Camera.Core.CameraSelector.DefaultBackCamera);
            }
            catch
            {
                return false;
            }
        }
#endif
    }
}