using gip.mes.webservices;
using gip.vb.mobile.Strings;
using gip.vb.mobile.ViewModels;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOFacilityInventoryLineEdit : BSOPageBase
    {
        #region Properties

        FacilityInventoryLineViewModel _ViewModel;

        #endregion

        #region ctor's

        public BSOFacilityInventoryLineEdit()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = _ViewModel = (FacilityInventoryLineViewModel)NavParam.Arguments;
            if (_ViewModel.MDFacilityInventoryPosStates == null)
                _ViewModel.GetListsCommand.Execute(null);
            _ViewModel.Title = AppStrings.BFI_PositionEdit_Header;
            this.PageState = PageStateEnum.Edit;
        }

        #endregion

        #region Event handlers

        private async void InventoryPosUpdate_Clicked(object sender, System.EventArgs e)
        {
            bool success = await _ViewModel.ExecuteUpdateFacilityInventoryPosAsync();
            if(success)
                await Navigation.PushAsync(new BSOFacilityInventoryLineOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel }});
        }

        private void TBItemRefresh_Clicked(object sender, System.EventArgs e)
        {

        }
        #endregion


    }
}