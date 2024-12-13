// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.webservices;
using gip.vbm.mobile.barcode;
using gip.vbm.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using gip.vbm.mobile.Controls;
using gip.core.webservices;

namespace gip.vbm.mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOBarcodeTasksManu : BSOPageBase
    {
        BarcodeScanManuModel _ViewModel;

        public BSOBarcodeTasksManu()
		{
            BindingContext = _ViewModel = new BarcodeScanManuModel();
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            barcodeScanner.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            barcodeScanner.OnDisappearing();
            base.OnDisappearing();
        }

        //private void barcodeScanner_OnBarcodeCommandInvoked(object sender, EventArgs e)
        //{
        //    EnableButtons();
        //}

#if ZXING
        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
#else
        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
#endif
        {
#if ZXING
            await barcodeScanner.OpenBarcodeCamera();
#endif
        }

        private async void barcodeScanner_OnBarcodeEntityTapped(object sender, BarcodeScannerEventArgs e)
        {
            ProdOrderPartslistWFInfo selectedWfInfo = _ViewModel.SelectedEntity as ProdOrderPartslistWFInfo;
            if (selectedWfInfo != null)
            {
                if (_ViewModel.ExchangedBarcodeSeq.State == mes.datamodel.BarcodeSequenceBase.ActionState.FastSelection)
                {
                    await _ViewModel.InvokeActionOnMachine();
                }
                else
                {
                    await Navigation.PushAsync(new BSOBarcodeTaskManuDetails(_ViewModel));
                }
            }
            else
            {
                ACClass machine = _ViewModel.SelectedEntity as ACClass;
                if (machine != null)
                {
                    await Navigation.PushAsync(new BSOBarcodeMachineDetails(_ViewModel));
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            ExitOnBackButtonPressed();
            return true;
        }

        private void SearchTBItem_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.ExchangedBarcodeSeq != null)
            {
                string searchText = _ViewModel.ExchangedBarcodeSeq.CurrentBarcode;
                _ViewModel.FilterSequenceList(searchText);
            }
        }
    }
}