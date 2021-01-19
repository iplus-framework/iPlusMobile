using gip.mes.webservices;
using gip.vb.mobile.barcode;
using gip.vb.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.DataGrid;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    /// <summary>
    /// Inward-Booking (Output of Production; Ergebnis)
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOProdOrderInward : BSOProdOrderInOutBase
    {
        public BSOProdOrderInward(ProdOrderPartslistPos intermOrIntermBatch)
        {
            BindingContext = _ViewModel = new ProdOrderInOutViewModel(true, null, intermOrIntermBatch);
            InitializeComponent();
        }

        public void ButtonDoBooking_Clicked(object sender, EventArgs e)
        {
            //BarcodeEntity barcodeEntity = _ViewModel.WSBarcodeEntityResult;
            //if (barcodeEntity == null || barcodeEntity.Facility == null)
            //{
            //    _ViewModel.Message = new core.datamodel.Msg() { Message = "Kein Facility gescannt!" };
            //    return;
            //}
            //if (barcodeEntity.FacilityCharge.Material.MaterialID != _ViewModel.Item.Material.MaterialID)
            //{
            //    _ViewModel.Message = new core.datamodel.Msg() { Message = "Material des Quants stimmt nicht mit dem Material der Kommissionierposition überein!" };
            //    return;
            //}

            _ViewModel.BookFacilityCommand.Execute(null);
        }

        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.RefreshInItems();
        }

        protected async override void OnAppearing()
        {
            await _ViewModel.LoadTargetFacilities();
            await _ViewModel.RefreshInItems();
        }
    }
}