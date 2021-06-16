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
        }

        protected override void OnAppearing()
        {

            barcodeScanner.OnAppearing();
            _ViewModel.InventoryNavArgument = NavParam.Arguments as InventoryNavArgument;
            _ViewModel.BarcodeScannerModel = barcodeScanner._ViewModel;
            _ViewModel.CleanUpForm();
            _ViewModel.CleanBarcodeAndSetCurrentFacility();
            _ViewModel.Start();
            base.OnAppearing();
            barcodeScanner.IsVisible = _ViewModel.IsSearchPanelVisible;
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
            barcodeScanner.IsVisible = _ViewModel.IsSearchPanelVisible;
        }

        private void cmdQuantEditAgain_Clicked(object sender, EventArgs e)
        {
            _ViewModel.IsEditPanelVisible = true;
        }

        #endregion

        #region Barcode Scanner events
        private async void barcodeScanner_OnBarcodeReceived(object sender, EventArgs e)
        {
            if (_ViewModel.BarcodeScannerModel.BarcodeSequence.Sequence != null)
            {
                if (_ViewModel.BarcodeScannerModel.BarcodeSequence.Sequence.Where(c => c.FacilityCharge != null).Count() == 1)
                {
                    BarcodeEntity barcodeEntity = _ViewModel.BarcodeScannerModel.BarcodeSequence.Sequence.Where(c => c.FacilityCharge != null).FirstOrDefault();
                    _ViewModel.CurrentFacilityCharge = barcodeEntity.FacilityCharge;
                    bool success = await _ViewModel.ExecuteGetFacilityInventorySearchCharge();
                    barcodeScanner.IsVisible = _ViewModel.IsSearchPanelVisible;
                }
            }
        }

        private async void barcodeScanner_OnSendSelectedCode(object sender, EventArgs e)
        {
            if (sender != null && sender is BarcodeEntity)
            {
                BarcodeEntity barcodeEntity = sender as BarcodeEntity;
                if (barcodeEntity.FacilityCharge != null)
                {
                    _ViewModel.CurrentFacilityCharge = barcodeEntity.FacilityCharge;
                    bool success = await _ViewModel.ExecuteGetFacilityInventorySearchCharge();
                    barcodeScanner.IsVisible = _ViewModel.IsSearchPanelVisible;
                }
            }
        }

        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            barcodeScanner._ViewModel.ZXingIsScanning = true;
        }

        private void barcodeScanner_OnCleanUpForm(object sender, EventArgs e)
        {
            _ViewModel.CleanBarcodeAndSetCurrentFacility();
        }

        #endregion


    }
}