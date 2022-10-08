using System;
using gip.vbm.mobile.ViewModels;
using gip.mes.webservices;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOPickingItems : BSOPageBase
    {
        PickingsViewModel _ViewModel;

        public BSOPickingItems()
        {
            BindingContext = _ViewModel = new PickingsViewModel();
            _ViewModel.SetTitleFromType(typeof(BSOPicking), App.UserRights);
            InitializeComponent();
        }

        private void InitPageOnNavigation()
        {
            if (NavParam != null)
            {
                PickingViewModel filter = NavParam.Arguments as PickingViewModel;
                if (filter != null)
                {
                    _ViewModel.PickingFilter = filter;
                }
            }
            this.PageState = PageStateEnum.View;
        }

        void OnPickingSelected(object sender, SelectedItemChangedEventArgs args)
        {
        }

        async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteLoadPickingsCommand();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitPageOnNavigation();

            if (_ViewModel.Pickings.Count == 0)
                _ViewModel.LoadPickingsCommand.Execute(null);
        }

        private async void PickingsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var picking = e.Item as Picking;
            if (picking == null)
                return;

            await Navigation.PushAsync(new BSOPickingDetail() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = picking } });

            PickingsListView.SelectedItem = null;

        }
    }
}