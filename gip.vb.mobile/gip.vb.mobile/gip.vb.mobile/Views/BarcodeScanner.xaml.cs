using gip.vb.mobile.barcode;
using gip.vb.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static gip.mes.datamodel.BarcodeSequenceBase;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarcodeScanner : ContentView
    {
        #region DI
        public BarcodeScannerModel _ViewModel;
        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;
        #endregion

        #region events

        public event EventHandler OnSendSelectedCode;
        public event EventHandler OnCleanUpForm;
        public event EventHandler OnTextChanged;

        #endregion

        #region ctor's

        public BarcodeScanner()
        {
            InitializeComponent();
            _ViewModel = new BarcodeScannerModel();
            BindingContext = _ViewModel;
        }

        #endregion

        #region Lifecycle


        /// <summary>
        /// OnAppearing - Call in same method name in hosting page
        /// </summary>
        public void OnAppearing()
        {
            InitZXing();
            SubcribeToBarcodeService();
            _ViewModel.BarcodeIssuer = BarcodeIssuer;
        }

        /// <summary>
        /// OnDisappearing - Call in same method name in hosting page
        /// </summary>
        public void OnDisappearing()
        {
            UnSubcribeToBarcodeService();
        }


        #endregion

        #region Properties

        public static BindableProperty IsEnabledToFetchBarcodeProperty = BindableProperty.Create(
             propertyName: "IsEnabledToFetchBarcode",
             returnType: typeof(bool?),
             declaringType: typeof(ContentView),
             defaultValue: false,
             defaultBindingMode: BindingMode.TwoWay,
             propertyChanged: HandleValuePropertyChanged);

        /// <summary>
        /// Option to disable barcode scanner
        /// </summary>
        public bool IsEnabledToFetchBarcode
        {
            get
            {
                return (bool)base.GetValue(IsEnabledToFetchBarcodeProperty);
            }
            set
            {
                if (this.IsEnabledToFetchBarcode != value)
                {
                    base.SetValue(IsEnabledToFetchBarcodeProperty, value);
                }
            }
        }


        public static BindableProperty BarcodeIssuerProperty = BindableProperty.Create(
             propertyName: "BarcodeIssuer",
             returnType: typeof(BarcodeIssuerEnum?),
             declaringType: typeof(ContentView),
             defaultValue: null,
             defaultBindingMode: BindingMode.TwoWay,
             propertyChanged: HandleValuePropertyChanged);
        /// <summary>
        /// Setup for usage in barcode sequence mode - setup issuer type to
        /// </summary>
        public BarcodeIssuerEnum? BarcodeIssuer
        {
            get
            {
                return (BarcodeIssuerEnum?)base.GetValue(BarcodeIssuerProperty);
            }
            set
            {
                if (this.BarcodeIssuer != value)
                {
                    base.SetValue(BarcodeIssuerProperty, value);
                }
            }
        }
        #endregion

        #region Event

        /// <summary>
        /// Additional handling property changes
        /// </summary>
        /// <param name="bindable"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private static void HandleValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // ----- Someone changed the full control's Value property. Store
            //       that new value in the internal Switch's IsToggled property.
        }

        /// <summary>
        /// ZXing panel hide on finish
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCloseZXingPanel_Clicked(object sender, EventArgs e)
        {
            _ViewModel.ZXingIsScanning = false;
        }

        /// <summary>
        /// Hanlde select some from barcode obtained item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarcodeListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        /// <summary>
        /// Select some item and call SendSelectedCode()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarcodeListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            SendSelectedCode();
        }

        /// <summary>
        /// On search call server side to return result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            SendScanRequest();
        }

        /// <summary>
        /// observe text changed event in search bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarcodeSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (OnTextChanged != null)
                OnTextChanged(this, new EventArgs());
        }

        #endregion

        #region ZXing

        /// <summary>
        /// setub ZXing scanner
        /// </summary>
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

        /// <summary>
        /// event select camera resolution - use higest available
        /// </summary>
        /// <param name="availableResolutions"></param>
        /// <returns></returns>
        ZXing.Mobile.CameraResolution SelectCameraResolution(List<ZXing.Mobile.CameraResolution> availableResolutions)
        {
            var highestResolution = availableResolutions.OrderByDescending(c => c.Width).FirstOrDefault();
            return highestResolution;
        }

        /// <summary>
        /// Subscribe to barcode service
        /// </summary>
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

        /// <summary>
        /// Unsubscribe to barcode service
        /// </summary>
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

        /// <summary>
        /// handle returned code from barcoode service 
        /// </summary>
        /// <param name="result"></param>
        private void scanView_OnScanResult(ZXing.Result result)
        {
            _ViewModel.CurrentBarcode = result.Text;
            _ViewModel.ZXingIsScanning = false;
            SendScanRequest();
        }

        /// <summary>
        /// For selected code obitain result from server: BarcodeEntity or BarcodeSequence
        /// </summary>
        private void SendScanRequest()
        {
            if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode) && IsEnabledToFetchBarcode)
            {
                if (_ViewModel.BarcodeIssuer != null)
                    _ViewModel.LoadInvokeBarcodeSequence.Execute(null);
                else
                    _ViewModel.LoadBarcodeEntityCommand.Execute(null);
            }
        }

        /// <summary>
        /// Send request is BarcodeEntity object selected to parent level
        /// </summary>
        public async void SendSelectedCode()
        {
            if (OnSendSelectedCode != null)
                OnSendSelectedCode(this, new EventArgs() { });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clean up forms and prepare for next usage
        /// </summary>
        public void Clean()
        {
            _ViewModel.Clean();
            if (OnCleanUpForm != null)
                OnCleanUpForm(this, new EventArgs() { });
        }
        #endregion



    }
}