using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using gip.vb.mobile.ViewModels;
using gip.mes.webservices;
using System.Collections.Generic;
using System.Linq;
using gip.vb.mobile.barcode;
using System.Threading.Tasks;

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
                _ViewModel.Message = new core.datamodel.Msg() { Message = Strings.AppStrings.EmptyBarcodeEntity_Text };
                return;
            }

            if (barcodeEntity.FacilityCharge != null)
            {
                if (barcodeEntity.FacilityCharge.Material.MaterialID != _ViewModel.Item.Material.MaterialID)
                {
                    _ViewModel.Message = new core.datamodel.Msg() { Message = Strings.AppStrings.PickingWrongMaterial_Text };
                    return;
                }
            }
            else if (barcodeEntity.Facility != null)
            {

            }
            else
            {
                _ViewModel.Message = new core.datamodel.Msg() { Message = Strings.AppStrings.PickingWrongBarcodeEntityQF_Text };
                return;
            }

            _ViewModel.BookFacilityCommand.Execute(null);
        }
        #endregion

        private void TabPrint_Clicked(object sender, EventArgs e)
        {
            if (CurrentPage == BookingHistoryPivotItem)
            {
                _ViewModel.Print(Strings.AppStrings.PrintCopies_Question);
            }
        }

        private void BSOTabbedPageBase_CurrentPageChanged(object sender, EventArgs e)
        {
            if (CurrentPage == BookingHistoryPivotItem)
            {
                TBPrint.IsEnabled = true;
            }
            else
            {
                TBPrint.IsEnabled = false;
            }
        }

        private async void BtnWarehouseInfo_Clicked(object sender, EventArgs e)
        {
            Material material = _ViewModel?.Item?.Material;
            if (material == null)
            {
                _ViewModel.ShowDialog(new core.datamodel.Msg(core.datamodel.eMsgLevel.Error, "Material to overview is null!"));
                return;
            }

            await Navigation.PushAsync(new BSOFacilityMaterialOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments =  new Tuple<Material,string>(material, "QuantPivotItem") } });
        }
    }
}