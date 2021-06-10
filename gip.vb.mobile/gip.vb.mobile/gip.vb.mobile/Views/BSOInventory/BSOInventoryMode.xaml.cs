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
            _ViewModel.Title = AppStrings.Inv_SelectStorage;
            _ViewModel.FacilityInventoryNo = NavParam.Arguments.ToString();
            if (_ViewModel.AllFacilities == null || !_ViewModel.AllFacilities.Any())
                _ViewModel.GetFacilitiesCommand.Execute(null);

            // Reomove all helping pages
            var _navigation = Application.Current.MainPage.Navigation;
            string[] helperNames = new string[] { "BSOInventoryLineEdit", "BSOInventoryLines" };
            foreach (var helperName in helperNames)
            {
                var item = _navigation.NavigationStack.Where(c => c.GetType().Name.Contains(helperName)).FirstOrDefault();
                if (item != null)
                    _navigation.RemovePage(item);
            }
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
        private void cmdClearStorageLocation_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedStorageLocation = null;
        }

        private void cmdClearFacility_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedFacility = null;
        }


        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            barcodeScanner.CleanUpForm();
            barcodeScanner._ViewModel.ZXingIsScanning = true;
        }

        private void barcodeScanner_OnSendSelectedCode(object sender, EventArgs e)
        {
            if (barcodeScanner._ViewModel.CurrentBarcodeEntity != null)
            {
                Facility facility = barcodeScanner._ViewModel.CurrentBarcodeEntity.FirstOrDefault() as Facility;
                if (facility != null )
                {
                    if (facility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageLocation)
                    {
                        _ViewModel.SelectedStorageLocation = _ViewModel.StorageLocations.FirstOrDefault(c=>c.FacilityNo == facility.FacilityNo);
                    }
                    else if(facility.ParentFacility != null && facility.ParentFacility.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageLocation)
                    {
                        _ViewModel.SelectedStorageLocation = _ViewModel.StorageLocations.FirstOrDefault(c=>c.FacilityNo == facility.ParentFacility.FacilityNo);
                        _ViewModel.SelectedFacility = _ViewModel.Facilities.FirstOrDefault(c=>c.FacilityNo == facility.FacilityNo);
                    }
                    barcodeScanner.CleanUpForm();
                }
                else
                    _ViewModel.Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Error, AppStrings.SelectTargetFacility_Text);
            }
        }

        #endregion


    }
}