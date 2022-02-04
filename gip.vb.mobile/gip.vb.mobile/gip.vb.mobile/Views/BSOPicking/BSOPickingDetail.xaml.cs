using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using gip.vb.mobile.ViewModels;
using gip.mes.webservices;
using System.Collections.Generic;
using System.Linq;
using gip.core.webservices;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOPickingDetail : BSOPageBase
    {
        PickingDetailViewModel _ViewModel;

        public BSOPickingDetail(PickingDetailViewModel viewModel)
        {
            BindingContext = this._ViewModel = viewModel;
            InitializeComponent();
        }

        public BSOPickingDetail()
        {
            _ViewModel = new PickingDetailViewModel();
            BindingContext = _ViewModel;
            InitializeComponent();
        }

        public ACClass RegisteredWorkplace
        {
            get;
            private set;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitPageOnNavigation();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void InitPageOnNavigation()
        {
            Picking picking;
            RegisteredWorkplace = null;

            List<object> parameters = NavParam.Arguments as List<object>;
            if (parameters != null)
            {
                picking = parameters.FirstOrDefault() as Picking;
                RegisteredWorkplace = parameters.LastOrDefault() as ACClass;
                if (RegisteredWorkplace != null)
                {
                    btnFinishOrder.Text = Strings.AppStrings.BtnBookAndFinish_Text;
                    _ViewModel.RegisteredWorkplace = RegisteredWorkplace;
                }
            }
            else
            {
                picking = NavParam.Arguments as Picking;
                btnFinishOrder.Text = Strings.AppStrings.BtnFinishOrder;
            }

            if (NavParam != null && _ViewModel.Item == null)
                _ViewModel.Item = picking;

            if (_ViewModel.Item != null)
                _ViewModel.Item.RefreshPickingPosInView();

            this.PageState = PageStateEnum.View;
        }

        private void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            _ViewModel.UpdatePickingCommand.Execute(null);
        }

        private void PickingPosListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
        }

        private async void PickingPosListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var pickingPos = e.Item as PickingPos;
            if (pickingPos == null)
                return;

            await Navigation.PushAsync(new BSOPickingPosDetail() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = new object[] { _ViewModel.Item, pickingPos } } });
        }

        private void btnFinishOrder_Clicked(object sender, EventArgs e)
        {
            if (RegisteredWorkplace != null)
            {
                _ViewModel.FinishAndBookOrderCommand.Execute(null);
            }
            else
            {
                _ViewModel.FinishOrderCommand.Execute(null);
            }
        }   
    }
}