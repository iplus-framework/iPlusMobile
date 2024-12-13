// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.webservices;
using gip.mes.webservices;
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
    public partial class BSOPickingWorkplaceActivation : BSOPageBase
    {
        public BSOPickingWorkplaceActivation()
        {
            BindingContext = _ViewModel = new PickingWorkplaceActViewModel();
            Title = Strings.AppStrings.QuantActivationDeactivation_Text;
            InitializeComponent();
        }

        PickingWorkplaceActViewModel _ViewModel;

        protected override void OnAppearing()
        {
            barcodeScanner.ViewModel = _ViewModel.BarcodeScanModel;
            barcodeScanner.OnAppearing();
            base.OnAppearing();
            InitPageOnNavigation();
        }

        protected override void OnDisappearing()
        {
            barcodeScanner.OnDisappearing();
            base.OnDisappearing();
        }

        private void InitPageOnNavigation()
        {
            if (NavParam != null)
            {
                object[] navParams = NavParam.Arguments as object[];
                if (navParams != null)
                {
                    FacilityCharge fc = navParams.FirstOrDefault() as FacilityCharge;
                    _ViewModel.FacilityChargeItem = fc;

                    ACClass workplace = navParams.LastOrDefault() as ACClass;
                    _ViewModel.Workplace = workplace;
                }
            }
            this.PageState = PageStateEnum.View;
        }

        private void barcodeScanner_OnBarcodeCommandInvoked(object sender, EventArgs e)
        {
        }

#if ZXING
        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
#else
        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
#endif
        {
            //_ViewModel.Clear();
#if ZXING
            await barcodeScanner.OpenBarcodeCamera();
#endif
        }

    }
}