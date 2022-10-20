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
            //EnableButtons();
            //SendChangedACMethod();
            //if (_ViewModel != null)
            //    _ViewModel.ResetScanSequence();
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

        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            _ViewModel.Clear();
            barcodeScanner.OpenCameraPanel();
        }



        private async void barcodeScanner_OnSelectBarcodeEntity(object sender, EventArgs e)
        {
            ProdOrderPartslistWFInfo selectedWfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (selectedWfInfo != null)
            {
                await Navigation.PushAsync(new BSOBarcodeTaskManuDetails(_ViewModel));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            ExitOnBackButtonPressed();
            return true;
        }

        private void SearchTBItem_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.Item != null)
            {
                string searchText = _ViewModel.Item.CurrentBarcode;
                _ViewModel.FilterSequenceList(searchText);
            }
        }
    }
}