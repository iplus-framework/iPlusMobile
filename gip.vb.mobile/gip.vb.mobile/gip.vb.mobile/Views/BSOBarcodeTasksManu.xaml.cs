using gip.mes.webservices;
using gip.vb.mobile.barcode;
using gip.vb.mobile.ViewModels;
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
	public partial class BSOBarcodeTasksManu : BSOPageBase
    {
        BarcodeTasksManuViewModel _ViewModel;
        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;

        public BSOBarcodeTasksManu()
		{
            BindingContext = _ViewModel = new BarcodeTasksManuViewModel();
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SubcribeToBarcodeService();
            EnableButtons();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnSubcribeToBarcodeService();
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
                if (_ViewModel.Item != null)
                    _ViewModel.Item.CurrentBarcode = e.Text;
                SendScanRequest();
            }
        }

        private void scanView_OnScanResult(ZXing.Result result)
        {
            if (_ViewModel.Item != null)
                _ViewModel.Item.CurrentBarcode = result.Text;
            _ViewModel.ZXingIsScanning = false;
            SendScanRequest();
        }

        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            _ViewModel.Clear();
            _ViewModel.ZXingIsScanning = true;
        }

        private void BtnOpenZXingPanel_Clicked(object sender, EventArgs e)
        {
            _ViewModel.Clear();
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

        private void SendScanRequest()
        {
            if (!String.IsNullOrEmpty(_ViewModel.Item.CurrentBarcode))
                _ViewModel.InvokeBarcodeCommand.Execute(null);
        }

        private void BtnReleaseMachine_Clicked(object sender, EventArgs e)
        {
            if (!IsEnabledBtnReleaseMachine())
                return;
            SetAndSendSelectedMachine();
        }

        private void BtnOccupyMachine_Clicked(object sender, EventArgs e)
        {
            if (!IsEnabledBtnOccupyMachine())
                return;
            SetAndSendSelectedMachine();
        }

        private void SetAndSendSelectedMachine()
        {
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            BarcodeEntity entity = _ViewModel.Item.Sequence.LastOrDefault();
            if (entity == null)
            {
                _ViewModel.ResetScanSequence();
                return;
            }
            entity.SelectedOrderWF = wfInfo;
            _ViewModel.InvokeBarcodeCommand.Execute(null);
        }

        private async void BtnDoBooking_Clicked(object sender, EventArgs e)
        {
            if (!IsEnabledBtnDoBooking())
                return;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo.IntermediateBatch != null)
            {
                if (!wfInfo.IntermediateBatch.IsFinalMixure)
                {
                    await Navigation.PushAsync(new BSOProdOrderOutwardMatSel(wfInfo.IntermediateBatch));
                }
                else if (!wfInfo.IntermediateBatch.HasInputMaterials)
                {
                    await Navigation.PushAsync(new BSOProdOrderInward(wfInfo.IntermediateBatch));
                }
                else
                {
                    await Navigation.PushAsync(new BSOProdOrderInOutSelector(wfInfo.IntermediateBatch));
                }
            }
            else if (wfInfo.Intermediate != null)
            {
                await Navigation.PushAsync(new BSOProdOrderBatch(wfInfo.Intermediate));
            }
            else
            {
                await Navigation.PushAsync(new BSOProdOrderIntermediate(wfInfo.ProdOrderPartslist));
            }
            _ViewModel.ResetScanSequence();
        }

        private void BarcodeListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            EnableButtons();
        }

        private void EnableButtons()
        {
            BtnDoBooking.IsEnabled = IsEnabledBtnDoBooking();
            BtnOccupyMachine.IsEnabled = IsEnabledBtnOccupyMachine();
            BtnReleaseMachine.IsEnabled = IsEnabledBtnReleaseMachine();
        }

        private bool IsEnabledBtnDoBooking()
        {
            if (_ViewModel == null)
                return false;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo == null)
                return false;
            return wfInfo.ForRelease;
        }

        private bool IsEnabledBtnOccupyMachine()
        {
            if (_ViewModel == null)
                return false;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo == null)
                return false;
            return !wfInfo.ForRelease;
        }

        private bool IsEnabledBtnReleaseMachine()
        {
            if (_ViewModel == null)
                return false;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo == null)
                return false;
            return wfInfo.ForRelease;
        }
    }
}