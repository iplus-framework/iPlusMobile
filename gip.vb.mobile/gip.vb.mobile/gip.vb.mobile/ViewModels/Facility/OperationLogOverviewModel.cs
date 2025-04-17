using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using gip.mes.webservices;
using gip.core.autocomponent;
using gip.vb.mobile.Views;
using gip.core.datamodel;
using gip.core.webservices;
using System.Collections.Generic;

namespace gip.vb.mobile.ViewModels
{
    public class OperationLogOverviewModel : BaseViewModel
    {
        public OperationLogOverviewModel(ACClass item = null)
        {
            Item = item;
            LoadOperationLogOverviewCommand = new Command(async () => await ExecuteLoadOperationLogOverviewCommand());
        }

        #region Properties
        private ACClass _Item;
        public ACClass Item
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

        public List<FacilityCharge> _Overview;
        public List<FacilityCharge> Overview
        {
            get
            {
                return _Overview;
            }
            set
            {
                SelectedFacilityCharge = null;
                SetProperty(ref _Overview, value);
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
                Title = Item.ACCaption;
            }
            else
                Title = "Operation log";
        }

        public Command LoadOperationLogOverviewCommand { get; set; }
        public async Task ExecuteLoadOperationLogOverviewCommand()
        {
            if (IsBusy || Item == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetOperationLogFacilityChargesAsync(Item.ACClassID.ToString());
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