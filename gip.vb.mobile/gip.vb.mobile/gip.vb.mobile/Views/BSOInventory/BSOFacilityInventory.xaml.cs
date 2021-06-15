﻿using gip.mes.webservices;
using gip.vb.mobile.ViewModels.Inventory;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOFacilityInventory : BSOPageBase
    {
        InventoryModel _ViewModel;

        public BSOFacilityInventory()
        {
            _ViewModel = new InventoryModel();
            BindingContext = _ViewModel;
            InitializeComponent();
        }


        #region Methods

        #region Methods -> Lifecycle


        /// <summary>
        /// Appering - prepare actual inventory list
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_ViewModel.FacilityInventories == null)
                _ViewModel.GetFacilityInventoriesCommand.Execute(null);
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);
            PageState = PageStateEnum.View;
        }

        #endregion

        #region Mehtods -> event handlers

        /// <summary>
        /// Setup select inventory by selecting on list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void InventoryListView_ItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            FacilityInventory inventory = args.SelectedItem as FacilityInventory;
            if (inventory == null)
                return;
            _ViewModel.SelectedFacilityInventory = inventory;
        }

        /// <summary>
        /// Navigate to inventory proces - BSOInvoetoryMode
        /// when dobule select inventory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void InventoryListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            FacilityInventory inventory = e.Item as FacilityInventory;
            if (inventory == null)
                return;
            _ViewModel.SelectedFacilityInventory = inventory;
            await Navigation.PushAsync(new BSOInventoryMode() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = inventory.FacilityInventoryNo } });
        }

        /// <summary>
        /// Hanlde refresh - refresh inventory list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            _ViewModel.GetFacilityInventoriesCommand.Execute(null);
        }

        #endregion

        #endregion
    }
}