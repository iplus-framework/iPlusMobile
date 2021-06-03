using gip.mes.webservices;
using gip.vb.mobile.barcode;
using gip.vb.mobile.Strings;
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
        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;

        #region ctor's
        public BSOInventoryLineEdit()
        {
            _ViewModel = new InventoryLineEditModel();
            BindingContext = _ViewModel;
            InitializeComponent();
            InitZXing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SubcribeToBarcodeService();
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
            UnSubcribeToBarcodeService();
        }

        #endregion

        #region ZXing

        private void InitZXing()
        {
            //if (scanView.Options != null)
            //{
            //    //scanView.Options.AutoRotate = false;
            //    scanView.Options.TryHarder = true;
            //    scanView.Options.UseNativeScanning = true;
            //    scanView.Options.PossibleFormats = new ZXing.BarcodeFormat[] { ZXing.BarcodeFormat.CODE_128,
            //                                                                   ZXing.BarcodeFormat.CODE_39,
            //                                                                   ZXing.BarcodeFormat.EAN_13,
            //                                                                   ZXing.BarcodeFormat.EAN_8,
            //                                                                   ZXing.BarcodeFormat.QR_CODE};
            //    if (scanView.Options.CameraResolutionSelector == null)
            //    {
            //        scanView.Options.CameraResolutionSelector = SelectCameraResolution;
            //    }
            //}
        }

        ZXing.Mobile.CameraResolution SelectCameraResolution(List<ZXing.Mobile.CameraResolution> availableResolutions)
        {
            var highestResolution = availableResolutions.OrderByDescending(c => c.Width).FirstOrDefault();
            return highestResolution;
        }

        private void SubcribeToBarcodeService()
        {
            if (_BarcodeService == null)
                _BarcodeService = DependencyService.Get<IBarcodeService>();
            if (!_BarcodeServiceSubcribed)
            {
                _BarcodeService.Read += _BarcodeService_Read;
                _BarcodeServiceSubcribed = true;
            }
        }

        private void UnSubcribeToBarcodeService()
        {
            if (_BarcodeService != null && _BarcodeServiceSubcribed)
            {
                _BarcodeService.Read -= _BarcodeService_Read;
                _BarcodeServiceSubcribed = false;
            }
        }

        private void _BarcodeService_Read(object sender, BarcodeReadEventArgs e)
        {
            if (e != null)
            {
                _ViewModel.CurrentBarcode = e.Text;
                SendScanRequest();
            }
        }

        private void scanView_OnScanResult(ZXing.Result result)
        {
            _ViewModel.CurrentBarcode = result.Text;
            _ViewModel.ZXingIsScanning = false;
            SendScanRequest();
        }

        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                CleanUpForm();
            _ViewModel.ZXingIsScanning = true;
        }

        private void BtnOpenZXingPanel_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                CleanUpForm();
            _ViewModel.ZXingIsScanning = true;
        }

        private void BtnCloseZXingPanel_Clicked(object sender, EventArgs e)
        {
            _ViewModel.ZXingIsScanning = false;
        }

        private void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            SendScanRequest();
        }

        private async void SendScanRequest()
        {
            if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode))
            {
                if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                    _ViewModel.LoadBarcodeEntityCommand.Execute(null);
                else
                    await _ViewModel.ExecuteGetFacilityInventorySearchCharge(EditModeEnum.Confirm, _ViewModel.CurrentBarcode);
            }
        }

        public async void SendSelectedCode()
        {
            if (_ViewModel.CurrentBarcodeEntity != null && _ViewModel.CurrentBarcodeEntity.Any())
            {
                FacilityCharge fc = _ViewModel.CurrentBarcodeEntity.FirstOrDefault() as FacilityCharge;
                if (fc != null)
                    await _ViewModel.ExecuteGetFacilityInventorySearchCharge(EditModeEnum.GoAndCount, fc.FacilityChargeID.ToString());
            }
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
            _ViewModel.CurrentBarcode = null;
            _ViewModel.SelectedInventoryLine = null;
            _ViewModel.IsEditPanelVisible = false;
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                _ViewModel.Title = AppStrings.Inv_EditLineS;
            _ViewModel.CurrentBarcodeEntity = null;
        }

        //private async void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(_ViewModel.InputCode))
        //    {
        //        switch (_ViewModel.InventoryNavArgument.EditMode)
        //        {
        //            case EditModeEnum.GoAndCount:
        //                await _ViewModel.ExecuteGetFacilityInventorySearchCharge(EditModeEnum.GoAndCount);
        //                break;
        //            case EditModeEnum.Confirm:
        //                await _ViewModel.ExecuteGetFacilityInventorySearchCharge(EditModeEnum.Confirm);
        //                break;
        //        }
        //    }
        //}

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

        private void BarcodeListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void BarcodeListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            SendSelectedCode();
        }
    }
}