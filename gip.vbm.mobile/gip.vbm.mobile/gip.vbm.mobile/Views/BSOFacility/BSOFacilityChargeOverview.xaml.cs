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
using gip.vbm.mobile.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOFacilityChargeOverview : BSOTabbedPageBase
    {
        #region c'tors

        public BSOFacilityChargeOverview()
		{
            BindingContext = _ViewModel = new FacilityChargesViewModel();
            InitializeComponent();
        }

        #endregion

        #region Properties
        FacilityChargesViewModel _ViewModel;
        #endregion

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();
            barcodeScanner.OnAppearing();
            InitPageOnNavigation();
        }

        protected override void OnDisappearing()
        {
            barcodeScanner.OnDisappearing();
            base.OnDisappearing();
        }

        private void InitPageOnNavigation()
        {
            _ViewModel.OnAppear(NavParam?.Arguments as FacilityCharge);
            this.PageState = PageStateEnum.View;
        }

        private async void TBItemHistory_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOFacilityBookingHistory() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.FacilityChargeItem } });
        }

        private void ButtonDoBooking_Clicked(object sender, EventArgs e)
        {
            if (sender == ButtonDoBooking)
            {
                _ViewModel.BookFacilityCommand.Execute(null);
            }
            else if (sender == ButtonDoZeroStock)
            {
                _ViewModel.BookZeroStockCommand.Execute(null);
            }
            else if (sender == ButtonUnDoZeroStock)
            {
                _ViewModel.BookUnZeroStockCommand.Execute(null);
            }
            else if (sender == ButtonBlockQuant)
            {
                _ViewModel.BookBlockQuantCommand.Execute(null);
            }
            else if (sender == ButtonReleaseQuant)
            {
                _ViewModel.BookReleaseQuantCommand.Execute(null);
            }
            else if (sender == ButtonRelocate)
            {
                _ViewModel.BookRelocateCommand.Execute(null);
            }
            else if (sender == ButtonReassign)
            {
                _ViewModel.BookReassignCommand.Execute(null);
            }
            else if (sender == ButtonReassignRelocate)
            {
                _ViewModel.BookReassignRelocateCommand.Execute(null);
            }
        }

#if ZXING
        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
#else
        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
#endif

        {
            _ViewModel.FacilityScanViewModel.Clear();
            #if ZXING
            await barcodeScanner.OpenBarcodeCamera();
            #endif
        }

        private void Print_Clicked(object sender, EventArgs e)
        {
            //_ViewModel.PrintCommand.Execute(null);
            _ViewModel.ShowPrintDialog();
        }

        private async void FacilityEntry_Focused(object sender, FocusEventArgs e)
        {
            _ViewModel.FacilitySelector = new FacilitySelectorViewModel("");
            await Navigation.PushAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));
            FacilityEntry.Unfocus();
            FacilityEntryReassignment.Unfocus();
        }

        private void cmdClearFacility_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedFacility = null;
        }

        private void ButtonSplit_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SplitQuantCommand.Execute(null);
        }

        private async void cmdSumByScan_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOSumQuantityByBarcode(_ViewModel.GetSumByBarcodeModel()));
        }

        private void cmdClearBookingQuantity_Clicked(object sender, EventArgs e)
        {
            BookingQuantity.Text = null;
            RelocationQuantity.Text = null;
        }

        private void cmdClearMovementReason_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedMovementReason = null;
        }

        private void lvMaterials_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (_ViewModel.SelectedMaterial != null)
            {
                _ViewModel.FacilityChargeItem.Material = _ViewModel.SelectedMaterial;
                _ViewModel.MaterialSearchText = _ViewModel.SelectedMaterial.MaterialNo + " " + _ViewModel.SelectedMaterial.MaterialName1;
            }
        }

        private void BtnSelectMaterialCancel_Clicked(object sender, EventArgs e)
        {
            _ViewModel.IsSelectMaterialVisible = false;
        }


#endregion

        private void sbMaterial_Focused(object sender, FocusEventArgs e)
        {
            if (_ViewModel.SelectedMaterial != null)
            {
                _ViewModel.SelectedMaterial = null;
            }
            if (_ViewModel.MaterialList != null && _ViewModel.MaterialList.Any())
            {
                _ViewModel.IsSelectMaterialVisible = true;
            }
            else 
            {
                _ViewModel.GetSuggestedMaterials.Execute(null);
                sbMaterial.Unfocus();
            }
        }

        private void ButtonTakeQuantQuantity_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel != null)
                _ViewModel.TakeBookingQuantityFromQuant();
        }
    }
}