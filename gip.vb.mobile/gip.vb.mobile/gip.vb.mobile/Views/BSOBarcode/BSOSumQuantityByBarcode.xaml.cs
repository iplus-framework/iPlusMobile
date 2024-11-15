// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.vb.mobile.ViewModels;
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
    public partial class BSOSumQuantityByBarcode : BSOPageBase
    {
        SumQuantityByBarcodeViewModel _ViewModel;

        public BSOSumQuantityByBarcode(SumQuantityByBarcodeViewModel viewModel)
        {
            BindingContext = _ViewModel = viewModel;
            InitializeComponent();
            //BtnApply_Text
        }

        private bool _ClosedFromOKButton = false;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            barcodeScanner.OnAppearing();
            _ViewModel.PropertyChanged += _ViewModel_PropertyChanged;
        }

        private void _ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BaseViewModel.Message))
            {
                barcodeScanner._ViewModel.Message = _ViewModel.Message;
            }
        }

        protected override void OnDisappearing()
        {
            if (!_ClosedFromOKButton)
            {
                _ViewModel.SumQuantity = 0.0;
            }

            _ViewModel.PropertyChanged -= _ViewModel_PropertyChanged;
            barcodeScanner.OnDisappearing();
            base.OnDisappearing();
        }

        private async void barCodeScanner_OnBarcodeCommandInvoked(object sender, EventArgs e)
        {
            await _ViewModel.OnBarcodeScanned(barcodeScanner._ViewModel.CurrentBarcode);
        }

        private void TBItemClear_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel != null)
            {
                _ViewModel.Clear();
            }
        }

        private void SumEntry_Focused(object sender, FocusEventArgs e)
        {
            SumEntry.Unfocus();
        }

        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            #if ZXING
            barcodeScanner.OpenCameraPanel();
            #endif
        }

        private async void BtnOkMinus_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SumQuantity = _ViewModel.SumQuantity * -1;
            _ClosedFromOKButton = true;
            _ = await Navigation.PopAsync();
        }

        private async void BtnOkPlus_Clicked(object sender, EventArgs e)
        {
            _ClosedFromOKButton = true;
            _ = await Navigation.PopAsync();
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            SumQuantityByBarcodeViewModel.SumItem? sumItem = e.Item as SumQuantityByBarcodeViewModel.SumItem?;
            if (sumItem != null)
            {
                _ViewModel.CurrentBarcodeInfo = sumItem;
                _ClosedFromOKButton = true;
                _ = await Navigation.PopAsync();
            }
        }
    }
}