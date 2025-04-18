// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.webservices;
using gip.mes.webservices;
using gip.vb.mobile.barcode;
using gip.vb.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
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
            barcodeScanner._ViewModel = _ViewModel;
            barcodeScanner.OnAppearing();
            //EnableButtons();
            //SendChangedACMethod();
            //if (_ViewModel != null)
            //    _ViewModel.ResetScanSequence();
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

        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            _ViewModel.Clear();
            #if ZXING
            barcodeScanner.OpenCameraPanel();
            #endif
        }



        private async void barcodeScanner_OnSelectBarcodeEntity(object sender, EventArgs e)
        {
            ProdOrderPartslistWFInfo selectedWfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (selectedWfInfo != null)
            {
                if (_ViewModel.Item.State == mes.datamodel.BarcodeSequenceBase.ActionState.FastSelection)
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
                ACClass machine = _ViewModel.SelectedSequence as ACClass;
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
            if (_ViewModel.Item != null)
            {
                _ViewModel.SearchSequenceList();


                //string searchText = _ViewModel.Item.CurrentBarcode;
                //_ViewModel.FilterSequenceList(searchText);
            }
        }
    }
}