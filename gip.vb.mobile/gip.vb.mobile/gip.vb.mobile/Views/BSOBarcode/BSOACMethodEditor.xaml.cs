using gip.core.datamodel;
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
    public partial class BSOACMethodEditor : BSOPageBase
    {
        ACMethod _ViewModel;

        public BSOACMethodEditor(ACMethod viewModel)
        {
            BindingContext = _ViewModel = viewModel;
            InitializeComponent();
        }

        private bool _ClosedFromOKButton = false;

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            if (!_ClosedFromOKButton)
            {
            }

            base.OnDisappearing();
        }

        private async void BtnApply_Clicked(object sender, EventArgs e)
        {
            _ClosedFromOKButton = true;
            _ = await Navigation.PopAsync();
        }
    }
}