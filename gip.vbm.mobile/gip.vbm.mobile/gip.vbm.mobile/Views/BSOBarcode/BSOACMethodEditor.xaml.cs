using gip.core.datamodel;
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
    public partial class BSOACMethodEditor : BSOTabbedPageBase
    {
        ACMethod _ViewModel;

        public BSOACMethodEditor(ACMethod viewModel)
        {
            BindingContext = _ViewModel = viewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private async void BtnApply_Clicked(object sender, EventArgs e)
        {
            _ViewModel.AutoRemove = true;
            _ = await Navigation.PopAsync();
        }
    }
}