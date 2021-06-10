using gip.mes.webservices;
using gip.vb.mobile.barcode;
using gip.vb.mobile.Strings;
using gip.vb.mobile.ViewModels;
using gip.vb.mobile.ViewModels.Inventory;
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
    public partial class BSOInventoryLineEdit : BSOPageBase
    {
        InventoryLineEditModel _ViewModel;

        #region ctor's
        public BSOInventoryLineEdit()
        {
            _ViewModel = new InventoryLineEditModel();
            BindingContext = _ViewModel;
            InitializeComponent();
            _ViewModel.BarcodeScannerModel = barcodeScanner.BindingContext as BarcodeScannerModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            barcodeScanner.OnAppearing();
            _ViewModel.InventoryNavArgument = NavParam.Arguments as InventoryNavArgument;
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                _ViewModel.Title = AppStrings.Inv_EditLineS;
            else
                _ViewModel.Title = AppStrings.Inv_EditLine;
            if (_ViewModel.InventoryNavArgument.SelectedInventoryLine != null)
            {
                _ViewModel.SelectedInventoryLine = _ViewModel.InventoryNavArgument.SelectedInventoryLine;
                _ViewModel.WriteNewStockQuantity();
                _ViewModel.IsEditPanelVisible = true;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            barcodeScanner.OnDisappearing();
        }

        #endregion



        #region Event

        private void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                CleanUpForm();
        }

        public void CleanUpForm()
        {
            _ViewModel.SelectedInventoryLine = null;
            _ViewModel.IsEditPanelVisible = false;
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                _ViewModel.Title = AppStrings.Inv_EditLineS;
        }

        private async void cmdUpdate_Clicked(object sender, EventArgs e)
        {
            bool succes = await _ViewModel.ExecuteUpdateFacilityInventoryPosAsync();
            if (succes)
            {
                CleanUpForm();
                switch (_ViewModel.InventoryNavArgument.EditMode)
                {
                    case EditModeEnum.GoAndCount:
                        // nothing stay there
                        break;
                    case EditModeEnum.Confirm:
                        // back to list
                        //await Navigation.PushAsync(
                        //    new BSOInventoryLines()
                        //    {
                        //        NavParam = new NavParameter(PageStateEnum.View)
                        //        {
                        //            Arguments = _ViewModel.InventoryNavArgument
                        //        }
                        //    });
                        Navigation.RemovePage(this);
                        break;
                }
            }
        }

        private async void cmdQuantAdd_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteSetFacilityInventoryChargeAvailable();
        }

        private void cmdQuantEditAgain_Clicked(object sender, EventArgs e)
        {
            _ViewModel.IsEditPanelVisible = true;
        }

        private void BarcodeSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue) && _ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
            {
                _ViewModel.HideChargeCommandPanel();
                _ViewModel.IsEditPanelVisible = false;
            }
        }

        #endregion

        #region Barcode Scanner events
        private void barcodeScanner_OnCleanUpForm(object sender, EventArgs e)
        {
            CleanUpForm();
        }

        private async void barcodeScanner_OnSendSelectedCode(object sender, EventArgs e)
        {
            if (_ViewModel.BarcodeScannerModel.CurrentBarcodeEntity != null && _ViewModel.BarcodeScannerModel.CurrentBarcodeEntity.Any())
            {
                FacilityCharge fc = _ViewModel.BarcodeScannerModel.CurrentBarcodeEntity.FirstOrDefault() as FacilityCharge;
                if (fc != null)
                    await _ViewModel.ExecuteGetFacilityInventorySearchCharge(EditModeEnum.GoAndCount, fc.FacilityChargeID.ToString());
            }
        }

        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            barcodeScanner.CleanUpForm();
             barcodeScanner._ViewModel.ZXingIsScanning = true;
        }

        #endregion

    }
}