using gip.core.datamodel;
using gip.mes.webservices;
using gip.vbm.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.vbm.mobile.Helpers;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using gip.vbm.mobile.Controls;

namespace gip.vbm.mobile.Views
{
    /// <summary>
    /// Outward-Booking (Materialeinsatz)
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOProdOrderOutward : BSOProdOrderInOutBase
	{

        protected override BarcodeScannerView BarcodeScanner
        {
            get
            {
                return barcodeScanner;
            }
        }

        BarcodeScanManuModel _FromTaskModel;
        ProdOrderInMaterialsViewModel _InMaterialsViewModel;
        public BSOProdOrderOutward(ProdOrderPartslistPosRelation relation, BarcodeScanManuModel taskModel, ACMethod wfMethod,
                                   ProdOrderInMaterialsViewModel inMaterialsViewModel)
		{
            _FromTaskModel = taskModel;
            _InMaterialsViewModel = inMaterialsViewModel;
            BindingContext = _ViewModel = new ViewModels.ProdOrderInOutViewModel(false, taskModel, relation, null, wfMethod, inMaterialsViewModel?.ProdOrderInMaterials);
			InitializeComponent();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            btnScaleAccordingAnotherComp.IsEnabled = false;
            if (_InMaterialsViewModel != null && _InMaterialsViewModel.ProdOrderInMaterials.Count > 1)
            {
                btnScaleAccordingAnotherComp.IsEnabled = true;
            }
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

#if ZXING
        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
#else
        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
#endif
        {
#if ZXING
            await BarcodeScanner.OpenBarcodeCamera();
#endif
        }

        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.RefreshOutItems();
        }

        private void btnTakeQFromQuant_Clicked(object sender, EventArgs e)
        {
            _ViewModel.TakePostingQuantityFromQuant();
        }

        private void btnScaleAccordingAnotherComp_Clicked(object sender, EventArgs e)
        {
            if (_InMaterialsViewModel != null)
            {
                if (_InMaterialsViewModel.ProdOrderInMaterials.Count > 1)
                {
                    var anotherComp = _InMaterialsViewModel.ProdOrderInMaterials.OrderBy(c => c.Sequence).FirstOrDefault();
                    if (anotherComp != null && anotherComp.ActualQuantityUOM > 0.00001 
                        && anotherComp.ProdOrderPartslistPosRelationID != _ViewModel.PosRelation.ProdOrderPartslistPosRelationID)
                    {
                        _ViewModel.ScaleAccordingAnotherComponent(anotherComp);
                    }
                }
            }
        }

        private async void btnShowQuants_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOProdOrderOutwardQSel(_ViewModel));
        }
    }
}