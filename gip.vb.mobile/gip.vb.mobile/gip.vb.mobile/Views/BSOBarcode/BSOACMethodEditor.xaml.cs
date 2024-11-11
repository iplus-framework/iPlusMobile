// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.vb.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOACMethodEditor : BSOTabbedPageBase
    {
        ACMethod _ViewModel;

        public BSOACMethodEditor(ACMethod viewModel)
        {
            BindingContext = _ViewModel = viewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private async void BtnApply_Clicked(object sender, EventArgs e)
        {
            _ViewModel.AutoRemove = true;
            _ = await Navigation.PopAsync();
        }
    }
}