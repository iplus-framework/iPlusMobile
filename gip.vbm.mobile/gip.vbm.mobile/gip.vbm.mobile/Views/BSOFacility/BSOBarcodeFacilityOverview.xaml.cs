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
using CommunityToolkit.Maui.Views;
using gip.vbm.mobile.Controls;
using gip.core.webservices;

namespace gip.vbm.mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOBarcodeFacilityOverview : BSOTabbedPageBase
    {
        BarcodeFacilityOverviewModel _ViewModel;

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
            barcodeScanner.OnAppearing();
            InitPageOnNavigation();
        }

        private void InitPageOnNavigation()
        {
            _ViewModel.OnAppear();
            this.PageState = PageStateEnum.View;
        }

        protected override void OnDisappearing()
        {
            barcodeScanner.OnDisappearing();
            base.OnDisappearing();
        }

        private void Pivot_SelectionChanged(object sender, EventArgs e)
        {
        }

        #region Barcode
#if ZXING
        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
#else
        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
#endif

        {
            _ViewModel.Clear();
            #if ZXING
            await barcodeScanner.OpenBarcodeCamera();
            #endif
        }

        private async void barcodeScanner_OnBarcodeEntityTapped(object sender, BarcodeScannerEventArgs e)
        {
            FacilityCharge fc = e.Value as FacilityCharge;
            if (fc != null)
                await NavigateToFacilityChargeOverview(fc);
            Material material = e.Value as Material;
            if (material != null)
                await NavigateToMaterialOverview(material);
            FacilityLot facilityLot = e.Value as FacilityLot;
            if (facilityLot != null)
                await NavigateToFacilityLotOverview(facilityLot);
            Facility facility = e.Value as Facility;
            if (facility != null)
            {
                if (facility.MDFacilityType != null && facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageLocation)
                    await NavigateToFacilityLocationOverview(facility);
                else
                    await NavigateToFacilityOverview(facility);
            }

            core.webservices.ACClass operationACClass = e.Value as core.webservices.ACClass;
            if (operationACClass != null)
            {
                await NavigateToOperationLogOverview(operationACClass);
            }
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

        #region OperationLog

        private async Task NavigateToOperationLogOverview(ACClass acClass)
        {
            await Navigation.PushAsync(new BSOOperationLogOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = acClass } });
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
            await Navigation.PushAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));

            FacilityEntry.Unfocus();
        }

        private void cmdClearFacility_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedLocation = null;
        }

    }
}