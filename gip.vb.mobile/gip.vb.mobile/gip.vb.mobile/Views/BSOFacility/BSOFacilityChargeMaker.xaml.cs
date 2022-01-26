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
        private FacilityChargeMakerViewModel _ViewModel;

        public BSOFacilityChargeMaker()
        {
            BindingContext = _ViewModel = new FacilityChargeMakerViewModel(null);
            InitializeComponent();
        }

        private void lvMaterials_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (_ViewModel.SelectedMaterial != null)
            {
                _ViewModel.Item.Material = _ViewModel.SelectedMaterial;
                _ViewModel.MaterialSearchText = _ViewModel.SelectedMaterial.MaterialNo + " " + _ViewModel.SelectedMaterial.MaterialName1;
            }
        }
    }
}