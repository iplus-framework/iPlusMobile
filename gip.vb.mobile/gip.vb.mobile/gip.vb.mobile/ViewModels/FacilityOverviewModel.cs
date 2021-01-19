using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using gip.mes.webservices;
using gip.core.autocomponent;
using gip.vb.mobile.Views;
using gip.core.datamodel;

namespace gip.vb.mobile.ViewModels
{
    public class FacilityOverviewModel : BaseViewModel
    {
        public FacilityOverviewModel(Facility item = null)
        {
            Item = item;
            LoadFacilitySumOverviewCommand = new Command(async () => await ExecuteLoadFacilitySumOverviewCommand());
        }

        #region Properties
        private Facility _Item;
        public Facility Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetProperty(ref _Item, value);
                RebuildTitle();
            }
        }

        public FacilitySumOverview _Overview;
        public FacilitySumOverview Overview
        {
            get
            {
                return _Overview;
            }
            set
            {
                SelectedFacilityCharge = null;
                SetProperty(ref _Overview, value);
                RebuildTitle();
            }
        }


        private FacilityCharge _SelectedFacilityCharge;
        public FacilityCharge SelectedFacilityCharge
        {
            get
            {
                return _SelectedFacilityCharge;
            }
            set
            {
                SetProperty(ref _SelectedFacilityCharge, value);
            }
        }
        #endregion

        #region Methods
        void RebuildTitle()
        {
            if (Item != null)
            {
                if (Overview != null && Overview.FacilityStock != null)
                    Title = String.Format("{0}: {1:0.0}", Item.FacilityName, Overview.FacilityStock.StockQuantity);
                else
                    Title = Item.FacilityName;
            }
            else
                Title = "FacilityOverview";
        }

        public Command LoadFacilitySumOverviewCommand { get; set; }
        public async Task ExecuteLoadFacilitySumOverviewCommand()
        {
            if (IsBusy || Item == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetFacilitySumAsync(Item.FacilityID.ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                    Overview = response.Data;
                else
                    Overview = null;
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }

        #endregion

    }
}