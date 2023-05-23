using gip.core.datamodel;
using gip.mes.webservices;
using gip.vbm.mobile.barcode;
using gip.vbm.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using gip.vbm.mobile.Controls;

namespace gip.vbm.mobile.Views
{
    /// <summary>
    /// Inward-Booking (Output of Production; Ergebnis)
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOProdOrderInward : BSOProdOrderInOutBase
    {
        BarcodeScanManuModel _FromTaskModel;

        protected override BarcodeScannerView BarcodeScanner
        {
            get
            {
                return barcodeScanner;
            }
        }

        public BSOProdOrderInward(ProdOrderPartslistPos intermOrIntermBatch, BarcodeScanManuModel taskModel, ACMethod wfMethod)
        {
            _FromTaskModel = taskModel;
            BindingContext = _ViewModel = new ProdOrderInOutViewModel(true, taskModel, null, intermOrIntermBatch, wfMethod);
            InitializeComponent();
        }

        public void ButtonDoBooking_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.CurrentFacility == null)
            {
                _ViewModel.Message = new core.datamodel.Msg() { Message = Strings.AppStrings.SelectTargetFacilityLocationOrBin_Text };
                return;
            }
            if (_FromTaskModel != null && _FromTaskModel.ScannedMachine != null)
                _ViewModel.PropertyACUrl = ACUrlHelper.GetParentACUrl(_FromTaskModel.ScannedMachine.ACClass.ACUrlComponent);
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

        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            await BarcodeScanner.OpenBarcodeCamera();
        }

        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.RefreshInItems();
        }

        protected async override void OnAppearing()
        {
            await _ViewModel.LoadTargetFacilities();
            await _ViewModel.RefreshInItems();
            _ViewModel.LoadMovementReasons();
        }

        private void cmdClearMovementReason_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedMovementReason = null;
        }
    }
}