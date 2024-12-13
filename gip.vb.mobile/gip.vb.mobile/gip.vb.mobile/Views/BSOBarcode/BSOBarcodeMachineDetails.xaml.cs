// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.webservices;
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
    public partial class BSOBarcodeMachineDetails : BSOPageBase
    {
        BarcodeScanManuModel _ViewModel;

        public BSOBarcodeMachineDetails(BarcodeScanManuModel viewModel)
        {
            _ViewModel = viewModel;
            InitializeComponent();
            BindingContext = _ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }



        #region Properties


        #endregion

        #region Methods

        #endregion

        private async void BtnSwitchToRunning_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.InvokeMachineMalfuction(true);
        }

        private async void BtnSwitchToMalfunction_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.InvokeMachineMalfuction(false);
        }
    }
}