// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.facility;
using gip.mes.webservices;
using gip.vb.mobile.Helpers;
using gip.vb.mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    /// <summary>
    /// Selection of Materials for Outward-Booking (Input-Materials)
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOProdOrderOutwardMatSel : BSOPageBase
    {
        ProdOrderInMaterialsViewModel _ViewModel;
        BarcodeScanManuModel _FromTaskModel;

        public BSOProdOrderOutwardMatSel(ProdOrderPartslistPos targetPos, BarcodeScanManuModel taskModel, ACMethod wfMethod)
        {
            _WFMethod = wfMethod;
            //_OutwardPostingQSuggestion = outwardPostingQSuggestion;
            //InViewModel = inViewModel;
            _FromTaskModel = taskModel;
            BindingContext = _ViewModel = new ProdOrderInMaterialsViewModel(targetPos, taskModel);
            InitializeComponent();
        }

        private ACMethod _WFMethod;

        //private PostingSuggestionMode _OutwardPostingQSuggestion;
        //private ProdOrderInOutViewModel InViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
           _ViewModel.LoadProdOrderInMaterialsCommand.Execute(null);
        }


        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteLoadProdOrderInMaterialsCommand();
        }

        private async void ProdOrderInMaterials_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ProdOrderPartslistPosRelation rel = e.Item as ProdOrderPartslistPosRelation;
            if (rel == null)
                return;

            await Navigation.PushAsync(new BSOProdOrderOutward(rel, _FromTaskModel, _WFMethod, _ViewModel));
        }
    }
}