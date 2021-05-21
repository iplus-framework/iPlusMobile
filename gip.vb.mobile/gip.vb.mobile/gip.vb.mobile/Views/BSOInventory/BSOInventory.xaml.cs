using gip.mes.webservices;
using gip.vb.mobile.ViewModels;
using gip.vb.mobile.ViewModels.Inventory;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOInventory : BSOPageBase
    {
        InventoryModel _ViewModel;

        public BSOInventory()
        {
            _ViewModel = new InventoryModel();
            BindingContext = _ViewModel;
            InitializeComponent();
        }


        #region Event handlers

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_ViewModel.FacilityInventories == null)
                _ViewModel.GetFacilityInventoriesCommand.Execute(null);
            this.PageState = PageStateEnum.View;
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
            await Navigation.PushAsync(new BSOInventoryMode() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = inventory.FacilityInventoryNo } });
        }

        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteGetFacilityInventoriesAsync();
        }

        #endregion
    }
}