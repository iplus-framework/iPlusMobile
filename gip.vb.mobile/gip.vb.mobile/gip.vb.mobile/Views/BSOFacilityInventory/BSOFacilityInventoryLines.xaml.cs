
using gip.mes.webservices;
using gip.vb.mobile.Strings;
using gip.vb.mobile.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOFacilityInventoryLines : BSOPageBase
    {

        #region Properties
        FacilityInventoryLinesViewModel _ViewModel;
        #endregion

        #region cto'rs

        public BSOFacilityInventoryLines()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ViewModel = (FacilityInventoryLinesViewModel)NavParam.Arguments;
            _ViewModel.GetFacilityInventoryPosesCommand.Execute(null);
            _ViewModel.Title = AppStrings.BFI_InventoryPositions_Header;
            BindingContext = null;
            BindingContext = _ViewModel;
            this.PageState = PageStateEnum.View;
        }


        #endregion

        #region Event hanlders
        void InventoryPosListView_ItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            FacilityInventoryPos inventoryPos = args.SelectedItem as FacilityInventoryPos;
            if (inventoryPos == null)
                return;
            _ViewModel.SelectedFacilityInventoryPos = inventoryPos;
        }

        private async void InventoryPosListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            FacilityInventoryPos inventoryPos = e.Item as FacilityInventoryPos;
            if (inventoryPos == null)
                return;
            _ViewModel.SelectedFacilityInventoryPos = inventoryPos;
            await Navigation.PushAsync(new BSOFacilityInventoryLineOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = new FacilityInventoryLineViewModel(_ViewModel.SelectedFacilityInventoryPos, _ViewModel) } });
        }

        private async void InventoryPosOpenFilter_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOFacilityInventoryLinesFilter() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel } });
        }

        async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteGetFacilityInventoryPosesAsync();
        }

        #endregion

    }

}