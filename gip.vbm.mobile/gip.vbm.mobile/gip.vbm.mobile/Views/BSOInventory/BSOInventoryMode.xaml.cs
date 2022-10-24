using gip.mes.webservices;
using gip.mes.datamodel;
using gip.vbm.mobile.Strings;
using gip.vbm.mobile.ViewModels.Inventory;
using System;
using System.Linq;
using gip.vbm.mobile.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOInventoryMode : BSOPageBase
    {

        InventoryModeModel _ViewModel;

        #region ctor's

        public BSOInventoryMode()
        {
            _ViewModel = new InventoryModeModel();
            BindingContext = _ViewModel;
            InitializeComponent();
        }
        #endregion

        #region Methods 

        #region Methods -> Lifecycle

        /// <summary>
        /// OnAppering - prepare list of facilites and clean up navigation loops
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
 
            _ViewModel.Title = AppStrings.Inv_SelectStorage;

            FacilityInventory inv = NavParam.Arguments as FacilityInventory;

            if (inv != null)
            {
                _ViewModel.FacilityInventory = inv;
                _ViewModel.FacilityInventoryNo = inv.FacilityInventoryNo;
                _ViewModel.FacilityInventoryID = inv.FacilityInventoryID;
            }

            _ViewModel.OnAppear();

            // Reomove all helping pages
            var _navigation = Application.Current.MainPage.Navigation;

            #region Clean up navigation history
            string[] helperNames = new string[] { "BSOInventoryLineEdit", "BSOInventoryLines" };
            foreach (var helperName in helperNames)
            {
                var item = _navigation.NavigationStack.Where(c => c.GetType().Name.Contains(helperName)).FirstOrDefault();
                if (item != null)
                    _navigation.RemovePage(item);
            }
            #endregion
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //barcodeScanner.OnDisappearing();
        }
        #endregion

        #region Methods -> Button 
        /// <summary>
        /// Hanle go and count mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GoAndCount_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(
                new BSOInventoryLineEdit()
                {
                    NavParam = new NavParameter(PageStateEnum.View)
                    {
                        Arguments = new InventoryNavArgument()
                        {
                            StorageLocationNo = _ViewModel.SelectedStorageLocation != null ? _ViewModel.SelectedStorageLocation.FacilityNo : null,
                            FacilityInventoryNo = _ViewModel.FacilityInventoryNo,
                            FacilityInventoryID = _ViewModel.FacilityInventoryID,
                            EditMode = EditModeEnum.GoAndCount,
                            IsValidateAndComplete = _ViewModel.IsValidateAndComplete,
                            SelectedFacility = _ViewModel.SelectedFacilityForFilter,
                            SelectedStorageLocation = _ViewModel.SelectedStorageLocation
                        }
                    }
                });
        }

        /// <summary>
        /// Command show open lines
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ViewOpenLines_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(
                new BSOInventoryLines()
                {
                    NavParam = new NavParameter(PageStateEnum.View)
                    {
                        Arguments = new InventoryNavArgument()
                        {
                            StorageLocationNo = _ViewModel.SelectedStorageLocation != null ? _ViewModel.SelectedStorageLocation.FacilityNo : null,
                            FacilityInventoryNo = _ViewModel.FacilityInventoryNo,
                            FacilityInventoryID = _ViewModel.FacilityInventoryID,
                            EditMode = EditModeEnum.Confirm,
                            IsValidateAndComplete = _ViewModel.IsValidateAndComplete,
                            SelectedFacility = _ViewModel.SelectedFacilityForFilter,
                            SelectedStorageLocation = _ViewModel.SelectedStorageLocation
                        }
                    }
                });
        }

        #endregion


        #region Methods -> Toolbar buttons
        /// <summary>
        /// Handle refresh
        /// call one more time load facility list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            _ViewModel.GetFacilitiesCommand.Execute(null);
        }

        /// <summary>
        /// Clear selected storage location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdClearStorageLocation_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedStorageLocation = null;
        }

        /// <summary>
        /// Clear selected faciltiy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdClearFacility_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedFacility = null;
        }

        #endregion

        #region Methods -> Event handler

        ///// <summary>
        ///// Handle BarcodeScanner return code event
        ///// wiht obitained facility populate filter dropdowns
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void barcodeScanner_OnSendSelectedCode(object sender, EventArgs e)
        //{
        //    if (barcodeScanner._ViewModel.BarcodeSequence != null)
        //    {
        //        Facility facility = barcodeScanner._ViewModel.BarcodeSequence.FirstOrDefault() as Facility;
        //        if (facility != null)
        //        {
        //            // Returned facility is Storage Location - only storage location filter is populated
        //            if (facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageLocation)
        //            {
        //                _ViewModel.SelectedStorageLocation = _ViewModel.StorageLocations.FirstOrDefault(c => c.FacilityNo == facility.FacilityNo);
        //            }
        //            // Barcode is facility - both filter (dropdown selected value) is populated - facility and facility parent as storage location
        //            else if (facility.ParentFacility != null && facility.ParentFacility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageLocation)
        //            {
        //                _ViewModel.SelectedStorageLocation = _ViewModel.StorageLocations.FirstOrDefault(c => c.FacilityNo == facility.ParentFacility.FacilityNo);
        //                _ViewModel.SelectedFacility = _ViewModel.Facilities.FirstOrDefault(c => c.FacilityNo == facility.FacilityNo);
        //            }
        //            barcodeScanner.Clean();
        //        }
        //        else
        //            _ViewModel.Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Error, AppStrings.SelectedBarcodeEntityNotValidFacility_Text);
        //    }
        //}

        //private void barcodeScanner_OnBarcodeCommandInvoked(object sender, EventArgs e)
        //{
        //    if (barcodeScanner._ViewModel.BarcodeSequence != null)
        //    {
        //        Facility facility = barcodeScanner._ViewModel.BarcodeSequence.FirstOrDefault() as Facility;
        //        if (facility != null)
        //        {
        //            // Returned facility is Storage Location - only storage location filter is populated
        //            if (facility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageLocation)
        //            {
        //                _ViewModel.SelectedStorageLocation = _ViewModel.StorageLocations.FirstOrDefault(c => c.FacilityNo == facility.FacilityNo);
        //                if (barcodeScanner != null)
        //                    barcodeScanner._ViewModel.IsListVisible = false;
        //            }
        //            // Barcode is facility - both filter (dropdown selected value) is populated - facility and facility parent as storage location
        //            else if (facility.ParentFacility != null && facility.ParentFacility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageLocation)
        //            {
        //                _ViewModel.SelectedStorageLocation = _ViewModel.StorageLocations.FirstOrDefault(c => c.FacilityNo == facility.ParentFacility.FacilityNo);
        //                _ViewModel.SelectedFacility = _ViewModel.Facilities.FirstOrDefault(c => c.FacilityNo == facility.FacilityNo);
        //                if (barcodeScanner != null)
        //                    barcodeScanner._ViewModel.IsListVisible = false;
        //            }
        //            barcodeScanner.Clean();
        //        }
        //        else
        //            _ViewModel.Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Error, AppStrings.SelectedBarcodeEntityNotValidFacility_Text);
        //    }
        //}

        #endregion

        #endregion

        private async void FacilityEntry_Focused(object sender, FocusEventArgs e)
        {
            _ViewModel.FacilitySelector = new FacilitySelectorViewModel("");
            await Navigation.PushAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));

            FacilityEntry.Unfocus();
        }

        private void cmdClearFacility_Clicked_1(object sender, EventArgs e)
        {
            _ViewModel.SelectedFacility = null;
        }
    }
}