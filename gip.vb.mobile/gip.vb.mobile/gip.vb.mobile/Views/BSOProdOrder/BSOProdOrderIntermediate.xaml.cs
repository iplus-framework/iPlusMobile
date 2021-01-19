using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.vb.mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOProdOrderIntermediate : BSOPageBase
	{
        ProdOrderPLIntermediatesViewModel _ViewModel;

		public BSOProdOrderIntermediate(ProdOrderPartslist prodOrderPartslist)
		{
            BindingContext = _ViewModel = new ProdOrderPLIntermediatesViewModel(prodOrderPartslist);
            _ViewModel.Title = prodOrderPartslist.Partslist.PartslistNo;
            InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_ViewModel.ProdOrderPLIntermediates.Count == 0)
                _ViewModel.LoadProdOrderPLIntermediatesCommand.Execute(null);
        }

        private async void TBItemRefresh_Clicked(object sender, EventArgs e)
        {
            await _ViewModel.ExecuteLoadProdOrderPLIntermediatesCommand();
        }

        private async void ProdOrderPLIntermediates_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ProdOrderPartslistPos item = e.Item as ProdOrderPartslistPos;
            if (item == null)
                return;

            await Navigation.PushAsync(new BSOProdOrderBatch(item));
        }
    }
}