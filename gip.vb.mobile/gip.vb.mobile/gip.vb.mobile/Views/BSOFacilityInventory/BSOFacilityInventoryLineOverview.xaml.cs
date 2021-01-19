using gip.mes.webservices;
using gip.vb.mobile.Strings;
using gip.vb.mobile.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOFacilityInventoryLineOverview : BSOPageBase
    {

        #region Properties

        FacilityInventoryDetailsViewModel _ViewModel;

        #endregion


        #region ctor's

        public BSOFacilityInventoryLineOverview()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ViewModel = (FacilityInventoryDetailsViewModel)NavParam.Arguments;
            _ViewModel.Title = AppStrings.BFI_PositionOverview_Header;
            BindingContext = _ViewModel;
            this.PageState = PageStateEnum.View;
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PushAsync(new BSOFacilityInventoryLines() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.FacilityInventoryLinesViewModel } });
            return true;
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        #endregion

        #region Event handlers
        async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
        }
        private async void ItemEditCommand_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOFacilityInventoryLineEdit() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel } });
        }

        private async void BackToList_Clicked(object sender, EventArgs e)
        {
           await Navigation.PushAsync(new BSOFacilityInventoryLines() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.FacilityInventoryLinesViewModel } });
        }
        #endregion


    }
}