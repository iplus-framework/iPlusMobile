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
            if (_ViewModel.EditMode == EditModeEnum.GoAndCount)
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
            object[] parameters = NavParam.Arguments as object[];
            _ViewModel.FacilityInventoryNo = parameters[0].ToString();
            _ViewModel.EditMode = (EditModeEnum)parameters[1];
            if (parameters[2] != null)
                _ViewModel.SelectedStorageLocation = parameters[2] as Facility;
            if (parameters[3] != null)
                _ViewModel.SelectedFacility = parameters[3] as Facility;
            if (parameters[4] != null)
            {
                _ViewModel.SelectedInventoryLine = parameters[4] as FacilityInventoryPos;
                spEditor.IsVisible = true;
            }

            BarcodeSearchBar.Placeholder = _ViewModel.EditMode == EditModeEnum.GoAndCount ? AppStrings.FC_Scan_Go : AppStrings.FC_Scan_Check;
        }

        private async void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_ViewModel.InputCode))
            {
                switch (_ViewModel.EditMode)
                {
                    case EditModeEnum.GoAndCount:
                        bool success = await _ViewModel.ExecuteGetFacilityInventoryPosesAsync();
                        if (success)
                        {
                            if (
                                _ViewModel.SelectedInventoryLine != null 
                                    &&!_ViewModel.SelectedInventoryLine.NotAvailable 
                                    && _ViewModel.SelectedInventoryLine.NewStockQuantity == null)
                                _ViewModel.SelectedInventoryLine.NewStockQuantity = _ViewModel.SelectedInventoryLine.StockQuantity;
                            spEditor.IsVisible = true;
                        }
                        break;
                    case EditModeEnum.Confirm:
                        await _ViewModel.ExecuteGetFacilityInventoryPosesAsync();
                        break;
                }
            }
        }

        private async void cmdUpdate_Clicked(object sender, EventArgs e)
        {
            bool succes = await _ViewModel.ExecuteUpdateFacilityInventoryPosAsync();
            if (succes)
            {
                CleanUpForm();
            }
            switch (_ViewModel.EditMode)
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
                                Arguments = new object[]
                                {
                                    _ViewModel.FacilityInventoryNo,
                                    _ViewModel.EditMode,
                                    _ViewModel.SelectedStorageLocation,
                                    _ViewModel.SelectedFacility
                                }
                            }
                        });
                    break;
            }
        }

        #endregion

    }
}