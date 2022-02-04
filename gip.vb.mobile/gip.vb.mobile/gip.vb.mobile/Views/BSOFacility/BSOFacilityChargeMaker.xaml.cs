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
    public partial class BSOFacilityChargeMaker : BSOPageBase
    {
        #region c'tors

        private FacilityChargeMakerViewModel _ViewModel;

        public BSOFacilityChargeMaker()
        {
            BindingContext = _ViewModel = new FacilityChargeMakerViewModel(null);
            BarcodeScan = new BarcodeScanner();
            BarcodeScan.IsEnabledInvokeBarcodeOnServer = true;

            InitializeComponent();
        }

        #endregion

        #region Properties

        public BarcodeScanner BarcodeScan
        {
            get;
            set;
        }

        #endregion

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BarcodeScan._ViewModel = _ViewModel.FacilityScanViewModel;
            BarcodeScan.OnAppearing();

            Guid? invID = NavParam.Arguments as Guid?;
            if (invID.HasValue)
            {
                _ViewModel.FacilityInventoryID = invID.Value;
            }

            _ViewModel.OnAppear();
        }

        protected override void OnDisappearing()
        {
            BarcodeScan?.OnDisappearing();
            base.OnDisappearing();
        }

        private void lvMaterials_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (_ViewModel.SelectedMaterial != null)
            {
                _ViewModel.Item.Material = _ViewModel.SelectedMaterial;
                _ViewModel.MaterialSearchText = _ViewModel.SelectedMaterial.MaterialNo + " " + _ViewModel.SelectedMaterial.MaterialName1;
            }
        }

        private void BtnClearFacility_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel.Item != null)
            {
                _ViewModel.Item.Facility = null;
                _ViewModel.Item.OnPropChanged(nameof(_ViewModel.Item.Facility));
            }
        }

        private async void FacilityEntry_Focused(object sender, FocusEventArgs e)
        {
            _ViewModel.FacilitySelector = new FacilitySelectorViewModel(PickingViewModel.PN_SelectedStorageLocationFrom);
            await Navigation.PushModalAsync(new BSOFacilitySelector(_ViewModel.FacilitySelector));

            FacilityEntry.Unfocus();
        }

        private void BtnSelectMaterialCancel_Clicked(object sender, EventArgs e)
        {
            _ViewModel.IsSelectMaterialVisible = false;
        }

        #endregion
    }
}