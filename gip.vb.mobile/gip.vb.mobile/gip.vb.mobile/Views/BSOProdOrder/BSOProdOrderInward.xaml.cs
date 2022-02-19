﻿using gip.mes.webservices;
using gip.vb.mobile.barcode;
using gip.vb.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.DataGrid;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    /// <summary>
    /// Inward-Booking (Output of Production; Ergebnis)
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOProdOrderInward : BSOProdOrderInOutBase
    {
        public BSOProdOrderInward(ProdOrderPartslistPos intermOrIntermBatch)
        {
            BindingContext = _ViewModel = new ProdOrderInOutViewModel(true, null, intermOrIntermBatch);
            InitializeComponent();
        }

        public void ButtonDoBooking_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.CurrentFacility == null)
            {
                _ViewModel.Message = new core.datamodel.Msg() { Message = Strings.AppStrings.SelectTargetFacilityLocationOrBin_Text };
                return;
            }
            _ViewModel.BookFacilityCommand.Execute(null);
        }

        private void TabPrint_Clicked(object sender, EventArgs e)
        {
            if (CurrentPage == ProdOrderInOutHistory)
            {
                _ViewModel.Print(Strings.AppStrings.PrintCopies_Question);
            }
        }

        private void BSOTabbedPageBase_CurrentPageChanged(object sender, EventArgs e)
        {
            if (CurrentPage == ProdOrderInOutHistory)
            {
                TBPrint.IsEnabled = true;
            }
            else
            {
                TBPrint.IsEnabled = false;
            }
        }


        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.RefreshInItems();
        }

        protected async override void OnAppearing()
        {
            await _ViewModel.LoadTargetFacilities();
            await _ViewModel.RefreshInItems();
        }
    }
}