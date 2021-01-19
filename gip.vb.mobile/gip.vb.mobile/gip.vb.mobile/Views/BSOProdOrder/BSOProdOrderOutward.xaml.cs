using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    /// <summary>
    /// Outward-Booking (Materialeinsatz)
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOProdOrderOutward : BSOProdOrderInOutBase
	{
		public BSOProdOrderOutward (ProdOrderPartslistPosRelation relation)
		{
            BindingContext = _ViewModel = new ViewModels.ProdOrderInOutViewModel(false, relation, null);
			InitializeComponent();
		}

        public void ButtonDoBooking_Clicked(object sender, EventArgs e)
        {
            BarcodeEntity barcodeEntity = _ViewModel.WSBarcodeEntityResult;
            if (barcodeEntity == null || barcodeEntity.FacilityCharge == null)
            {
                _ViewModel.Message = new core.datamodel.Msg() { Message = "Kein Quant gescannt!" };
                return;
            }
            //if (barcodeEntity.FacilityCharge.Material.MaterialID != _ViewModel.Item.Material.MaterialID)
            //{
            //    _ViewModel.Message = new core.datamodel.Msg() { Message = "Material des Quants stimmt nicht mit dem Material der Kommissionierposition überein!" };
            //    return;
            //}

            _ViewModel.BookFacilityCommand.Execute(null);
        }

        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.RefreshOutItems();
        }
    }
}