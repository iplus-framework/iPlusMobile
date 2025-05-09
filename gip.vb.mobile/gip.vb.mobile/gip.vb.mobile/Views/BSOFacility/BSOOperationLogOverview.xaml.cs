﻿using gip.core.webservices;
using gip.mes.webservices;
using gip.vb.mobile.barcode;
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
	public partial class BSOOperationLogOverview : BSOPageBase
    {

        public BSOOperationLogOverview()
		{
            BindingContext = _ViewModel = new OperationLogOverviewModel();
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
                _ViewModel.Item = NavParam.Arguments as ACClass;
            _ViewModel.LoadOperationLogOverviewCommand.Execute(null);
            this.PageState = PageStateEnum.View;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        #region Properties
        OperationLogOverviewModel _ViewModel;
        #endregion

        #region Methods

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