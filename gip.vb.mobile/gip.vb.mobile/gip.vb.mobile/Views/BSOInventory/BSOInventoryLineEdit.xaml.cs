using gip.mes.webservices;
using gip.vb.mobile.Strings;
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

        #endregion

        #region Event


        private void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                CleanUpForm();
        }

        public void CleanUpForm()
        {
            _ViewModel.InputCode = "";
            _ViewModel.SelectedInventoryLine = null;
            spEditor.IsVisible = false;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            _ViewModel.InventoryNavArgument = NavParam.Arguments as InventoryNavArgument;
            if (_ViewModel.InventoryNavArgument.SelectedInventoryLine != null)
            {
                _ViewModel.SelectedInventoryLine = _ViewModel.InventoryNavArgument.SelectedInventoryLine;
                WriteNewStockQuantity();
                spEditor.IsVisible = true;
            }

            BarcodeSearchBar.Placeholder = _ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount ? AppStrings.FC_Scan_Go : AppStrings.FC_Scan_Check;
        }

        private async void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            bool success = false;
            switch (_ViewModel.InventoryNavArgument.EditMode)
            {
                case EditModeEnum.GoAndCount:
                    if (!string.IsNullOrEmpty(_ViewModel.InputCode))
                        success = await _ViewModel.ExecuteGetFacilityInventoryLinesAsync();
                    if (success)
                    {
                        WriteNewStockQuantity();
                        spEditor.IsVisible = true;
                    }
                    break;
                case EditModeEnum.Confirm:
                    success = await _ViewModel.ExecuteGetFacilityInventoryLinesAsync();
                    break;
            }
        }

        private async void cmdUpdate_Clicked(object sender, EventArgs e)
        {
            bool succes = await _ViewModel.ExecuteUpdateFacilityInventoryPosAsync();
            if (succes)
            {
                CleanUpForm();
            }
            switch (_ViewModel.InventoryNavArgument.EditMode)
            {
                case EditModeEnum.GoAndCount:
                    // nothing stay there
                    break;
                case EditModeEnum.Confirm:
                    // back to list
                    await Navigation.PushAsync(
                        new BSOInventoryLines()
                        {
                            NavParam = new NavParameter(PageStateEnum.View)
                            {
                                Arguments = _ViewModel.InventoryNavArgument
                            }
                        });
                    break;
            }
        }

        public void WriteNewStockQuantity()
        {
            if (_ViewModel.SelectedInventoryLine != null
                                && !_ViewModel.SelectedInventoryLine.NotAvailable
                                && _ViewModel.SelectedInventoryLine.NewStockQuantity == null)
                _ViewModel.SelectedInventoryLine.NewStockQuantity = _ViewModel.SelectedInventoryLine.StockQuantity;
        }
        #endregion

        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                CleanUpForm();
        }
    }
}