using gip.vb.mobile.barcode;
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
    public partial class BSOPrinterSettings : BSOPageBase
    {
        public BSOPrinterSettings()
        {
            BindingContext = _ViewModel = new ViewModels.PrinterSettingsViewModel();
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SubcribeToBarcodeService();
            if (_ViewModel != null)
            {
                if (_ViewModel.GetAssignedPrinter != null)
                {
                    _ViewModel.GetAssignedPrinter.Execute(null);
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnSubcribeToBarcodeService();
        }

        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;

        private ViewModels.PrinterSettingsViewModel _ViewModel;


        #region Barcode
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
                if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode))
                {
                    _ViewModel.ScanPrinterID.Execute(null);
                }
            }
        }

        private void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode))
            {
                _ViewModel.ScanPrinterID.Execute(null);
            }
        }
        #endregion

        protected override bool OnBackButtonPressed()
        {
            ExitOnBackButtonPressed();
            return true;
        }
    }
}