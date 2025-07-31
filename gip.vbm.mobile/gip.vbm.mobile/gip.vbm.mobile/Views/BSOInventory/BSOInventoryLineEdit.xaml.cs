using gip.mes.webservices;
using gip.vbm.mobile.barcode;
using gip.vbm.mobile.Strings;
using gip.vbm.mobile.ViewModels;
using gip.vbm.mobile.ViewModels.Inventory;
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
    public partial class BSOInventoryLineEdit : BSOPageBase
    {
        InventoryLineEditModel _ViewModel;

        #region ctor's
        public BSOInventoryLineEdit()
        {
            _ViewModel = new InventoryLineEditModel();
            _ViewModel.BarcodeScannerModel = new BarcodeScanInventoryModel(this);
            BindingContext = _ViewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            //_ViewModel.BarcodeScannerModel = new BarcodeScanInventoryModel(this);
            ////barcodeScanner._ViewModel = _ViewModel.BarcodeScannerModel;
            //barcodeScanner.OnAppearing();
            //_ViewModel.InventoryNavArgument = NavParam.Arguments as InventoryNavArgument;
            ////_ViewModel.BarcodeScannerModel = barcodeScanner._ViewModel;
            //_ViewModel.CleanUpForm();
            //_ViewModel.CleanBarcodeAndSetCurrentFacility();
            //_ViewModel.Start();
            //base.OnAppearing();
            //barcodeScanner.IsVisible = false;

            _ViewModel.InventoryNavArgument = NavParam.Arguments as InventoryNavArgument;
            if (_ViewModel._SumByBarcodeModel == null)
            {
                _ViewModel.CleanUpForm();
                _ViewModel.CleanBarcodeAndSetCurrentFacility();
            }
            _ViewModel.Start();
            base.OnAppearing();

            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                btnCheckQuant.IsVisible = false;
            /*
            barcodeScanner.OnAppearing();
            barcodeScanner.IsVisible = false;
            */
        }

        protected override void OnDisappearing()
        {
            barcodeScanner.OnDisappearing();
            base.OnDisappearing();
        }

        #endregion

        #region Event

        private void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                _ViewModel.CleanUpForm();
        }

        private async void cmdUpdate_Clicked(object sender, EventArgs e)
        {
            bool success = await _ViewModel.ExecuteUpdateFacilityInventoryPosAsync();
            if (success)
            {
                _ViewModel.CleanUpForm();
                _ViewModel.CleanBarcodeAndSetCurrentFacility();
                switch (_ViewModel.InventoryNavArgument.EditMode)
                {
                    case EditModeEnum.GoAndCount:
                        // nothing stay there
                        break;
                    case EditModeEnum.Confirm:
                        Navigation.RemovePage(this);
                        break;
                }
            }
            //barcodeScanner.IsVisible = false;
        }

        private void cmdQuantEditAgain_Clicked(object sender, EventArgs e)
        {
            _ViewModel.IsEditPanelVisible = true;
        }

        #endregion

        #region Barcode Scanner events
        private async void barcodeScanner_OnBarcodeReceived(object sender, EventArgs e)
        {
            if (_ViewModel.BarcodeScannerModel.ExchangedBarcodeSeq.Sequence != null)
            {
                if (_ViewModel.BarcodeScannerModel.ExchangedBarcodeSeq.Sequence.Where(c => c.FacilityCharge != null).Count() == 1)
                {
                    BarcodeEntity barcodeEntity = _ViewModel.BarcodeScannerModel.ExchangedBarcodeSeq.Sequence.Where(c => c.FacilityCharge != null).FirstOrDefault();
                    _ViewModel.CurrentFacilityCharge = barcodeEntity.FacilityCharge;
                    bool success = await _ViewModel.ExecuteGetFacilityInventorySearchCharge();
                }
            }
        }

        private async void barcodeScanner_OnBarcodeEntityTapped(object sender, EventArgs e)
        {
            if (sender != null && sender is BarcodeEntity)
            {
                BarcodeEntity barcodeEntity = sender as BarcodeEntity;
                if (barcodeEntity.FacilityCharge != null)
                {
                    _ViewModel.CurrentFacilityCharge = barcodeEntity.FacilityCharge;
                    bool success = await _ViewModel.ExecuteGetFacilityInventorySearchCharge();
                }
            }
        }

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


        private void barcodeScanner_OnCleanUpForm(object sender, EventArgs e)
        {
            _ViewModel.CleanBarcodeAndSetCurrentFacility();
        }

        protected override bool OnBackButtonPressed()
        {
            if (_ViewModel.BarcodeScannerModel.IsListVisible)
            {
                _ViewModel.CleanBarcodeAndSetCurrentFacility();
                return true;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }

#endregion

        private async void btnNewQuant_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOFacilityChargeMaker() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.InventoryNavArgument.FacilityInventoryID } });
        }

        private async void cmdSumByScan_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOSumQuantityByBarcode(_ViewModel.GetSumByBarcodeModel()));
        }

        private void cmdClearBookingQuantity_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel != null && _ViewModel.SelectedInventoryLine != null)
            {
                _ViewModel.SelectedInventoryLine.NewStockQuantity = null;
            }
        }

        private void btnCheckQuant_Clicked(object sender, EventArgs e)
        {
            barcodeScanner.IsVisible = !barcodeScanner.IsVisible;
        }
    }
}