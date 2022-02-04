using gip.core.webservices;
using gip.mes.webservices;
using gip.vb.mobile.Controls;
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
    public partial class BSOPickingWorkplaceActivationItems : BSOPageBase
    {
        public BSOPickingWorkplaceActivationItems()
        {
            BindingContext = _ViewModel = new PickingWorkplaceActItemsViewModel();
            BarcodeScan = new BarcodeScanner();
            BarcodeScan._ViewModel = _ViewModel.BarcodeScanModel;
            BarcodeScan.IsEnabledInvokeBarcodeOnServer = true;
            Title = Strings.AppStrings.QuantActivationDeactivation_Text;
            InitializeComponent();
        }

        PickingWorkplaceActItemsViewModel _ViewModel;

        public BarcodeScanner BarcodeScan
        {
            get;
            private set;
        }

        protected override void OnAppearing()
        {
            BarcodeScan.OnBarcodeCommandInvoked += BarcodeScan_OnBarcodeCommandInvoked;
            BarcodeScan.OnAppearing();
            base.OnAppearing();
            InitPageOnNavigation();
        }

        private void BarcodeScan_OnBarcodeCommandInvoked(object sender, EventArgs e)
        {
            _ViewModel.ActivateDeactivateQuantCommnd.Execute(null);
        }

        protected override void OnDisappearing()
        {
            BarcodeScan.OnBarcodeCommandInvoked -= BarcodeScan_OnBarcodeCommandInvoked;
            BarcodeScan.OnDisappearing();
            base.OnDisappearing();
        }

        private void InitPageOnNavigation()
        {
            if (NavParam != null)
            {
                PickingsWorkplaceViewModel viewModel = NavParam.Arguments as PickingsWorkplaceViewModel;
                if (viewModel != null)
                {
                    _ViewModel.PickingsWorkplaceModel = viewModel;
                }
            }
            this.PageState = PageStateEnum.View;
        }

        private async void FacilityChargeActivationListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var quant = e.Item as FacilityCharge;
            if (quant == null)
                return;

            await Navigation.PushAsync(new BSOPickingWorkplaceActivation() 
                                           { NavParam = new NavParameter(PageStateEnum.View) 
                                                           { Arguments = new object[] { quant, _ViewModel.PickingsWorkplaceModel?.RegisteredWorkplace } } 
                                           });
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            BarcodeScan.Search(sbActivateDeactivateQuant.Text);
        }
    }
}