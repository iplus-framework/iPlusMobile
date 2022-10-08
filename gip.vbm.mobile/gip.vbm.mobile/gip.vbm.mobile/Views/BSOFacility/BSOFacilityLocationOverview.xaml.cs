using gip.mes.webservices;
using gip.vbm.mobile.barcode;
using gip.vbm.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOFacilityLocationOverview : BSOTabbedPageBase
    {

        public BSOFacilityLocationOverview()
		{
            BindingContext = _ViewModel = new FacilityLocationOverviewModel();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitPageOnNavigation();
        }

        private void InitPageOnNavigation()
        {
            if (NavParam != null)
                _ViewModel.Item = NavParam.Arguments as Facility;
            _ViewModel.LoadFacilityLocationSumOverviewCommand.Execute(null);
            this.PageState = PageStateEnum.View;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        #region Properties
        FacilityLocationOverviewModel _ViewModel;
        #endregion

        #region Methods
        private void Pivot_SelectionChanged(object sender, EventArgs e)
        {
        }

        private async void TBItemHistory_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOFacilityBookingHistory() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.Item } });
        }

        private async void TBItemQuant_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.SelectedFacilityCharge == null)
                return;
            await Navigation.PushAsync(new BSOFacilityChargeOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.SelectedFacilityCharge } });
        }
        #endregion
    }
}