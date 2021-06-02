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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ViewModel.InventoryNavArgument = NavParam.Arguments as InventoryNavArgument;
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                _ViewModel.Title = AppStrings.Inv_EditLineS;
            else
                _ViewModel.Title = AppStrings.Inv_EditLine;
            if (_ViewModel.InventoryNavArgument.SelectedInventoryLine != null)
            {
                _ViewModel.SelectedInventoryLine = _ViewModel.InventoryNavArgument.SelectedInventoryLine;
                _ViewModel.IsEditPanelVisible = true;
            }
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
            _ViewModel.IsEditPanelVisible = false;
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                _ViewModel.Title = AppStrings.Inv_EditLineS;
        }

        private async void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_ViewModel.InputCode))
            {
                switch (_ViewModel.InventoryNavArgument.EditMode)
                {
                    case EditModeEnum.GoAndCount:
                        await _ViewModel.ExecuteGetFacilityInventorySearchCharge(EditModeEnum.GoAndCount);
                        break;
                    case EditModeEnum.Confirm:
                        await _ViewModel.ExecuteGetFacilityInventorySearchCharge(EditModeEnum.Confirm);
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

        
        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
                CleanUpForm();
        }

        private async void cmdQuantAdd_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteSetFacilityInventoryChargeAvailable();
        }

        private void cmdQuantEditAgain_Clicked(object sender, EventArgs e)
        {
            _ViewModel.IsEditPanelVisible = true;
        }

        private void BarcodeSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue) && _ViewModel.InventoryNavArgument.EditMode == EditModeEnum.GoAndCount)
            {
                _ViewModel.HideChargeCommandPanel();
                _ViewModel.IsEditPanelVisible = false;
            }
        }
        #endregion


    }
}