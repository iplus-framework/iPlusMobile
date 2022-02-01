using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using gip.vb.mobile.ViewModels;
using gip.mes.webservices;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOPickingItemsWorkplace : BSOPageBase
    {
        PickingsWorkplaceViewModel _ViewModel;

        public BSOPickingItemsWorkplace()
        {
            BindingContext = _ViewModel = new PickingsWorkplaceViewModel();
            _ViewModel.SetTitleFromType(typeof(BSOPickingItemsWorkplace), App.UserRights);
            InitializeComponent();
        }

        private void InitPageOnNavigation()
        {
            if (NavParam != null)
            {
                PickingWorkplaceViewModel viewModel = NavParam.Arguments as PickingWorkplaceViewModel;
                if (viewModel != null)
                {
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

            await Navigation.PushAsync(new BSOPickingDetail() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = picking } });

            PickingsListView.SelectedItem = null;

        }

        private async void TBQuantActivation_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOPickingWorkplaceActivationItems() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel } });
        }
    }
}