using gip.mes.webservices;
using gip.vb.mobile.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOFacilityInventory : BSOPageBase
    {

        FacilityInventoryViewModel _ViewModel;

        #region ctor's
        public BSOFacilityInventory()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (NavParam != null && NavParam.Arguments != null)
            {
                _ViewModel = (FacilityInventoryViewModel)NavParam.Arguments;
                _ViewModel.GetFacilityInventoriesCommand.Execute(null);
            }
            else
            {
                _ViewModel = new FacilityInventoryViewModel();
                if (_ViewModel.MDFacilityInventoryStates.Count == 0)
                {
                    _ViewModel.GetListsCommand.Execute(null);
                }
            }
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);
            BindingContext = _ViewModel;
            this.PageState = PageStateEnum.View;
        }


        #endregion

        #region Event handlers

        private async void ButtonInventoryFilter_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOFacilityInventoryFilter() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel } });
        }

        void InventoryListView_ItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            FacilityInventory inventory = args.SelectedItem as FacilityInventory;
            if (inventory == null)
                return;
            _ViewModel.SelectedFacilityInventory = inventory;
        }

        private async void InventoryListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            FacilityInventory inventory = e.Item as FacilityInventory;
            if (inventory == null)
                return;
            _ViewModel.SelectedFacilityInventory = inventory;
            await Navigation.PushAsync(new BSOFacilityInventoryLinesFilter() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.SelectedFacilityInventory.FacilityInventoryNo } });
        }

        async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteGetFacilityInventoriesAsync();
        }

        #endregion
    }

}