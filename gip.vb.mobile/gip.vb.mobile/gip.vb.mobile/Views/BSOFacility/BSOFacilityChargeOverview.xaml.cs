// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.webservices;
using gip.vb.mobile.barcode;
using gip.vb.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.vb.mobile.Controls;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOFacilityChargeOverview : BSOTabbedPageBase
    {
        #region c'tors

        public BSOFacilityChargeOverview()
		{
            BindingContext = _ViewModel = new FacilityChargesViewModel();
            barcodeScanner = new BarcodeScanner();
            barcodeScanner.IsEnabledInvokeBarcodeOnServer = true;
            InitializeComponent();
        }

        #endregion

        #region Properties

        FacilityChargesViewModel _ViewModel;

        BarcodeScanner barcodeScanner;

        #endregion

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();
            barcodeScanner._ViewModel = _ViewModel.FacilityScanViewModel;
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
            await Navigation.PushAsync(new BSOFacilityBookingHistory() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.Item } });
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

        private void Print_Clicked(object sender, EventArgs e)
        {
            //_ViewModel.PrintCommand.Execute(null);
            _ViewModel.ShowPrintDialog();
        }

        private async void FacilityEntry_Focused(object sender, FocusEventArgs e)
        {
            _ViewModel.FacilitySelector = new FacilitySelectorViewModel("");
            await Navigation.PushModalAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));
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
                _ViewModel.Item.Material = _ViewModel.SelectedMaterial;
                _ViewModel.MaterialSearchText = _ViewModel.SelectedMaterial.MaterialNo + " " + _ViewModel.SelectedMaterial.MaterialName1;
            }
        }

        private void BtnSelectMaterialCancel_Clicked(object sender, EventArgs e)
        {
            _ViewModel.IsSelectMaterialVisible = false;
        }


        #endregion

        private async void sbMaterial_Focused(object sender, FocusEventArgs e)
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
                await _ViewModel.ExecuteGetSuggestedMaterials();

                //_ViewModel.GetSuggestedMaterials.Execute(null);
                if (_ViewModel.SelectedMaterial != null)
                    sbMaterial.Unfocus();
            }
        }

        private void ButtonTakeQuantQuantity_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel != null)
                _ViewModel.TakeBookingQuantityFromQuant();
        }

        private async void ButtonNavigateToLastPosting_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel != null)
            {
                BarcodeEntity entity = await _ViewModel.GetLastPostingData();
                if (entity != null)
                {
                    if (entity.SelectedOrderWF != null)
                    {
                        ProdOrderInMaterialsViewModel inModel = new ProdOrderInMaterialsViewModel(entity.SelectedOrderWF.IntermediateBatch, null);
                        BSOProdOrderOutward prodOrderOutward = new BSOProdOrderOutward(entity.SelectedOrderWF.Relation, null, null, inModel);
                        prodOrderOutward._ViewModel.CurrentBarcodeEntity = new List<object> { _ViewModel.Item };
                        prodOrderOutward._ViewModel.WSBarcodeEntityResult = new BarcodeEntity { FacilityCharge = _ViewModel.Item }; ;
                        await Navigation.PushAsync(prodOrderOutward);
                    }
                }
            }
        }
    }
}