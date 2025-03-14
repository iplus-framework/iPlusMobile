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


namespace gip.vbm.mobile.Views
{
    /// <summary>
    /// Selection of Materials for Outward-Booking (Input-Materials)
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOMaterialUnitCalc : BSOPageBase
    {
        MaterialUnitCalcModel _ViewModel;

        public BSOMaterialUnitCalc(MaterialUnitCalcModel viewModel)
        {
            BindingContext = _ViewModel = viewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ViewModel.MaterialConvertAllUnitsCommand.Execute(null);
        }


        //private async void QuantTBItem_Clicked(object sender, EventArgs e)
        //{
        //    if (_ViewModel.SelectedAvailableFC != null)
        //    {
        //        await Navigation.PushAsync(new BSOFacilityChargeOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.SelectedAvailableFC } });
        //    }
        //}

        private async void btnSelectLot_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.SelectedUnitCalc != null || _ViewModel.UnitCalcs == null || !_ViewModel.UnitCalcs.Any())
            {
                if (_ViewModel.UnitReceiver != null && _ViewModel.SelectedUnitCalc != null)
                    _ViewModel.UnitReceiver.SetQuantityFromUnitRecalc(_ViewModel.SelectedUnitCalc.ResultValueInBase);
                await Navigation.PopAsync();
            }
        }

        private async void Units_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (_ViewModel.SelectedUnitCalc != null || _ViewModel.UnitCalcs == null || !_ViewModel.UnitCalcs.Any())
            {
                if (_ViewModel.UnitReceiver != null && _ViewModel.SelectedUnitCalc != null)
                    _ViewModel.UnitReceiver.SetQuantityFromUnitRecalc(_ViewModel.SelectedUnitCalc.ResultValueInBase);
                await Navigation.PopAsync();
            }
        }
    }
}