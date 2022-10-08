using System;
using gip.vbm.mobile.ViewModels;
using gip.mes.webservices;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOPickingItemsWorkplace : BSOPageBase
    {
        PickingsWorkplaceViewModel _ViewModel;

        public BSOPickingItemsWorkplace()
        {
            BindingContext = _ViewModel = new PickingsWorkplaceViewModel();
            _ViewModel.Title = Strings.AppStrings.PickingOrders_Text;
            InitializeComponent();
        }

        private void InitPageOnNavigation()
        {
            if (NavParam != null)
            {
                PickingWorkplaceViewModel viewModel = NavParam.Arguments as PickingWorkplaceViewModel;
                if (viewModel != null)
                {
                    _ViewModel.PickingFilter = viewModel;
                    _ViewModel.RegisteredWorkplace = viewModel.RegisteredWorkplace ;
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

            await Navigation.PushAsync(new BSOPickingDetail() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = new List<object>() { picking, _ViewModel.RegisteredWorkplace } } });

            PickingsListView.SelectedItem = null;

        }

        private async void TBQuantActivation_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOPickingWorkplaceActivationItems() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel } });
        }

        private void cmdClearGroupItem_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedGroupItem = null;
        }
    }
}