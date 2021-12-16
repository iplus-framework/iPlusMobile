using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.webservices;
using gip.vb.mobile.barcode;
using gip.vb.mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOPickingByMaterialDetail : BSOTabbedPageBase
    {
        private PickingByMaterialViewModel _ViewModel;

        public BSOPickingByMaterialDetail()
        {
            BindingContext = _ViewModel = new PickingByMaterialViewModel();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitPageOnNavigation();
            _ViewModel.PropertyChanged += _ViewModel_PropertyChanged;
        }

        private void _ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_ViewModel != null && e.PropertyName == "MissingBookingQuanity")
            {
                if (_ViewModel.MissingBookingQuanity < 0)
                {
                    lblMissingQuantity.TextColor = Color.Red;
                }
                else
                {
                    lblMissingQuantity.TextColor = Color.Green;
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnSubcribeToBarcodeService();
            _ViewModel.PropertyChanged -= _ViewModel_PropertyChanged;
        }

        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;

        private void InitPageOnNavigation()
        {
            SubcribeToBarcodeService();
            if (NavParam != null)
            {
                PickingMaterial material = NavParam.Arguments as PickingMaterial;
                if (material != null)
                {
                    _ViewModel.Item = material;
                }
            }
            this.PageState = PageStateEnum.View;
        }

        private void PickingsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _ViewModel.ChangePostingQuantity();
        }

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

        private void ButtonDoBooking_Clicked(object sender, EventArgs e)
        {
            BarcodeEntity barcodeEntity = _ViewModel.WSBarcodeEntityResult;
            if (barcodeEntity == null)
            {
                _ViewModel.Message = new core.datamodel.Msg() { MessageLevel = core.datamodel.eMsgLevel.Error, 
                                                                Message = "Barcode entity is null! Please, scan first." };
                return;
            }

            if (barcodeEntity.FacilityCharge != null)
            {
                if (barcodeEntity.FacilityCharge.Material.MaterialID != _ViewModel.Item.Material.MaterialID)
                {
                    _ViewModel.Message = new core.datamodel.Msg() { MessageLevel = core.datamodel.eMsgLevel.Error, 
                                                                    Message = "Material des Quants stimmt nicht mit dem Material der Kommissionierposition überein!" };
                    return;
                }
            }
            else if (barcodeEntity.Facility != null)
            {

            }
            else
            {
                _ViewModel.Message = new core.datamodel.Msg() { MessageLevel = core.datamodel.eMsgLevel.Error,
                                                                Message = "Kein Quant oder Lager gescannt!" };
                return;
            }

            _ViewModel.BookFacilityCommand.Execute(null);
        }

        
    }
}