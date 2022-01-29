using gip.vb.mobile.Controls;
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
    public partial class BSOPickingWorkplace : BSOPageBase
    {
        public BSOPickingWorkplace()
        {
            BarcodeScan = new BarcodeScanner();
            BarcodeScan.IsEnabledInvokeBarcodeOnServer = true;

            InitializeComponent();
        }

        public BarcodeScanner BarcodeScan
        {
            get;
            set;
        }

        protected override void OnAppearing()
        {
            BarcodeScan.OnAppearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            BarcodeScan.OnDisappearing();
            base.OnDisappearing();
        }

    }
}