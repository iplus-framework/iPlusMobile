using gip.vb.mobile.Strings;
using gip.vb.mobile.ViewModels;
using System;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOFacilityInventoryLinesFilter : BSOPageBase
    {
        #region Properteis
        FacilityInventoryLinesViewModel _ViewModel;
        #endregion

        #region ctor's
        public BSOFacilityInventoryLinesFilter()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ViewModel = (FacilityInventoryLinesViewModel)NavParam.Arguments;
            if (_ViewModel.MDFacilityInventoryPosStates == null)
            {
                _ViewModel.GetListsCommand.Execute(null);
            }
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);
            BindingContext = _ViewModel;
            this.PageState = PageStateEnum.View;
        }
        #endregion

        #region Event handlers
        private async void InventoryPosSearch_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOFacilityInventoryLines() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel } });
        }

        async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {

        }

        private void MoreFilterCommand_Clicked(object sender, EventArgs e)
        {
            FilterMore.IsVisible = !FilterMore.IsVisible;
            MoreFilterCommand.Text = FilterMore.IsVisible ? AppStrings.BtnLes_Text : AppStrings.BtnMore_Text;
        }

        #endregion

    }
}