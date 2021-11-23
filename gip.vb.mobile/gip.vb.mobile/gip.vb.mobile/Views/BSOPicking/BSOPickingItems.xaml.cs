using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using gip.vb.mobile.ViewModels;
using gip.mes.webservices;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOPickingItems : BSOPageBase
    {
        PickingsViewModel _ViewModel;

        public BSOPickingItems()
        {
            BindingContext = _ViewModel = new PickingsViewModel();
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);
            InitializeComponent();
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