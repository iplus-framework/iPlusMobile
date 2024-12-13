// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.webservices;
using gip.vbm.mobile.barcode;
using gip.vbm.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using gip.vbm.mobile.Controls;

namespace gip.vbm.mobile.Views
{
    public abstract class BSOProdOrderInOutBase : BSOTabbedPageBase
    {
        internal ProdOrderInOutViewModel _ViewModel;

        public BSOProdOrderInOutBase() : base()
        {

        }

        //public BSOProdOrderInOutBase(ProdOrderPartslistPosRelation rel)
        //{
        //    BindingContext = _ViewModel = new ProdOrderInOutViewModel(true, rel);
        //}

        protected abstract BarcodeScannerView BarcodeScanner { get; }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            BarcodeScanner.OnAppearing();
            await _ViewModel.ExecuteReadPostingsCommand();
        }

        protected override void OnDisappearing()
        {
            BarcodeScanner.OnDisappearing();
            base.OnDisappearing();
        }
    }
}
