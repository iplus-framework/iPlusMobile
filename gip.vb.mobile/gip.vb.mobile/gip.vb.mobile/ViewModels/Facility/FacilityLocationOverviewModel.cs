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
    public class FacilityLocationOverviewModel : BaseViewModel
    {
        public FacilityLocationOverviewModel(Facility item = null)
        {
            Item = item;
            LoadFacilityLocationSumOverviewCommand = new Command(async () => await ExecuteLoadFacilityLocationSumOverviewCommand());
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

        public FacilityLocationSumOverview _Overview;
        public FacilityLocationSumOverview Overview
        {
            get
            {
                return _Overview;
            }
            set
            {
                //SelectedFacilityCharge = null;
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
                Title = Item.FacilityName;
            else
                Title = "MaterialOverview";
        }


        public Command LoadFacilityLocationSumOverviewCommand { get; set; }
        public async Task ExecuteLoadFacilityLocationSumOverviewCommand()
        {
            if (IsBusy || Item == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetFacilityLocationSumAsync(Item.FacilityID.ToString());
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