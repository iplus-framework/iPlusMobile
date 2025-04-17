using System;
using gip.mes.webservices;
using gip.core.datamodel;
using gip.core.webservices;

namespace gip.vbm.mobile.ViewModels
{
    public class OperationLogOverviewModel : BaseViewModel
    {
        public OperationLogOverviewModel(ACClass item = null)
        {
            ACClassItem = item;
            LoadOperationLogOverviewCommand = new Command(async () => await ExecuteLoadOperationLogOverviewCommand());
        }

        #region Properties
        private ACClass _ACClassItem;
        public ACClass ACClassItem
        {
            get
            {
                return _ACClassItem;
            }
            set
            {
                SetProperty(ref _ACClassItem, value);
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
            if (ACClassItem != null)
            {
                Title = ACClassItem.ACCaption;
            }
            else
                Title = "Operation log";
        }

        public Command LoadOperationLogOverviewCommand { get; set; }
        public async Task ExecuteLoadOperationLogOverviewCommand()
        {
            if (IsBusy || ACClassItem == null)
                return;
            IsBusy = true;

            try
            {
                var response = await _WebService.GetOperationLogFacilityChargesAsync(ACClassItem.ACClassID.ToString());
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