using gip.vbm.mobile.DependencyServices;
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
    public partial class BSOPicking : BSOPageBase
    {
        private PickingViewModel _ViewModel;
        

        public BSOPicking()
        {
            BindingContext = _ViewModel = new PickingViewModel();
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _ViewModel.OnAppear();

        }

        private void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            _ViewModel.LoadPickingTypesCommand.Execute(null);
        }

        private void cmdClearPickingType_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedPickingType = null;
        }

        private async void cmdClearFacilityFrom_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.SelectedStorageLocationFrom != null)
                _ViewModel.SelectedStorageLocationFrom = null;
            else
            {
                _ViewModel.FacilitySelector = new FacilitySelectorViewModel(PickingViewModel.PN_SelectedStorageLocationFrom);
                await Navigation.PushAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));
            }
        }

        private async void cmdClearFacilityTo_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.SelectedStorageLocationTo != null)
                _ViewModel.SelectedStorageLocationTo = null;
            else
            {
                _ViewModel.FacilitySelector = new FacilitySelectorViewModel(PickingViewModel.PN_SelectedStorageLocationTo);
                await Navigation.PushAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));
            }
        }

        private async void btnShowOrders_Clicked(object sender, EventArgs e)
        {
            _ViewModel.IsGroupedByMaterial = false;
            await Navigation.PushAsync(new BSOPickingItems() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel } });
        }

        private async void btnGroupView_Clicked(object sender, EventArgs e)
        {
            _ViewModel.IsGroupedByMaterial = true;
            await Navigation.PushAsync(new BSOPickingByMaterial() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel } });
        }

        //Workaround with button: https://github.com/dotnet/maui/issues/21009
        //private async void FacilityFromEntry_Focused(object sender, FocusEventArgs e)
        //{
        //    _ViewModel.FacilitySelector = new FacilitySelectorViewModel(PickingViewModel.PN_SelectedStorageLocationFrom);
        //    await Navigation.PushAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));

        //    FacilityFromEntry.Unfocus();
        //}

        //private async void FacilityToEntry_Focused(object sender, FocusEventArgs e)
        //{
        //    _ViewModel.FacilitySelector = new FacilitySelectorViewModel(PickingViewModel.PN_SelectedStorageLocationTo);
        //    await Navigation.PushAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));

        //    FacilityToEntry.Unfocus();
        //}

        protected override bool OnBackButtonPressed()
        {
            ExitOnBackButtonPressed();
            return true;
        }
    }
}