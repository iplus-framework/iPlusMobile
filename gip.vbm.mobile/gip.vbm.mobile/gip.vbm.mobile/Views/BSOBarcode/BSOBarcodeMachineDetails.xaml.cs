// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.webservices;
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
    public partial class BSOBarcodeMachineDetails : BSOPageBase
    {
        #region c'tors

        public BSOBarcodeMachineDetails(BarcodeScanManuModel viewModel)
        {
            _ViewModel = viewModel;
            InitializeComponent();
            BindingContext = _ViewModel;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await _ViewModel.ExecuteLoadOEEReasons();
        }

        #endregion

        #region Properties

        BarcodeScanManuModel _ViewModel;

        #endregion

        #region Methods

        private async void BtnSwitchToRunning_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.InvokeMachineMalfuction(true);
        }

        private async void BtnSwitchToMalfunction_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.InvokeMachineMalfuction(false);
        }

        #endregion
    }
}