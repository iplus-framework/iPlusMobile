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
	public partial class BSOProdOrder : BSOPageBase
	{
        ProdOrderPartslistsViewModel _ViewModel;
        IBarcodeService _BarcodeService;
        bool _BarcodeServiceSubcribed;

        public BSOProdOrder()
		{
            BindingContext = _ViewModel = new ProdOrderPartslistsViewModel();
            _ViewModel.SetTitleFromType(this.GetType(), App.UserRights);
            InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_ViewModel.ProdOrderPartslists.Count == 0)
                _ViewModel.LoadProdOrderPartslistsCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteLoadProdOrderPartslistsCommand();
        }

        private async void ProdOrderPartslistsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ProdOrderPartslist item = e.Item as ProdOrderPartslist;
            if (item == null)
                return;

            await Navigation.PushAsync(new BSOProdOrderIntermediate(item, null));
        }

        private void ProdOrderPartslistsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        #region Barcode
        public void SubcribeToBarcodeService()
        {
            if (_BarcodeService == null)
                _BarcodeService = DependencyService.Get<IBarcodeService>();
            if (!_BarcodeServiceSubcribed)
            {
                _BarcodeService.Read += _BarcodeService_Read;
                _BarcodeServiceSubcribed = true;
            }
        }

        public void UnSubcribeToBarcodeService()
        {
            if (_BarcodeService != null && _BarcodeServiceSubcribed)
            {
                _BarcodeService.Read -= _BarcodeService_Read;
                _BarcodeServiceSubcribed = false;
            }
        }

        public async void _BarcodeService_Read(object sender, BarcodeReadEventArgs e)
        {
            if (e != null)
            {
                _ViewModel.CurrentBarcode = e.Text;
                if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode))
                {
                    ProdOrderPartslistPos result = await _ViewModel.ExecuteLoadBarcodeEntityCommand();
                    if (result != null)
                    {
                        _ViewModel.CurrentBarcode = null;
                        await Navigation.PushAsync(new BSOProdOrderInOutSelector(result, null, false, mes.facility.PostingQuantitySuggestionMode.OrderQuantity));
                    }
                }
            }
        }

        public async void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_ViewModel.CurrentBarcode))
            {
                ProdOrderPartslistPos result = await _ViewModel.ExecuteLoadBarcodeEntityCommand();
                if(result != null)
                {
                    _ViewModel.CurrentBarcode = null;
                    await Navigation.PushAsync(new BSOProdOrderInOutSelector(result, null, false, mes.facility.PostingQuantitySuggestionMode.OrderQuantity));
                }

            }
        }
        #endregion

        protected override bool OnBackButtonPressed()
        {
            ExitOnBackButtonPressed();
            return true;
        }
    }
}