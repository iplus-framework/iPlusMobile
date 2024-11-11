// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
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