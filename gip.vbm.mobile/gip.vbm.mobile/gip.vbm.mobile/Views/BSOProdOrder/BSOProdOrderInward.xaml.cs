// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.webservices;
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

namespace gip.vbm.mobile.Views
{
    /// <summary>
    /// Inward-Booking (Output of Production; Ergebnis)
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOProdOrderInward : BSOProdOrderInOutBase
    {
        BarcodeScanManuModel _FromTaskModel;
        private MaterialUnitCalcModel _UnitCalcModel;

        protected override BarcodeScannerView BarcodeScanner
        {
            get
            {
                return barcodeScanner;
            }
        }

        public BSOProdOrderInward(ProdOrderPartslistPos intermOrIntermBatch, BarcodeScanManuModel taskModel, ACMethod wfMethod)
        {
            _FromTaskModel = taskModel;
            BindingContext = _ViewModel = new ProdOrderInOutViewModel(true, taskModel, null, intermOrIntermBatch, wfMethod);
            InitializeComponent();
        }

        public void ButtonDoBooking_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.CurrentFacility == null)
            {
                _ViewModel.Message = new core.datamodel.Msg() { Message = Strings.AppStrings.SelectTargetFacilityLocationOrBin_Text };
                return;
            }
            if (_FromTaskModel != null && _FromTaskModel.ScannedMachine != null)
                _ViewModel.PropertyACUrl = ACUrlHelper.GetParentACUrl(_FromTaskModel.ScannedMachine.ACClass.ACUrlComponent);
            _ViewModel.BookFacilityCommand.Execute(null);
        }

        private void TabPrint_Clicked(object sender, EventArgs e)
        {
            if (CurrentPage == ProdOrderInOutHistory)
            {
                _ViewModel.Print(Strings.AppStrings.PrintCopies_Question);
            }
        }

        private void BSOTabbedPageBase_CurrentPageChanged(object sender, EventArgs e)
        {
            if (CurrentPage == ProdOrderInOutHistory)
            {
                TBPrint.IsEnabled = true;
            }
            else
            {
                TBPrint.IsEnabled = false;
            }
        }

#if ZXING
        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
#else
        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
#endif
        {
#if ZXING
            await BarcodeScanner.OpenBarcodeCamera();
#endif
        }

        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.RefreshInItems();
        }

        protected async override void OnAppearing()
        {
            if (_UnitCalcModel != null)
            {
                _UnitCalcModel = null;
                return;
            }

            await _ViewModel.LoadTargetFacilities();
            await _ViewModel.RefreshInItems();
            _ViewModel.LoadMovementReasons();
        }

        private void cmdClearMovementReason_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedMovementReason = null;
        }

        private async void btnUnit_Clicked(object sender, EventArgs e)
        {
            if (Math.Abs(_ViewModel.BookingQuantity) <= double.Epsilon)
                return;
            _UnitCalcModel = new MaterialUnitCalcModel(_ViewModel);
            _UnitCalcModel.MaterialToCalc = _ViewModel.IntermOrIntermBatch.BookingMaterial;
            if (_UnitCalcModel.MaterialToCalc == null)
                _UnitCalcModel.MaterialToCalc = _ViewModel.IntermOrIntermBatch.ProdOrderPartslist?.Partslist?.Material;
            if (_UnitCalcModel.MaterialToCalc == null)
                return;
            _UnitCalcModel.InputValue = _ViewModel.BookingQuantity;
            await Navigation.PushAsync(new BSOMaterialUnitCalc(_UnitCalcModel));
        }
    }
}