using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using gip.mes.webservices;
using gip.core.autocomponent;
using gip.vbm.mobile.Views;
using gip.core.datamodel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.ViewModels
{
    public class FacilityLotOverviewModel : BaseViewModel
    {
        public FacilityLotOverviewModel(FacilityLot item = null)
        {
            Item = item;
            LoadFacilityLotSumOverviewCommand = new Command(async () => await ExecuteLoadFacilityLotSumOverviewCommand());
        }

        #region Properties
        private FacilityLot _Item;
        public FacilityLot Item
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

        public FacilityLotSumOverview _Overview;
        public FacilityLotSumOverview Overview
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
                if (Overview != null && Overview.FacilityLotStock != null)
                    Title = String.Format("{0}: {1:0.0}", Item.LotNo, Overview.FacilityLotStock.StockQuantity);
                else
                    Title = Item.LotNo;
            }
            else
                Title = "FacilityLotOverview";
        }

        public Command LoadFacilityLotSumOverviewCommand { get; set; }
        public async Task ExecuteLoadFacilityLotSumOverviewCommand()
        {
            if (IsBusy || Item == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetFacilityLotSumAsync(Item.FacilityLotID.ToString());
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