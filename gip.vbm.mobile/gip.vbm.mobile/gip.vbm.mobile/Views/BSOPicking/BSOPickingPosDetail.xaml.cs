using System;
using gip.vbm.mobile.ViewModels;
using gip.mes.webservices;
using System.Collections.Generic;
using System.Linq;
using gip.vbm.mobile.barcode;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
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
            barcodeScanner.OnDisappearing();
        }

        private void InitPageOnNavigation()
        {
            barcodeScanner.OnAppearing();
            if (NavParam != null)
            {
                object[] arguments = NavParam.Arguments as object[];
                if (arguments != null && arguments.Count() >= 2)
                {
                    _ViewModel.PickingItem = arguments[0] as Picking;
                    if (_ViewModel.PickingPosItem == null)
                        _ViewModel.PickingPosItem = arguments[1] as PickingPos;
                }
            }

            double? sumQuantity = _ViewModel.GetQuantityFromSumModel();
            if (sumQuantity != null)
            {
                _ViewModel.BookingQuantity = sumQuantity.Value;
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

        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            await barcodeScanner.OpenBarcodeCamera();
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
                if (barcodeEntity.FacilityCharge.Material.MaterialID != _ViewModel.PickingPosItem.Material.MaterialID)
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
            Material material = _ViewModel?.PickingPosItem?.Material;
            if (material == null)
            {
                _ViewModel.ShowDialog(new core.datamodel.Msg(core.datamodel.eMsgLevel.Error, "Material to overview is null!"));
                return;
            }

            await Navigation.PushAsync(new BSOFacilityMaterialOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments =  new Tuple<Material,string>(material, "QuantPivotItem") } });
        }

        private void cmdClearBookingQuantity_Clicked(object sender, EventArgs e)
        {
            BookingQuantity.Text = null;
        }

        private async void cmdSumByScan_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOSumQuantityByBarcode(_ViewModel.GetSumByBarcodeModel()));
        }
    }
}