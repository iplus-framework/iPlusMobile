// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using gip.vb.mobile.ViewModels;

namespace gip.vb.mobile.Views
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

            lblCurrentUser.Text = App.UserRights.UserName;
        }

        protected override bool OnBackButtonPressed()
        {
            ExitOnBackButtonPressed();
            return true;
        }

        private void SendPerfLog_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SendPerfLog.Execute(null);
        }
    }
}