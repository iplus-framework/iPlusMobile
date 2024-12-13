// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using gip.vb.mobile.ViewModels;
using gip.mes.webservices;
using System.Collections.Generic;
using System.Linq;
using gip.vb.mobile.barcode;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOFacilityBookingHistory : BSOTabbedPageBase
    {
        FacilityBookingHistoryModel _ViewModel;

        public BSOFacilityBookingHistory(FacilityBookingHistoryModel viewModel)
        {
            BindingContext = this._ViewModel = viewModel;
            InitializeComponent();
        }

        public BSOFacilityBookingHistory()
        {
            _ViewModel = new FacilityBookingHistoryModel();
            BindingContext = _ViewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitPageOnNavigation();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void InitPageOnNavigation()
        {
            if (NavParam != null)
            {
                _ViewModel.Item = NavParam.Arguments;
            }
            this.PageState = PageStateEnum.View;
        }

        #region Booking

        private void ButtonFilter_Clicked(object sender, EventArgs e)
        {
            _ViewModel.ReadPostingsCommand.Execute(null);
            TabRoot.SelectedItem = BookingHistoryPivotItem;
        }
        #endregion
    }
}