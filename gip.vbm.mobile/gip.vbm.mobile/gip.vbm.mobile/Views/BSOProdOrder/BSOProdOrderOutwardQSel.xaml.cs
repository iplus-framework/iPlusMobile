using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.facility;
using gip.mes.webservices;
using gip.vbm.mobile.Helpers;
using gip.vbm.mobile.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    /// <summary>
    /// Selection of Materials for Outward-Booking (Input-Materials)
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOProdOrderOutwardQSel : BSOPageBase
    {
        ProdOrderInOutViewModel _ViewModel;

        public BSOProdOrderOutwardQSel(ProdOrderInOutViewModel viewModel)
        {
            BindingContext = _ViewModel = viewModel;
            InitializeComponent();
        }

        private ACMethod _WFMethod;

        //private PostingSuggestionMode _OutwardPostingQSuggestion;
        //private ProdOrderInOutViewModel InViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _ViewModel.LoadAvailableFacilityChargesCommand.Execute(null);
        }


        private async void QuantTBItem_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.SelectedAvailableFC != null)
            {
                await Navigation.PushAsync(new BSOFacilityChargeOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.SelectedAvailableFC } });
            }
        }

        private async void btnSelectLot_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.SelectedAvailableFC != null)
            {
                _ViewModel.SetOutwardFacilityCharge(_ViewModel.SelectedAvailableFC, true);
                await Navigation.PopAsync();
            }
        }
    }
}