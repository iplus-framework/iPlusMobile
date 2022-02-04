using gip.mes.webservices;
using gip.vb.mobile.barcode;
using gip.vb.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.vb.mobile.Controls;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOFacilityChargeOverview : BSOTabbedPageBase
    {
        #region c'tors

        public BSOFacilityChargeOverview()
		{
            BindingContext = _ViewModel = new FacilityChargesViewModel();
            barcodeScanner = new BarcodeScanner();
            barcodeScanner.IsEnabledInvokeBarcodeOnServer = true;
            InitializeComponent();
        }

        #endregion

        #region Properties

        FacilityChargesViewModel _ViewModel;

        BarcodeScanner barcodeScanner;

        #endregion

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();
            barcodeScanner._ViewModel = _ViewModel.FacilityScanViewModel;
            barcodeScanner.OnAppearing();
            InitPageOnNavigation();
        }

        protected override void OnDisappearing()
        {
            barcodeScanner.OnDisappearing();
            base.OnDisappearing();
        }

        private void InitPageOnNavigation()
        {
            _ViewModel.OnAppear(NavParam?.Arguments as FacilityCharge);
            this.PageState = PageStateEnum.View;
        }

        private async void TBItemHistory_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOFacilityBookingHistory() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.Item } });
        }

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
            //_ViewModel.PrintCommand.Execute(null);
            _ViewModel.ShowPrintDialog();
        }

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

        private void ButtonSplit_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SplitQuantCommand.Execute(null);
        }

        private async void cmdSumByScan_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOSumQuantityByBarcode(_ViewModel.GetSumByBarcodeModel()));
        }

        private void cmdClearBookingQuantity_Clicked(object sender, EventArgs e)
        {
            BookingQuantity.Text = null;
        }

        private void cmdClearMovementReason_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedMovementReason = null;
        }

        #endregion
    }
}