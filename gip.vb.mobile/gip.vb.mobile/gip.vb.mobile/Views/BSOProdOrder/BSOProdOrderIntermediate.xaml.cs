// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.vb.mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOProdOrderIntermediate : BSOPageBase
	{
        ProdOrderPLIntermediatesViewModel _ViewModel;
        BarcodeScanManuModel _FromTaskModel;
        public BSOProdOrderIntermediate(ProdOrderPartslist prodOrderPartslist, BarcodeScanManuModel taskModel)
		{
            _FromTaskModel = taskModel;
            BindingContext = _ViewModel = new ProdOrderPLIntermediatesViewModel(prodOrderPartslist);
            _ViewModel.Title = prodOrderPartslist.Partslist.PartslistNo;
            InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_ViewModel.ProdOrderPLIntermediates.Count == 0)
                _ViewModel.LoadProdOrderPLIntermediatesCommand.Execute(null);
        }

        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteLoadProdOrderPLIntermediatesCommand();
        }

        private async void ProdOrderPLIntermediates_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ProdOrderPartslistPos item = e.Item as ProdOrderPartslistPos;
            if (item == null)
                return;

            ProdOrderPartslistWFInfo wfInfo = null;
            if (_FromTaskModel != null)
                wfInfo = _FromTaskModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo != null && wfInfo.IntermediateBatch != null && wfInfo.MaterialWFConnectionMode == 10)
            {
                item.ProdOrderPartslist = wfInfo.Intermediate.ProdOrderPartslist;
                await Navigation.PushAsync(new BSOProdOrderInOutSelector(item, _FromTaskModel, null));
            }
            else
                await Navigation.PushAsync(new BSOProdOrderBatch(item, _FromTaskModel));
        }
    }
}