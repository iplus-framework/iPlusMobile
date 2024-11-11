// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.webservices;
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
	public partial class BSOFacilityMaterialOverview : BSOTabbedPageBase
    {

        public BSOFacilityMaterialOverview()
		{
            BindingContext = _ViewModel = new MaterialOverviewModel();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitPageOnNavigation();
        }

        private void InitPageOnNavigation()
        {
            string preSelectTab = null;
            if (NavParam != null)
            {
                Tuple<Material, string> argument = NavParam.Arguments as Tuple<Material, string>;
                if (argument != null)
                {
                    _ViewModel.Item = argument.Item1;
                    preSelectTab = argument.Item2;
                }
                else
                {
                    _ViewModel.Item = NavParam.Arguments as Material;
                }
            }

            _ViewModel.LoadMaterialSumOverviewCommand.Execute(null);

            if (!string.IsNullOrEmpty(preSelectTab))
            {
                BSOPageBase tab = this.FindByName(preSelectTab) as BSOPageBase;
                if (tab != null)
                    SelectedItem = tab;
            }

            var test = QuantDataGrid.SelectionEnabled;
            this.PageState = PageStateEnum.View;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        #region Properties
        MaterialOverviewModel _ViewModel;
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

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BSOFacilitySortInfoOverLocation());
        }
    }
}