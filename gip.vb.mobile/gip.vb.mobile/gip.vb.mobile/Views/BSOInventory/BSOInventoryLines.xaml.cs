using gip.mes.webservices;
using gip.vb.mobile.ViewModels;
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
    public partial class BSOInventoryLines : BSOPageBase
    {
        InventoryLinesModel _ViewModel;

        public BSOInventoryLines()
        {
            _ViewModel = new InventoryLinesModel();
            BindingContext = _ViewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            object[] parameters = NavParam.Arguments as object[];
            _ViewModel.FacilityInventoryNo = parameters[0].ToString();
            if (parameters[1] != null)
                _ViewModel.SelectedStorageLocation = parameters[1] as Facility;
            if (parameters[2] != null)
                _ViewModel.SelectedFacility = parameters[2] as Facility;

            if (_ViewModel.OpenLines == null || !_ViewModel.OpenLines.Any())
                _ViewModel.GetOpenLinesCommand.Execute(null);
            this.PageState = PageStateEnum.View;
        }

        #region Event handlers



        async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteGetOpenLines();
        }

        #endregion

        private async void lstOpenLines_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            FacilityInventoryPos inventoryPos = e.Item as FacilityInventoryPos;
            if (inventoryPos == null)
                return;
            _ViewModel.SelectedLine = inventoryPos;
            await Navigation.PushAsync(
                new BSOInventoryLineEdit()
                {
                    NavParam = new NavParameter(PageStateEnum.View)
                    {
                        Arguments = new object[]
                        {
                            _ViewModel.FacilityInventoryNo,
                            EditModeEnum.Confirm,
                            _ViewModel.SelectedStorageLocation,
                            _ViewModel.SelectedFacility,
                            _ViewModel.SelectedLine
                        }
                    }
                });
        }

        private void lstOpenLines_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            FacilityInventoryPos inventoryPos = e.SelectedItem as FacilityInventoryPos;
            if (inventoryPos == null)
                return;
            _ViewModel.SelectedLine = inventoryPos;
        }
    }
}