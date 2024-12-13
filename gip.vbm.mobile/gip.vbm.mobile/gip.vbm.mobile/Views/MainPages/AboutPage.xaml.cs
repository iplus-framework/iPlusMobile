// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using gip.vbm.mobile.Controls;
using gip.vbm.mobile.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Views;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : BSOPageBase
    {
        static int _AppearingCounter = 0;

        internal AboutViewModel _ViewModel;
        public AboutPage()
        {
            BindingContext = _ViewModel = new AboutViewModel();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (_AppearingCounter <= 0)
                Settings.IsVisible = false;
            else
                Settings.IsVisible = true;
            _AppearingCounter++;
            DisplayVersionInfo();
            base.OnAppearing();
        }

        public void DisplayVersionInfo()
        {
            lblVersion.Text = VersionTracking.CurrentVersion;
            lblBuild.Text = VersionTracking.CurrentBuild;

            lblCurrentUser.Text = App.UserRights?.UserName;
        }

        protected override bool OnBackButtonPressed()
        {
            ExitOnBackButtonPressed();
            return true;
        }

        private void SendPerfLog_Clicked(object sender, EventArgs e)
        {
            DisplayPopup();
            _ViewModel.SendPerfLog.Execute(null);
        }

        public void DisplayPopup()
        {
            var popup = new BarcodePopup();

            this.ShowPopup(popup);
            //App._Root.DisplayPopup();
            //var result = await this.ShowPopupAsync(popup);

            //if (result is bool boolResult)
            //{
            //    if (boolResult)
            //    {
            //        // Yes was tapped
            //    }
            //    else
            //    {
            //        // No was tapped
            //    }
            //}
        }
    }
}