using gip.vb.mobile.Strings;
using gip.vb.mobile.ViewModels;
using System;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOFacilityInventoryFilter : BSOPageBase
    {
        FacilityInventoryViewModel _ViewModel;

        #region ctor's
        public BSOFacilityInventoryFilter()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ViewModel = (FacilityInventoryViewModel)NavParam.Arguments;
            if (_ViewModel.MDFacilityInventoryStates.Count == 0)
            {
                _ViewModel.GetListsCommand.Execute(null);
            }
            _ViewModel.Title = AppStrings.Filter_Text + ": " + AppStrings.BFI_InventoryList_Header;
            BindingContext = _ViewModel;
            this.PageState = PageStateEnum.View;
        }

        #endregion

        #region Event handlers
        private async void ButtonInventorySearch_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(
                new BSOFacilityInventory()
                {
                    NavParam = new NavParameter(PageStateEnum.View)
                    {
                        Arguments = _ViewModel
                    }
                }
                );
        }
        #endregion

        private void TBItemRefresh_Clicked(object sender, EventArgs e)
        {

        }
    }
}