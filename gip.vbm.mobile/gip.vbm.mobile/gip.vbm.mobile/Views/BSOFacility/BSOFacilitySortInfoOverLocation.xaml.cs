// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.vbm.mobile.Controls;
using gip.vbm.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOFacilitySortInfoOverLocation : BSOPageBase
    {
        public BSOFacilitySortInfoOverLocation()
        {
            BindingContext = _ViewModel = new FacilitySortInfoOverLocationModel();
            InitializeComponent();
        }

        private FacilitySortInfoOverLocationModel _ViewModel;

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await _ViewModel.ExecuteLoadStorageLocations();
        }

        private async void btnApply_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SaveSortStorageLocations();
            await Navigation.PopAsync();
        }
    }
}