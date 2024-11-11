// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.webservices;
using gip.vb.mobile.Helpers;
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
	public partial class BSOProdOrderInOutSelector : BSOPageBase
	{
        BarcodeScanManuModel _FromTaskModel;
        public BSOProdOrderInOutSelector(ProdOrderPartslistPos batch, BarcodeScanManuModel taskModel, ACMethod wfMethod)
		{
            //_InwardPostingQSuggestion = inwardPostingQSuggestion;
            //_OutwardPostingQSuggestion = outwardPostingQSuggestion;

            _WFMethod = wfMethod;
            _FromTaskModel = taskModel;
            IntermOrIntermBatch = batch;
            //Intermediate = intermediate;
            BindingContext = this;
            InitializeComponent();
		}

        //private bool _InwardPostingQSuggestion;
        //private PostingSuggestionMode _OutwardPostingQSuggestion;

        private ACMethod _WFMethod;

        public new string Title
        {
            get => Strings.AppStrings.ProdOrderInOutBooking_Header;
        }

        public ProdOrderPartslistPos IntermOrIntermBatch
        {
            get;
            set;
        }

        //public ProdOrderPartslistPos Intermediate
        //{
        //    get;
        //    set;
        //}

        private async void BtnInput_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOProdOrderOutwardMatSel(IntermOrIntermBatch, _FromTaskModel, _WFMethod));
        }

        private async void BtnOutput_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOProdOrderInward(IntermOrIntermBatch, _FromTaskModel, _WFMethod));
        }
    }
}