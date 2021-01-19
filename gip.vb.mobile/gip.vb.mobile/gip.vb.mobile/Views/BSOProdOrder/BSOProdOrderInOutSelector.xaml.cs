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
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BSOProdOrderInOutSelector : BSOPageBase
	{
		public BSOProdOrderInOutSelector(ProdOrderPartslistPos batch)
		{
            IntermOrIntermBatch = batch;
            //Intermediate = intermediate;
            BindingContext = this;
            InitializeComponent();
		}

        public new string Title
        {
            get => Strings.AppStrings.ProdOrderInOutBooking_Header;
        }

        public ProdOrderPartslistPos IntermOrIntermBatch
        {
            get;
            set;
        }

        //public ProdOrderPartslistPos Intermediate
        //{
        //    get;
        //    set;
        //}

        private async void BtnInput_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOProdOrderOutwardMatSel(IntermOrIntermBatch));
        }

        private async void BtnOutput_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BSOProdOrderInward(IntermOrIntermBatch));
        }
    }
}