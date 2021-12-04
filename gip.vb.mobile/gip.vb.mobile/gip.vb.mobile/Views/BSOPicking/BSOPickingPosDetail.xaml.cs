using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using gip.vb.mobile.ViewModels;
using gip.mes.webservices;
using System.Collections.Generic;
using System.Linq;
using gip.vb.mobile.barcode;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOPickingPosDetail : BSOTabbedPageBase
    {
        PickingPosDetailViewModel _ViewModel;
        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;

        public BSOPickingPosDetail(PickingPosDetailViewModel viewModel)
        {
            BindingContext = this._ViewModel = viewModel;
            InitializeComponent();
        }

        public BSOPickingPosDetail()
        {
            _ViewModel = new PickingPosDetailViewModel();
            BindingContext = _ViewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitPageOnNavigation();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnSubcribeToBarcodeService();
        }

        private void InitPageOnNavigation()
        {
            SubcribeToBarcodeService();
            if (NavParam != null)
            {
                object[] arguments = NavParam.Arguments as object[];
                if (arguments != null && arguments.Count() >= 2)
                {
                    _ViewModel.PickingItem = arguments[0] as Picking;
                    _ViewModel.Item = arguments[1] as PickingPos;
                }
            }
            _ViewModel.ReadPostingsCommand.Execute(null);
            this.PageState = PageStateEnum.View;
        }

        #region Picking
        private void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            _ViewModel.ReadPickingPosCommand.Execute(null);
        }
        #endregion

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
                    _ViewModel.LoadBarcodeEntityCommand.Execute(null);
            }
        }

        private void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode))
                _ViewModel.LoadBarcodeEntityCommand.Execute(null);
        }
        #endregion

        #region Booking
        private void ButtonDoBooking_Clicked(object sender, EventArgs e)
        {
            BarcodeEntity barcodeEntity = _ViewModel.WSBarcodeEntityResult;
            if (barcodeEntity == null)
            {
                _ViewModel.Message = new core.datamodel.Msg() { Message = "Barcode entity is null!" };
                return;
            }

            if (barcodeEntity.FacilityCharge != null)
            {
                if (barcodeEntity.FacilityCharge.Material.MaterialID != _ViewModel.Item.Material.MaterialID)
                {
                    _ViewModel.Message = new core.datamodel.Msg() { Message = "Material des Quants stimmt nicht mit dem Material der Kommissionierposition überein!" };
                    return;
                }
            }
            else if (barcodeEntity.Facility != null)
            {

            }
            else
            {
                _ViewModel.Message = new core.datamodel.Msg() { Message = "Kein Quant oder Lager gescannt!" };
                return;
            }

            _ViewModel.BookFacilityCommand.Execute(null);
        }
        #endregion
    }
}