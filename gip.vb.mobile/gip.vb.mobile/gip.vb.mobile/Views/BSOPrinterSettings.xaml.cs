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
    public partial class BSOPrinterSettings : BSOPageBase
    {
        public BSOPrinterSettings()
        {
            BindingContext = _ViewModel = new ViewModels.PrinterSettingsViewModel();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_ViewModel != null)
            {
                if (_ViewModel.GetAssignedPrinter != null)
                {
                    _ViewModel.GetAssignedPrinter.Execute(null);
                }
            }
        }

        private ViewModels.PrinterSettingsViewModel _ViewModel;


        private void BarcodeSearchBar_SearchButtonPressed(object sender, EventArgs e)
        {

        }
    }
}