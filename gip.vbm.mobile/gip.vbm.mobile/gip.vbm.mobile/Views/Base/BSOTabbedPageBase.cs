// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using gip.vbm.mobile.DependencyServices;
using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    public class BSOTabbedPageBase : TabbedPage, IBSOPage
    {
        public BSOTabbedPageBase() : base()
        {
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "Load page exception");
            }
        }

        private async void OnCurrentPageChanged(object sender, EventArgs e)
        {
            try
            {
                var activePage = CurrentPage;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "Tab change exception");
            }
        }

        private async Task HandleExceptionAsync(Exception ex, string title)
        {
            App.SettingsViewModel.LastCrashDump = title + "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
        }

        public NavParameter NavParam
        {
            get; set;
        }

        public virtual void OnNavigatedTo(NavigationEventArgs e, NavigationMode navigationMode)
        {
        }

        public virtual void OnNavigatedFrom(NavigationEventArgs e, NavigationMode navigationMode)
        {
        }


        public static readonly BindableProperty PageStateProperty = BindableProperty.Create("PageState", typeof(PageStateEnum), typeof(BSOTabbedPageBase), PageStateEnum.View);
        public PageStateEnum PageState
        {
            get
            {
                return (PageStateEnum)GetValue(PageStateProperty);
            }
            set
            {
                SetValue(PageStateProperty, value);
                OnPageStateChanged();
            }
        }

        protected virtual void OnPageStateChanged()
        {
        }

        private DateTime _LastExitTap;
        private TimeSpan _ExitTapTimeout = TimeSpan.FromSeconds(2.5);

        protected virtual void ExitOnBackButtonPressed()
        {
            DateTime tap = DateTime.Now;
            TimeSpan elapsedTime = tap - _LastExitTap;
            _LastExitTap = tap;
            IUtility utilityService = DependencyService.Get<IUtility>();
            if (utilityService != null)
            {
                if (elapsedTime > _ExitTapTimeout)
                {
                    utilityService.ShortAlert(Strings.AppStrings.ExitHint_Text);
                }
                else
                {
                    utilityService.CloseApp();
                }
            }
        }
    }
}
