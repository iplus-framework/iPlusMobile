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
	public partial class BSOFacilityChargeOverview : BSOTabbedPageBase
    {
        public BSOFacilityChargeOverview()
		{
            BindingContext = _ViewModel = new FacilityChargesViewModel();
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
            //UnSubcribeToBarcodeService();
        }

        private void InitPageOnNavigation()
        {
            //SubcribeToBarcodeService();
            //if (_ViewModel.Locations.Count == 0)
            //    _ViewModel.LoadLocationsCommand.Execute(null);

            _ViewModel.OnAppear(NavParam?.Arguments as FacilityCharge);

            this.PageState = PageStateEnum.View;
        }

        #region Properties
        FacilityChargesViewModel _ViewModel;
        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;
        #endregion

        #region Methods
        private async void TBItemHistory_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOFacilityBookingHistory() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.Item } });
        }

        #region Barcode
        //private void SubcribeToBarcodeService()
        //{
        //    if (_BarcodeService == null)
        //        _BarcodeService = DependencyService.Get<IBarcodeService>();
        //    if (!_BarcodeServiceSubcribed)
        //    {
        //        _BarcodeService.Read += _BarcodeService_Read;
        //        _BarcodeServiceSubcribed = true;
        //    }
        //}

        //private void UnSubcribeToBarcodeService()
        //{
        //    if (_BarcodeService != null && _BarcodeServiceSubcribed)
        //    {
        //        _BarcodeService.Read -= _BarcodeService_Read;
        //        _BarcodeServiceSubcribed = false;
        //    }
        //}

        //private void _BarcodeService_Read(object sender, BarcodeReadEventArgs e)
        //{
        //    if (e != null)
        //    {
        //        _ViewModel.CurrentBarcode = e.Text;
        //        if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode))
        //            _ViewModel.LoadBarcodeEntityCommand.Execute(null);
        //    }
        //}

        //private void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        //{
        //    if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode))
        //        _ViewModel.LoadBarcodeEntityCommand.Execute(null);
        //}
        #endregion

        #region Facility
        //private void FacilitySearchBar_TextChanged(object sender, TextChangedEventArgs e)
        //{

        //}

        ////private void FacilitySearchBar_SearchButtonPressed(object sender, EventArgs e)
        ////{
        ////    if (String.IsNullOrEmpty(_ViewModel.FacilitySearch) && _ViewModel.SelectedLocation != null)
        ////        return;
        ////    _ViewModel.LoadFilteredFacilitiesCommand.Execute(null);
        ////}

        //private void FacilityListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{

        //}

        //private void FacilityListView_ItemTapped(object sender, ItemTappedEventArgs e)
        //{

        //}
        #endregion

        private void ButtonDoBooking_Clicked(object sender, EventArgs e)
        {
            if (sender == ButtonDoBooking)
            {
                _ViewModel.BookFacilityCommand.Execute(null);
            }
            else if (sender == ButtonDoZeroStock)
            {
                _ViewModel.BookZeroStockCommand.Execute(null);
            }
            else if (sender == ButtonUnDoZeroStock)
            {
                _ViewModel.BookUnZeroStockCommand.Execute(null);
            }
            else if (sender == ButtonBlockQuant)
            {
                _ViewModel.BookBlockQuantCommand.Execute(null);
            }
            else if (sender == ButtonReleaseQuant)
            {
                _ViewModel.BookReleaseQuantCommand.Execute(null);
            }
            else if (sender == ButtonRelocate)
            {
                _ViewModel.BookRelocateCommand.Execute(null);
            }
        }

        private void Print_Clicked(object sender, EventArgs e)
        {
            _ViewModel.PrintCommand.Execute(null);
        }


        #endregion

        private async void FacilityEntry_Focused(object sender, FocusEventArgs e)
        {
            _ViewModel.FacilitySelector = new FacilitySelectorViewModel("");
            await Navigation.PushModalAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));

            FacilityEntry.Unfocus();
        }

        private void cmdClearFacility_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedFacility = null;
        }
    }
}