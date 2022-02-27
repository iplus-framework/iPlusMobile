using gip.core.datamodel;
using gip.mes.webservices;
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
    /// <summary>
    /// Outward-Booking (Materialeinsatz)
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOProdOrderOutward : BSOProdOrderInOutBase
	{
        BarcodeScanManuModel _FromTaskModel;
        public BSOProdOrderOutward(ProdOrderPartslistPosRelation relation, BarcodeScanManuModel taskModel)
		{
            _FromTaskModel = taskModel;
            BindingContext = _ViewModel = new ViewModels.ProdOrderInOutViewModel(false, relation, null);
			InitializeComponent();
		}

        public void ButtonDoBooking_Clicked(object sender, EventArgs e)
        {
            BarcodeEntity barcodeEntity = _ViewModel.WSBarcodeEntityResult;
            if (barcodeEntity == null || barcodeEntity.FacilityCharge == null)
            {
                _ViewModel.Message = new core.datamodel.Msg() { MessageLevel = core.datamodel.eMsgLevel.Error, Message = Strings.AppStrings.ScanFCToActivate_Text };
                return;
            }
            else if (barcodeEntity.FacilityCharge.NotAvailable)
            {
                _ViewModel.Message = new core.datamodel.Msg() { MessageLevel = core.datamodel.eMsgLevel.Info, Message = Strings.AppStrings.FC_QuantRestoreNotAvailable };
                return;
            }
            else if (barcodeEntity.FacilityCharge.Material.MaterialID != _ViewModel.PosRelation.SourcePos.Material.MaterialID)
            {
                _ViewModel.Message = new core.datamodel.Msg() { MessageLevel = core.datamodel.eMsgLevel.Error, Message = Strings.AppStrings.PickingWrongMaterial_Text };
                return;
            }

            if (_FromTaskModel != null && _FromTaskModel.ScannedMachine != null)
                _ViewModel.PropertyACUrl = ACUrlHelper.GetParentACUrl(_FromTaskModel.ScannedMachine.ACClass.ACUrlComponent);
            _ViewModel.BookFacilityCommand.Execute(null);
        }

        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.RefreshOutItems();
        }
    }
}