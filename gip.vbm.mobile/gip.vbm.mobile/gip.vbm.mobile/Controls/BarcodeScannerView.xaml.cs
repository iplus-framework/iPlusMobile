// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
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
        CustomCommand = 0,
        UseBarcodeSequences = 1,
        DecodeEntity = 2,
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
    public partial class BarcodeScannerView : ContentView
    {
        #region DI
        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;
        #endregion

        #region events
        public event EventHandler<BarcodeScannerEventArgs> OnBarcodeCommandInvoked;
        public event EventHandler<BarcodeScannerEventArgs> OnBarcodeEntityTapped;
        public event EventHandler<BarcodeScannerEventArgs> OnNewBarcodeScanned;
        public event EventHandler OnCleanUpForm;
        public event EventHandler OnTextChanged;
        #endregion

        #region ctor's
        public BarcodeScannerView()
        {
            InitializeComponent();
        }

        #endregion

        #region Lifecycle
        public void OnAppearing()
        {
            SubcribeToBarcodeService();
        }

        public void OnDisappearing()
        {
            UnSubcribeToBarcodeService();
        }
        #endregion

        #region Properties

        public BarcodeScanModelBase ViewModel
        {
            get
            {
                return BindingContext as BarcodeScanModelBase;
            }
            set
            {
                BindingContext = value;
            }
        }


        public static BindableProperty BarcodeIssuerProperty = BindableProperty.Create(
             propertyName: "BarcodeIssuer",
             returnType: typeof(BarcodeIssuerEnum?),
             declaringType: typeof(BarcodeScannerView),
             defaultValue: null,
             defaultBindingMode: BindingMode.TwoWay);
        /// <summary>
        /// The BarcodeIssuer-Property is sent to the server, to control the behaviour of the scanned sequences
        /// This property is needed if BarcodeServiceMethodEnum is set to UseBarcodeSequences
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
             declaringType: typeof(BarcodeScannerView),
             defaultValue: BarcodeServiceMethodEnum.CustomCommand,
             defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// With BarcodeServiceMethod you control which "Barcode"-Method is called on Serverside
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

        public static BindableProperty ShowDecodedEntitiesProperty = BindableProperty.Create(
             propertyName: nameof(ShowDecodedEntities),
             returnType: typeof(bool),
             declaringType: typeof(BarcodeScannerView),
             defaultValue: true,
             defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// With BarcodeServiceMethod you control which "Barcode"-Method is called on Serverside
        /// </summary>
        public bool ShowDecodedEntities
        {
            get
            {
                return (bool)base.GetValue(ShowDecodedEntitiesProperty);
            }
            set
            {
                base.SetValue(ShowDecodedEntitiesProperty, value);
            }
        }
        #endregion

        #region UI-Event-Handling
        private static void HandleValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
        }

        private void BarcodeSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (OnTextChanged != null)
                OnTextChanged(this, new EventArgs());
        }

        private void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            NewBarcodeScanned((sender as SearchBar)?.Text);
        }

        private void BarcodeListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
        }

        private void BarcodeListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            OnBarcodeEntityTapped?.Invoke(this, new BarcodeScannerEventArgs(e.Item));
        }

        private void btnClearList_Clicked(object sender, EventArgs e)
        {
            Clear();
        }
        #endregion

        #region Camera-Scanner (ZXing)
        public async Task OpenBarcodeCamera()
        {
            if (ViewModel != null)
                ViewModel.ZXingIsScanning = true;
            try
            {
                BarcodeResult[] result = await App.Root.OpenBarcodeCamera() as BarcodeResult[];
                string scanResult = null;
                if (result != null && result.Any())
                {
                    scanResult = result.FirstOrDefault().Value;
                    NewBarcodeScanned(scanResult);
                }
            }
            finally
            {
                if (ViewModel != null)
                    ViewModel.ZXingIsScanning = false;
            }
        }
        #endregion

        #region Built-In-Scanner
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
                NewBarcodeScanned(e.Text);
            }
        }
        #endregion

        #region Methods
        public void NewBarcodeScanned(string text)
        {
            if (ViewModel != null)
            {
                if (ViewModel.ExchangedBarcodeSeq != null)
                    ViewModel.ExchangedBarcodeSeq.CurrentBarcode = text;
                ViewModel.CurrentBarcode = text;
            }
            OnNewBarcodeScanned?.Invoke(this, new BarcodeScannerEventArgs(text));
            InvokeBarcodeServiceMethod();
        }

        private async void InvokeBarcodeServiceMethod()
        {
            bool success = false;
            if (this.BarcodeServiceMethod == BarcodeServiceMethodEnum.UseBarcodeSequences)
            {
                if (ViewModel != null && !String.IsNullOrEmpty(ViewModel.ExchangedBarcodeSeq.CurrentBarcode))
                    success = await ViewModel.ExecuteInvokeBarcodeSequenceCommand();
            }
            else if (this.BarcodeServiceMethod == BarcodeServiceMethodEnum.DecodeEntity)
            {
                if (ViewModel != null && !String.IsNullOrEmpty(ViewModel.CurrentBarcode))
                    success = await ViewModel.ExecuteDecodeEntityCommand();
            }
            else
            {
                if (ViewModel != null)
                    success = await ViewModel.ExecuteCustomBarcodeCommand();
            }
            OnBarcodeCommandInvoked?.Invoke(this, new BarcodeScannerEventArgs(success));
        }

        /// <summary>
        /// Clean up forms and prepare for next usage
        /// </summary>
        public void Clear()
        {
            ViewModel?.Clear();
            if (OnCleanUpForm != null)
                OnCleanUpForm(this, new EventArgs() { });
        }
        #endregion

    }
}