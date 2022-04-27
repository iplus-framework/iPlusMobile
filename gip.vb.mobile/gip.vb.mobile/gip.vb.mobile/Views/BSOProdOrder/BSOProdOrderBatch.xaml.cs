using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.webservices;
using gip.vb.mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOProdOrderBatch : BSOPageBase
	{
        ProdOrderIntermBatchesViewModel _ViewModel;
        BarcodeScanManuModel _FromTaskModel;
        public BSOProdOrderBatch(ProdOrderPartslistPos intermediate, BarcodeScanManuModel taskModel)
		{
            _FromTaskModel = taskModel;
            BindingContext = _ViewModel = new ProdOrderIntermBatchesViewModel(intermediate);
            
			InitializeComponent();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_ViewModel.ProdOrderIntermediateBatches.Count == 0)
                _ViewModel.LoadProdOrderIntermBatchesCommand.Execute(null);
        }

        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteLoadProdOrderIntermBatchesCommand();
        }

        private async void ProdOrderIntermediateBatches_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ProdOrderPartslistPos batch = e.Item as ProdOrderPartslistPos;
            if (batch == null)
                return;

            batch.ProdOrderPartslist = _ViewModel.IntermediatePos.ProdOrderPartslist;
            await Navigation.PushAsync(new BSOProdOrderInOutSelector(batch, _FromTaskModel, false, mes.facility.PostingQuantitySuggestionMode.OrderQuantity));
        }
    }
}