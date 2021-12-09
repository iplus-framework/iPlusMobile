using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.webservices;
using gip.vb.mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOPickingByMaterialDetail : BSOTabbedPageBase
    {
        private PickingByMaterialViewModel _ViewModel;

        public BSOPickingByMaterialDetail()
        {
            BindingContext = _ViewModel = new PickingByMaterialViewModel();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitPageOnNavigation();
        }

        private void InitPageOnNavigation()
        {
            if (NavParam != null)
            {
                PickingMaterial material = NavParam.Arguments as PickingMaterial;
                if (material != null)
                {
                    _ViewModel.Item = material;
                }
            }
            this.PageState = PageStateEnum.View;
        }

        private void PickingsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _ViewModel.ChangePostingQuantity();
        }
    }
}