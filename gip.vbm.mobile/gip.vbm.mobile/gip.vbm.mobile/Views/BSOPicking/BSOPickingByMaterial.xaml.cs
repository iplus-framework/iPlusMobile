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
    public partial class BSOPickingByMaterial : BSOPageBase
    {
        public BSOPickingByMaterial()
        {
            BindingContext = _ViewModel = new PickingsViewModel();
            InitializeComponent();
        }

        private PickingsViewModel _ViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitPageOnNavigation();

            if (_ViewModel.Pickings.Count == 0)
                _ViewModel.LoadPickingsCommand.Execute(null);
        }

        private void InitPageOnNavigation()
        {
            if (NavParam != null)
            {
                PickingViewModel filter = NavParam.Arguments as PickingViewModel;
                if (filter != null)
                {
                    _ViewModel.PickingFilter = filter;
                }
            }
            this.PageState = PageStateEnum.View;
        }

        private async void PickingsByMaterialListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new BSOPickingByMaterialDetail() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = _ViewModel.SelectedPickingMaterial } });
        }
    }
}