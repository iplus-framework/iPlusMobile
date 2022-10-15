﻿using gip.mes.webservices;
using gip.vbm.mobile.barcode;
using gip.vbm.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static gip.mes.datamodel.BarcodeSequenceBase;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using ZXing.Net.Maui;

namespace gip.vbm.mobile.Controls
{
    public enum BarcodeServiceMethodEnum
    {
        InvokeBarcodeCommand = 0,
        GetBarcodeEntityCommand = 1
    }

    public class BarcodeScannerEventArgs : EventArgs
    {
        public BarcodeScannerEventArgs(object value)
        {
            this.Value = value;
        }
        public object Value { get; private set; }
    }

    /// <summary>
    /// Component for barcode scan. Important: Call OnAppearing() and OnDisappearing() from host (parent) page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarcodeScanner : ContentView
    {
        #region DI
        public BarcodeScanModelBase _ViewModel;
        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;
        #endregion

        #region events

        public event EventHandler<BarcodeScannerEventArgs> OnBarcodeCommandInvoked;
        public event EventHandler<BarcodeScannerEventArgs> OnSelectBarcodeEntity;
        public event EventHandler OnCleanUpForm;
        public event EventHandler OnTextChanged;

        #endregion

        #region ctor's

        public BarcodeScanner()
        {
            InitializeComponent();
            _ViewModel = new BarcodeScanModelBase();
            BindingContext = _ViewModel;
        }

        #endregion

        #region Lifecycle


        /// <summary>
        /// OnAppearing - Call in same method name in hosting page
        /// </summary>
        public void OnAppearing()
        {
            SubcribeToBarcodeService();
            BindingContext = _ViewModel;
        }

        /// <summary>
        /// OnDisappearing - Call in same method name in hosting page (parent page)
        /// </summary>
        public void OnDisappearing()
        {
            UnSubcribeToBarcodeService();
            //_IsZXingInitialized = false;
        }


        #endregion

        #region Properties

        public static BindableProperty IsEnabledInvokeBarcodeOnServerProperty = BindableProperty.Create(
             propertyName: nameof(IsEnabledInvokeBarcodeOnServer),
             returnType: typeof(bool?),
             declaringType: typeof(BarcodeScanner),
             defaultValue: false,
             defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Option to disable barcode scanner
        /// </summary>
        public bool IsEnabledInvokeBarcodeOnServer
        {
            get
            {
                return (bool)base.GetValue(IsEnabledInvokeBarcodeOnServerProperty);
            }
            set
            {
                base.SetValue(IsEnabledInvokeBarcodeOnServerProperty, value);
            }
        }


        public static BindableProperty BarcodeIssuerProperty = BindableProperty.Create(
             propertyName: "BarcodeIssuer",
             returnType: typeof(BarcodeIssuerEnum?),
             declaringType: typeof(BarcodeScanner),
             defaultValue: null,
             defaultBindingMode: BindingMode.TwoWay);
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

        public static BindableProperty BarcodeServiceMethodProperty = BindableProperty.Create(
             propertyName: nameof(BarcodeServiceMethod),
             returnType: typeof(BarcodeServiceMethodEnum),
             declaringType: typeof(BarcodeScanner),
             defaultValue: BarcodeServiceMethodEnum.InvokeBarcodeCommand,
             defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Option to disable barcode scanner
        /// </summary>
        public BarcodeServiceMethodEnum BarcodeServiceMethod
        {
            get
            {
                return (BarcodeServiceMethodEnum)base.GetValue(BarcodeServiceMethodProperty);
            }
            set
            {
                base.SetValue(BarcodeServiceMethodProperty, value);
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
            SelectBarcodeEntity(e.Item);
        }

        /// <summary>
        /// On search call server side to return result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            Search((sender as SearchBar)?.Text);
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

        ///// <summary>
        ///// event select camera resolution - use higest available
        ///// </summary>
        ///// <param name="availableResolutions"></param>
        ///// <returns></returns>
        //CameraResolution SelectCameraResolution(List<ZXing.Mobile.CameraResolution> availableResolutions)
        //{
        //    var highestResolution = availableResolutions.OrderByDescending(c => c.Width).FirstOrDefault();
        //    return highestResolution;
        //}

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
                _ViewModel.Item.CurrentBarcode = e.Text;
                _ViewModel.CurrentBarcode = e.Text;
                HandleScanProcess();
            }
        }

        //private void scanView_OnScanResult(ZXing.Result result)
        //{
        //    _ViewModel.CurrentBarcode = result.Text;
        //    _ViewModel.ZXingIsScanning = false;
        //    HandleScanProcess();
        //}

        //private void scanView_OnScanResult(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
        //{
        //    if (e.Results != null && e.Results.Any())
        //    {
        //        _ViewModel.CurrentBarcode = e.Results.FirstOrDefault().Value;
        //        HandleScanProcess();
        //    }
        //    _ViewModel.ZXingIsScanning = false;
        //}

        /// <summary>
        /// For selected code obitain result from server: BarcodeEntity or BarcodeSequence
        /// </summary>
        private async void HandleScanProcess()
        {
            bool success = false;
            if (!String.IsNullOrEmpty(_ViewModel.Item.CurrentBarcode) && IsEnabledInvokeBarcodeOnServer)
            {
                if (this.BarcodeServiceMethod == BarcodeServiceMethodEnum.InvokeBarcodeCommand)
                    success = await _ViewModel.ExecuteInvokeBarcode();
                else //(this.BarcodeServiceMethod == BarcodeServiceMethodEnum.GetBarcodeEntityCommand)
                    success = await _ViewModel.ExecuteGetBarcodeEntityCommand();
            }

            if (!success && !IsEnabledInvokeBarcodeOnServer)
                success = true;

            OnBarcodeCommandInvoked?.Invoke(this, new BarcodeScannerEventArgs(success));
        }

        /// <summary>
        /// Send request is BarcodeEntity object selected to parent level
        /// </summary>
        public void SelectBarcodeEntity(object item)
        {
            OnSelectBarcodeEntity?.Invoke(this, new BarcodeScannerEventArgs(item));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clean up forms and prepare for next usage
        /// </summary>
        public void Clear()
        {
            _ViewModel.Clear();
            if (OnCleanUpForm != null)
                OnCleanUpForm(this, new EventArgs() { });
        }

        public async Task OpenBarcodeCamera()
        {
            BarcodeResult[] result = await App.Root.OpenBarcodeCamera() as BarcodeResult[];
            string scanResult = null;
            if (result != null && result.Any())
                scanResult = result.FirstOrDefault().Value;
            if (_ViewModel != null)
            {
                if (_ViewModel.Item != null)
                    _ViewModel.Item.CurrentBarcode = scanResult;
                _ViewModel.CurrentBarcode = scanResult;
                HandleScanProcess();
            }
        }

        //public void OpenCameraPanel()
        //{
        //    if (_ViewModel != null)
        //        _ViewModel.ZXingIsScanning = true;
        //}

        //public void CloseCameraPanel()
        //{
        //    if (_ViewModel != null)
        //        _ViewModel.ZXingIsScanning = false;
        //}

        private void btnClearList_Clicked(object sender, EventArgs e)
        {
            if (OnCleanUpForm != null)
                OnCleanUpForm(this, new EventArgs() { });
            else
             _ViewModel.Clear();
        }

        public void Search(string text)
        {
            if (_ViewModel.Item != null)
                _ViewModel.Item.CurrentBarcode = text;
            _ViewModel.CurrentBarcode = text;
            HandleScanProcess();
        }


        #endregion

    }
}