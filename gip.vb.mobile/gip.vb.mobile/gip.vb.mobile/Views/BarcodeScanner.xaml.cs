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
    public partial class BarcodeScanner : ContentView
    {
        BarcodeScannerModel _ViewModel;
        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;

        public event EventHandler OnSendSelectedCode;
        public event EventHandler OnCleanUpForm;
        public event EventHandler OnTextChanged;


        #region ctor's
        public BarcodeScanner()
        {
            InitializeComponent();
            _ViewModel = new BarcodeScannerModel();
            BindingContext = _ViewModel;
        }



        #endregion

        #region Lifecycle

        public void OnAppearing()
        {
            InitZXing();
            SubcribeToBarcodeService();
        }

        public void OnDisappearing()
        {
            UnSubcribeToBarcodeService();
        }


        #endregion


        #region Event

        private void BtnCloseZXingPanel_Clicked(object sender, EventArgs e)
        {
            _ViewModel.ZXingIsScanning = false;
        }

        private void BarcodeListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void BarcodeListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            SendSelectedCode();
        }

        private void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            SendScanRequest();
        }
        private void BarcodeSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (OnTextChanged != null)
                OnTextChanged(this, new EventArgs());
        }

        #endregion

        #region ZXing

        private void InitZXing()
        {
            if (scanView.Options != null)
            {
                scanView.Options.AutoRotate = false;
                scanView.Options.TryHarder = true;
                scanView.Options.UseNativeScanning = true;
                scanView.Options.PossibleFormats = new ZXing.BarcodeFormat[] { ZXing.BarcodeFormat.CODE_128,
                                                                               ZXing.BarcodeFormat.CODE_39,
                                                                               ZXing.BarcodeFormat.EAN_13,
                                                                               ZXing.BarcodeFormat.EAN_8,
                                                                               ZXing.BarcodeFormat.QR_CODE};
                if (scanView.Options.CameraResolutionSelector == null)
                {
                    scanView.Options.CameraResolutionSelector = SelectCameraResolution;
                }
            }
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
            CleanUpForm();
            _ViewModel.ZXingIsScanning = true;
        }

        private void BtnOpenZXingPanel_Clicked(object sender, EventArgs e)
        {
            CleanUpForm();
            _ViewModel.ZXingIsScanning = true;
        }


        private void SendScanRequest()
        {
            if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode))
            {
                _ViewModel.LoadBarcodeEntityCommand.Execute(null);
            }
        }

        public async void SendSelectedCode()
        {
            if (OnSendSelectedCode != null)
                OnSendSelectedCode(this, new EventArgs() { });
        }

        #endregion

        #region Methods
        public void CleanUpForm()
        {
            _ViewModel.CurrentBarcode = null;
            _ViewModel.CurrentBarcodeEntity = null;
            if (OnCleanUpForm != null)
                OnCleanUpForm(this, new EventArgs() { });
        }
        #endregion


    }
}