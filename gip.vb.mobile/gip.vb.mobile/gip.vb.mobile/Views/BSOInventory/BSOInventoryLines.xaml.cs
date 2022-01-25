using gip.mes.webservices;
using gip.vb.mobile.Strings;
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

        #region ctor's
        public BSOInventoryLines()
        {
            _ViewModel = new InventoryLinesModel();
            BindingContext = _ViewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ViewModel.Title = AppStrings.Inv_OpenPositions;
            _ViewModel.InventoryNavArgument = NavParam.Arguments as InventoryNavArgument;
            _ViewModel.GetOpenLinesCommand.Execute(null);
            this.PageState = PageStateEnum.View;
        }

        #endregion

        #region Event handlers

        async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteGetOpenLines();
        }

        private async void lstOpenLines_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            FacilityInventoryPos inventoryPos = e.Item as FacilityInventoryPos;
            if (inventoryPos == null)
                return;

            _ViewModel.SelectedLine = inventoryPos;
            _ViewModel.InventoryNavArgument.SelectedInventoryLine = _ViewModel.SelectedLine;


            await Navigation.PushAsync(new BSOInventoryLineEdit()
            {
                NavParam = new NavParameter(PageStateEnum.View)
                {
                    Arguments = _ViewModel.InventoryNavArgument
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


        #endregion

        private async void btnNewQuant_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOFacilityChargeMaker());
        }
    }
}