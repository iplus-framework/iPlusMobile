using gip.mes.webservices;
using gip.vb.mobile.barcode;
using gip.vb.mobile.Strings;
using gip.vb.mobile.ViewModels;
using gip.vb.mobile.ViewModels.Inventory;
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
    public partial class BSOInventoryLineEdit : BSOPageBase
    {
        InventoryLineEditModel _ViewModel;

        #region ctor's
        public BSOInventoryLineEdit()
        {
            _ViewModel = new InventoryLineEditModel();
            BindingContext = _ViewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            barcodeScanner.OnAppearing();
            _ViewModel.InventoryNavArgument = NavParam.Arguments as InventoryNavArgument;
            _ViewModel.BarcodeScannerModel = barcodeScanner._ViewModel;
            _ViewModel.CleanAndSetFacility();
            _ViewModel.Start();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            barcodeScanner.OnDisappearing();
        }

        #endregion

        #region Event

        private void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                _ViewModel.CleanUpForm();
        }

        private void cmdUpdate_Clicked(object sender, EventArgs e)
        {
            _ViewModel.UpdateFacilityInventoryPosCommand.Execute(null);
            if (_ViewModel.WSResponse.Suceeded)
            {
                _ViewModel.CleanUpForm();
                _ViewModel.CleanAndSetFacility();
                switch (_ViewModel.InventoryNavArgument.EditMode)
                {
                    case EditModeEnum.GoAndCount:
                        // nothing stay there
                        break;
                    case EditModeEnum.Confirm:
                        // back to list
                        //await Navigation.PushAsync(
                        //    new BSOInventoryLines()
                        //    {
                        //        NavParam = new NavParameter(PageStateEnum.View)
                        //        {
                        //            Arguments = _ViewModel.InventoryNavArgument
                        //        }
                        //    });
                        Navigation.RemovePage(this);
                        break;
                }
            }
        }

        private void cmdQuantEditAgain_Clicked(object sender, EventArgs e)
        {
            _ViewModel.IsEditPanelVisible = true;
        }

        #endregion

        #region Barcode Scanner events

        private void barcodeScanner_OnSendSelectedCode(object sender, EventArgs e)
        {
            _ViewModel.GetFacilityInventorySearchChargeCommand.Execute(null);
        }

        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            barcodeScanner._ViewModel.ZXingIsScanning = true;
        }

        #endregion

    }
}