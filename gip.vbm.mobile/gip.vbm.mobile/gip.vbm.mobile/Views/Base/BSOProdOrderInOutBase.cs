using gip.mes.webservices;
using gip.vbm.mobile.barcode;
using gip.vbm.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    public class BSOProdOrderInOutBase : BSOTabbedPageBase
    {
        internal ProdOrderInOutViewModel _ViewModel;
        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;

        public BSOProdOrderInOutBase() : base()
        {

        }

        //public BSOProdOrderInOutBase(ProdOrderPartslistPosRelation rel)
        //{
        //    BindingContext = _ViewModel = new ProdOrderInOutViewModel(true, rel);
        //}

        protected async override void OnAppearing()
        {
            SubcribeToBarcodeService();
            base.OnAppearing();
            await _ViewModel.ExecuteReadPostingsCommand();
        }

        protected override void OnDisappearing()
        {
            UnSubcribeToBarcodeService();
            base.OnDisappearing();
        }

        #region Barcode
        public void SubcribeToBarcodeService()
        {
            if (_BarcodeService == null)
                _BarcodeService = DependencyService.Get<IBarcodeService>();
            if (!_BarcodeServiceSubcribed)
            {
                _BarcodeService.Read += _BarcodeService_Read;
                _BarcodeServiceSubcribed = true;
            }
        }

        public void UnSubcribeToBarcodeService()
        {
            if (_BarcodeService != null && _BarcodeServiceSubcribed)
            {
                _BarcodeService.Read -= _BarcodeService_Read;
                _BarcodeServiceSubcribed = false;
            }
        }

        public void _BarcodeService_Read(object sender, BarcodeReadEventArgs e)
        {
            if (e != null)
            {
                _ViewModel.CurrentBarcode = e.Text;
                if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode))
                    _ViewModel.LoadBarcodeEntityCommand.Execute(null);
            }
        }

        public void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode))
                _ViewModel.LoadBarcodeEntityCommand.Execute(null);
        }
        #endregion

        
    }
}
