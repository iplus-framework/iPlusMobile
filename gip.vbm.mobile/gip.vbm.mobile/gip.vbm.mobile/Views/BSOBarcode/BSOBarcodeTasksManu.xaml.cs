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
            barcodeScanner.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            barcodeScanner.OnDisappearing();
            base.OnDisappearing();
        }

        //private void barcodeScanner_OnBarcodeCommandInvoked(object sender, EventArgs e)
        //{
        //    EnableButtons();
        //}

        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            await barcodeScanner.OpenBarcodeCamera();
        }

        private async void barcodeScanner_OnBarcodeEntityTapped(object sender, BarcodeScannerEventArgs e)
        {
            ProdOrderPartslistWFInfo selectedWfInfo = _ViewModel.SelectedEntity as ProdOrderPartslistWFInfo;
            if (selectedWfInfo != null)
            {
                if (_ViewModel.ExchangedBarcodeSeq.State == mes.datamodel.BarcodeSequenceBase.ActionState.FastSelection)
                {
                    await _ViewModel.InvokeActionOnMachine();
                }
                else
                {
                    await Navigation.PushAsync(new BSOBarcodeTaskManuDetails(_ViewModel));
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            ExitOnBackButtonPressed();
            return true;
        }

        private void SearchTBItem_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.ExchangedBarcodeSeq != null)
            {
                string searchText = _ViewModel.ExchangedBarcodeSeq.CurrentBarcode;
                _ViewModel.FilterSequenceList(searchText);
            }
        }
    }
}