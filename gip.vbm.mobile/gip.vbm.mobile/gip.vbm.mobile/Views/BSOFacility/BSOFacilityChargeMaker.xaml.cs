using gip.vbm.mobile.Controls;
using gip.vbm.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOFacilityChargeMaker : BSOPageBase
    {
        #region c'tors

        private FacilityChargeMakerViewModel _ViewModel;

        public BSOFacilityChargeMaker()
        {
            BindingContext = _ViewModel = new FacilityChargeMakerViewModel(null);
            InitializeComponent();
        }

        #endregion

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();
            barcodeScanner.OnAppearing();

            Guid? invID = NavParam.Arguments as Guid?;
            if (invID.HasValue)
            {
                _ViewModel.FacilityInventoryID = invID.Value;
            }

            _ViewModel.OnAppear();
        }

        protected override void OnDisappearing()
        {
            barcodeScanner?.OnDisappearing();
            base.OnDisappearing();
        }

        private void lvMaterials_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (_ViewModel.SelectedMaterial != null)
            {
                _ViewModel.FacilityChargeItem.Material = _ViewModel.SelectedMaterial;
                _ViewModel.MaterialSearchText = _ViewModel.SelectedMaterial.MaterialNo + " " + _ViewModel.SelectedMaterial.MaterialName1;
            }
        }

        private void BtnClearFacility_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.FacilityChargeItem != null)
            {
                _ViewModel.FacilityChargeItem.Facility = null;
                _ViewModel.FacilityChargeItem.OnPropChanged(nameof(_ViewModel.FacilityChargeItem.Facility));
            }
        }

        private async void FacilityEntry_Focused(object sender, FocusEventArgs e)
        {
            _ViewModel.FacilitySelector = new FacilitySelectorViewModel(PickingViewModel.PN_SelectedStorageLocationFrom);
            await Navigation.PushAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));

            FacilityEntry.Unfocus();
        }

        private void BtnSelectMaterialCancel_Clicked(object sender, EventArgs e)
        {
            _ViewModel.IsSelectMaterialVisible = false;
        }

        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
        {
            await barcodeScanner.OpenBarcodeCamera();
        }

        #endregion
    }
}