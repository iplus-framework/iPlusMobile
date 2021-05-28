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
    public partial class BSOInventoryMode : BSOPageBase
    {
        InventoryModeModel _ViewModel;
        public BSOInventoryMode()
        {
            _ViewModel = new InventoryModeModel();
            BindingContext = _ViewModel;
            InitializeComponent();
        }

        #region Event handlers

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ViewModel.FacilityInventoryNo = NavParam.Arguments.ToString();
            if (_ViewModel.AllFacilities == null || !_ViewModel.AllFacilities.Any())
                _ViewModel.GetFacilitiesCommand.Execute(null);
        }

        async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteGetFacilities();
        }

        private async void GoAndCount_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(
                new BSOInventoryLineEdit() 
                { 
                    NavParam = new NavParameter(PageStateEnum.View) 
                    {
                        Arguments = new InventoryNavArgument()
                        {
                            StorageLocationNo = _ViewModel.SelectedStorageLocation.FacilityNo,
                            FacilityInventoryNo = _ViewModel.FacilityInventoryNo,
                            EditMode = EditModeEnum.GoAndCount,
                            IsValidateAndComplete = _ViewModel.IsValidateAndComplete,
                            SelectedFacility = _ViewModel.SelectedFacility,
                            SelectedStorageLocation = _ViewModel.SelectedStorageLocation
                        }
                    } 
                });
        }

        private async void ViewOpenLines_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(
                new BSOInventoryLines() 
                { 
                    NavParam = new NavParameter(PageStateEnum.View) 
                    { 
                        Arguments = new InventoryNavArgument()
                        {
                            StorageLocationNo = _ViewModel.SelectedStorageLocation.FacilityNo,
                            FacilityInventoryNo = _ViewModel.FacilityInventoryNo,
                            EditMode = EditModeEnum.Confirm,
                            IsValidateAndComplete = _ViewModel.IsValidateAndComplete,
                            SelectedFacility = _ViewModel.SelectedFacility,
                            SelectedStorageLocation = _ViewModel.SelectedStorageLocation
                        }
                    } 
                });
        }

        #endregion

    }
}