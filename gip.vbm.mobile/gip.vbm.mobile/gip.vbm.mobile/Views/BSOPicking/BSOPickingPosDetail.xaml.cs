using System;
using gip.vbm.mobile.ViewModels;
using gip.mes.webservices;
using System.Collections.Generic;
using System.Linq;
using gip.vbm.mobile.barcode;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Maui.NullableDateTimePicker;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOPickingPosDetail : BSOTabbedPageBase
    {
        PickingPosDetailViewModel _ViewModel;

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
            barcodeScanner.BarcodeServiceMethod = Controls.BarcodeServiceMethodEnum.DecodeEntity;
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

            if (_ViewModel.PickingItem != null)
            {
                if (_ViewModel.PickingItem.PickingType.PickingType == mes.datamodel.GlobalApp.PickingType.Receipt)
                {
                    stckCollectDetails.IsVisible = true;
                    stckExpDate.IsVisible = true;
                    stckExtLot.IsVisible = true;
                }
            }

            SumQuantityByBarcodeViewModel.SumItem? sumItem = _ViewModel.GetBarcodeInfoFromSumModel();

            if (sumItem != null)
            {
                _ViewModel.ExpirationDate = sumItem.Value.ExpDate;
                _ViewModel.ExternLotNo = sumItem.Value.ExtLotNo;
                if (sumItem.Value.Quantity > double.Epsilon)
                    _ViewModel.BookingQuantity = sumItem.Value.Quantity;
            }
            else
            {
                double? sumQuantity = _ViewModel.GetQuantityFromSumModel();
                if (sumQuantity != null)
                {
                    _ViewModel.BookingQuantity = sumQuantity.Value;
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

#if ZXING
        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
#else
        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
#endif
        {
#if ZXING
           await barcodeScanner.OpenBarcodeCamera();
#endif
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
            _ViewModel.QuantitySetFromNetWeight = false;
        }

        private async void cmdSumByScan_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOSumQuantityByBarcode(_ViewModel.GetSumByBarcodeModel()));
        }

        private async void Entry_Focused(object sender, FocusEventArgs e)
        {
            INullableDateTimePickerOptions nullableDateTimePickerOptions = new NullableDateTimePickerOptions
            {
                NullableDateTime = _ViewModel.ExpirationDate,
                Mode = PickerModes.Date,
                ShowWeekNumbers = false
                // .. other calendar options
            };

            var result = await NullableDateTimePicker.OpenCalendarAsync(nullableDateTimePickerOptions);
            if (result is PopupResult popupResult && popupResult.ButtonResult != PopupButtons.Cancel)
            {
                _ViewModel.ExpirationDate = popupResult.NullableDateTime;
            }
            dtEntry.Unfocus();
        }

        private void cbCollectDetailsOverScan_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (cbCollectDetailsOverScan.IsChecked)
            {
                barcodeScanner.BarcodeServiceMethod = Controls.BarcodeServiceMethodEnum.CustomCommand;
            }
            else
            {
                barcodeScanner.BarcodeServiceMethod = Controls.BarcodeServiceMethodEnum.DecodeEntity;
            }
        }

        private void barcodeScanner_OnNewBarcodeScanned(object sender, Controls.BarcodeScannerEventArgs e)
        {
            if (cbCollectDetailsOverScan.IsChecked)
            {
                string text = e.Value as string;
                _ViewModel.ProcessDetailsBarcode(text);
            }
        }
    }
}