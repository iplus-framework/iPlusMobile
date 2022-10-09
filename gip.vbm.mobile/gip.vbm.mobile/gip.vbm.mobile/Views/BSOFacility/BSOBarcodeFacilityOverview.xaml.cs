using gip.mes.webservices;
using gip.mes.datamodel;
using gip.vbm.mobile.barcode;
using gip.vbm.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using ZXing.Net.Maui;

namespace gip.vbm.mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOBarcodeFacilityOverview : BSOTabbedPageBase
    {
        BarcodeFacilityOverviewModel _ViewModel;
        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;

        public BSOBarcodeFacilityOverview()
		{
            BindingContext = _ViewModel = new BarcodeFacilityOverviewModel();
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);

            InitializeComponent();
            this.CurrentPageChanged += Pivot_SelectionChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitPageOnNavigation();
            InitZXing();
        }

        private void InitPageOnNavigation()
        {
            SubcribeToBarcodeService();

            _ViewModel.OnAppear();
            this.PageState = PageStateEnum.View;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnSubcribeToBarcodeService();
        }

        private void Pivot_SelectionChanged(object sender, EventArgs e)
        {
        }

        #region Barcode
        private void InitZXing()
        {
            scanView.Options = new BarcodeReaderOptions
            {
                AutoRotate = false,
                TryHarder = true,
                Formats = BarcodeFormat.Code128 | BarcodeFormat.Code39 | BarcodeFormat.Ean13 | BarcodeFormat.Ean8 | BarcodeFormat.QrCode,
                Multiple = true
            };
        }

        //ZXing.Mobile.CameraResolution SelectCameraResolution(List<ZXing.Mobile.CameraResolution> availableResolutions)
        //{
        //    var highestResolution = availableResolutions.OrderByDescending(c => c.Width).FirstOrDefault();
        //    return highestResolution;
        //}

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
                SendScanRequest();
            }
        }

        private void scanView_OnScanResult(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
        {
            if (e.Results != null && e.Results.Any())
            {
                _ViewModel.CurrentBarcode = e.Results.FirstOrDefault().Value;
                SendScanRequest();
            }
            _ViewModel.ZXingIsScanning = false;
        }

        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            _ViewModel.Clear();
            _ViewModel.ZXingIsScanning = true;
        }

        private void BtnOpenZXingPanel_Clicked(object sender, EventArgs e)
        {
            _ViewModel.Clear();
            _ViewModel.ZXingIsScanning = true;
        }

        private void BtnCloseZXingPanel_Clicked(object sender, EventArgs e)
        {
            _ViewModel.ZXingIsScanning = false;
        }

        private void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            SendScanRequest();
        }

        private void SendScanRequest()
        {
            if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode))
                _ViewModel.LoadBarcodeEntityCommand.Execute(null);
        }

        private async void BarcodeListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            FacilityCharge fc = e.Item as FacilityCharge;
            if (fc != null)
                await NavigateToFacilityChargeOverview(fc);
            Material material = e.Item as Material;
            if (material != null)
                await NavigateToMaterialOverview(material);
            FacilityLot facilityLot = e.Item as FacilityLot;
            if (facilityLot != null)
                await NavigateToFacilityLotOverview(facilityLot);
            Facility facility = e.Item as Facility;
            if (facility != null)
            {
                if (facility.MDFacilityType != null && facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageLocation)
                    await NavigateToFacilityLocationOverview(facility);
                else
                    await NavigateToFacilityOverview(facility);
            }

        }

        private void BarcodeListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
        }

        private async Task NavigateToFacilityChargeOverview(FacilityCharge fc)
        {
            await Navigation.PushAsync(new BSOFacilityChargeOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = fc } });
        }
        #endregion


        #region Material
        private void MaterialSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MaterialSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_ViewModel.MaterialSearch))
                return;
            _ViewModel.LoadFilteredMaterialsCommand.Execute(null);
        }

        private async void MaterialListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item as Material == null)
                return;
            await NavigateToMaterialOverview(e.Item as Material);
        }

        private void MaterialListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
        }

        private async Task NavigateToMaterialOverview(Material material)
        {
            await Navigation.PushAsync(new BSOFacilityMaterialOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = material } });
        }
        #endregion


        #region Facility
        private void FacilitySearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void FacilitySearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_ViewModel.FacilitySearch) && _ViewModel.SelectedLocation != null)
                return;
            _ViewModel.LoadFilteredFacilitiesCommand.Execute(null);
        }

        private async void FacilityListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item as Facility == null)
                return;
            await NavigateToFacilityOverview(e.Item as Facility);
        }

        private void FacilityListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
        }

        private async void ButtonLocation_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.SelectedLocation != null)
            {
                if (_ViewModel.SelectedLocation.MDFacilityType != null && 
                    _ViewModel.SelectedLocation.MDFacilityType.MDFacilityTypeIndex == (short)mes.datamodel.FacilityTypesEnum.StorageBin)
                {
                    await NavigateToFacilityOverview(_ViewModel.SelectedLocation);
                }
                else
                {
                    await NavigateToFacilityLocationOverview(_ViewModel.SelectedLocation);
                }
            }
        }


        private async Task NavigateToFacilityOverview(Facility facility)
        {
            await Navigation.PushAsync(new BSOFacilityOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = facility } });
        }

        private async Task NavigateToFacilityLocationOverview(Facility facility)
        {
            await Navigation.PushAsync(new BSOFacilityLocationOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = facility } });
        }

        #endregion


        #region Lot
        private void LotSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LotSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_ViewModel.LotSearch))
                return;
            _ViewModel.LoadFilteredLotsCommand.Execute(null);
        }

        private async void LotListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item as FacilityLot == null)
                return;
            await NavigateToFacilityLotOverview(e.Item as FacilityLot);
        }

        private void LotListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
        }

        private async Task NavigateToFacilityLotOverview(FacilityLot facilityLot)
        {
            await Navigation.PushAsync(new BSOFacilityLotOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = facilityLot } });
        }
        #endregion

        protected override bool OnBackButtonPressed()
        {
            ExitOnBackButtonPressed();
            return true;
        }

        private async void FacilityEntry_Focused(object sender, FocusEventArgs e)
        {
            _ViewModel.FacilitySelector = new FacilitySelectorViewModel("");
            await Navigation.PushModalAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));

            FacilityEntry.Unfocus();
        }

        private void cmdClearFacility_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedLocation = null;
        }
    }
}