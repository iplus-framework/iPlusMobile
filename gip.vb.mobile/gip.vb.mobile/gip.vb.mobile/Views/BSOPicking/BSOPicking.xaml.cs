using gip.vb.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOPicking : BSOPageBase
    {
        private PickingViewModel _ViewModel;

        public BSOPicking()
        {
            BindingContext = _ViewModel = new PickingViewModel();
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ViewModel.LoadPickingTypesCommand.Execute(null);
        }

        private void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            _ViewModel.LoadPickingTypesCommand.Execute(null);
        }

        private void cmdClearPickingType_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedPickingType = null;
        }

        private void cmdClearFacilityFrom_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedStorageLocationFrom = null;
        }

        private void cmdClearFacilityTo_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedStorageLocationTo = null;
        }

        private async void btnShowOrders_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOPickingItems() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = inventory.FacilityInventoryNo } });
        }
    }
}