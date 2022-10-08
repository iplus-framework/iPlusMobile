using gip.vbm.mobile.Controls;
using gip.vbm.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOPickingWorkplace : BSOPageBase
    {
        public BSOPickingWorkplace()
        {
            BarcodeScan = new BarcodeScanner();
            BarcodeScan.IsEnabledInvokeBarcodeOnServer = true;
            BindingContext = _ViewModel = new PickingWorkplaceViewModel();
            BarcodeScan._ViewModel = new BarcodeScanACClassModel(_ViewModel);
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);
            InitializeComponent();
        }

        private PickingWorkplaceViewModel _ViewModel;

        public BarcodeScanner BarcodeScan
        {
            get;
            set;
        }

        protected override void OnAppearing()
        {
            BarcodeScan.OnAppearing();
            _ViewModel.OnAppear();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            BarcodeScan.OnDisappearing();
            base.OnDisappearing();
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            BarcodeScan.Search(SBRegisteredWorkplace.Text);
        }

        private void cmdClearPickingType_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedPickingType = null;
        }

        private void cmdClearFacilityFrom_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedStorageLocationFrom = null;
        }

        private void cmdClearFacilityTo_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedStorageLocationTo = null;
        }

        private async void FacilityFromEntry_Focused(object sender, FocusEventArgs e)
        {
            _ViewModel.FacilitySelector = new FacilitySelectorViewModel(PickingViewModel.PN_SelectedStorageLocationFrom);
            await Navigation.PushModalAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));

            FacilityFromEntry.Unfocus();
        }

        private async void FacilityToEntry_Focused(object sender, FocusEventArgs e)
        {
            _ViewModel.FacilitySelector = new FacilitySelectorViewModel(PickingViewModel.PN_SelectedStorageLocationTo);
            await Navigation.PushModalAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));

            FacilityToEntry.Unfocus();
        }

        private async void btnShowOrders_Clicked(object sender, EventArgs e)
        {
            _ViewModel.IsGroupedByMaterial = false;
            await Navigation.PushAsync(new BSOPickingItemsWorkplace() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel } });
        }

        protected override bool OnBackButtonPressed()
        {
            ExitOnBackButtonPressed();
            return true;
        }
    }
}