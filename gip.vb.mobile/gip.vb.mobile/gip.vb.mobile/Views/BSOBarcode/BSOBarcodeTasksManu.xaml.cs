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
        BarcodeScanManuModel _ViewModel;

        public BSOBarcodeTasksManu()
		{
            BindingContext = _ViewModel = new BarcodeScanManuModel();
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            barcodeScanner._ViewModel = _ViewModel;
            barcodeScanner.OnAppearing();
            EnableButtons();
            SendChangedACMethod();
        }

        protected override void OnDisappearing()
        {
            barcodeScanner.OnDisappearing();
            base.OnDisappearing();
        }

        private void barcodeScanner_OnBarcodeCommandInvoked(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            _ViewModel.Clear();
            barcodeScanner.OpenCameraPanel();
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

        private async void BtnEnterACMParams_Clicked(object sender, EventArgs e)
        {
            if (!IsEnabledBtnEnterACMParams())
                return;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo == null || wfInfo.WFMethod == null)
                return;
            wfInfo.WFMethod.AutoRemove = false;
            await Navigation.PushAsync(new BSOACMethodEditor(wfInfo.WFMethod));
        }

        private void SendChangedACMethod()
        {
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo != null && wfInfo.WFMethod != null && wfInfo.WFMethod.AutoRemove == true)
            {
                wfInfo.WFMethod.AutoRemove = false;
                BarcodeEntity entity = _ViewModel.Item.Sequence.LastOrDefault();
                if (entity == null)
                {
                    _ViewModel.ResetScanSequence();
                    return;
                }
                entity.SelectedOrderWF = wfInfo;
                entity.WFMethod = wfInfo.WFMethod;
                _ViewModel.InvokeBarcodeCommand.Execute(null);
            }
        }

        private void EnableButtons()
        {
            BtnDoBooking.IsEnabled = IsEnabledBtnDoBooking();
            BtnOccupyMachine.IsEnabled = IsEnabledBtnOccupyMachine();
            BtnReleaseMachine.IsEnabled = IsEnabledBtnReleaseMachine();
            BtnEnterACMParams.IsEnabled = IsEnabledBtnEnterACMParams();
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

        private void barcodeScanner_OnSelectBarcodeEntity(object sender, EventArgs e)
        {
            EnableButtons();
        }

        protected override bool OnBackButtonPressed()
        {
            ExitOnBackButtonPressed();
            return true;
        }

        private bool IsEnabledBtnEnterACMParams()
        {
            if (_ViewModel == null)
                return false;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo == null)
                return false;
            return wfInfo.WFMethod != null;
        }
    }
}