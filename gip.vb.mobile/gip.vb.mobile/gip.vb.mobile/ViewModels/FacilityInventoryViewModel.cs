using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class FacilityInventoryViewModel : BaseViewModel
    {

        #region ctor's
        public FacilityInventoryViewModel()
        {
            FilterInventoryEndDate = DateTime.Now.Date.AddDays(1);
            FilterInventoryStartDate = FilterInventoryEndDate.AddYears(-1);
            MDFacilityInventoryStates = new List<MDFacilityInventoryState>();

            // Commands
            GetListsCommand = new Command(async () => await ExecuteGetListsAsync());
            GetFacilityInventoriesCommand = new Command(async () => await ExecuteGetFacilityInventoriesAsync());

        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {

        }
        #endregion

        #region Properties -> Filter

        #region Properties -> Filter -> Inventory


        private DateTime _FilterInventoryStartDate;
        public DateTime FilterInventoryStartDate
        {
            get
            {
                return _FilterInventoryStartDate;
            }
            set
            {
                SetProperty(ref _FilterInventoryStartDate, value);
            }
        }



        private DateTime _FilterInventoryEndDate;
        public DateTime FilterInventoryEndDate
        {
            get
            {
                return _FilterInventoryEndDate;
            }
            set
            {
                SetProperty(ref _FilterInventoryEndDate, value);
            }
        }



        private MDFacilityInventoryState _FilterFacilityInventoryState;
        public MDFacilityInventoryState FilterFacilityInventoryState
        {
            get
            {
                return _FilterFacilityInventoryState;
            }
            set
            {
                SetProperty(ref _FilterFacilityInventoryState, value);
            }
        }



        #endregion

        #region Properties -> Model

        private List<MDFacilityInventoryState> _MDFacilityInventoryStates;
        public List<MDFacilityInventoryState> MDFacilityInventoryStates
        {
            get
            {
                return _MDFacilityInventoryStates;
            }
            set
            {
                SetProperty(ref _MDFacilityInventoryStates, value);
            }
        }

        private List<FacilityInventory> _FacilityInventories;
        public List<FacilityInventory> FacilityInventories
        {
            get
            {
                return _FacilityInventories;
            }
            set
            {
                SetProperty(ref _FacilityInventories, value);
            }
        }

        private FacilityInventory _SelectedFacilityInventory;
        public FacilityInventory SelectedFacilityInventory
        {
            get
            {
                return _SelectedFacilityInventory;
            }
            set
            {
                SetProperty(ref _SelectedFacilityInventory, value);
            }
        }
        #endregion

        #endregion

        #region Commands
        public Command GetListsCommand { get; set; }
        public Command GetFacilityInventoriesCommand { get; private set; }

        #endregion

        #region Tasks
        /// <summary>
        /// Calling ExecuteGetListsAsync
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExecuteGetListsAsync()
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    WSResponse<List<MDFacilityInventoryState>> wSResponsemDFacilityInventoryState = await _WebService.GetMDFacilityInventoryStatesAsync();
                    MDFacilityInventoryStates = wSResponsemDFacilityInventoryState.Data;

                    short? stateIndex = null;
                    if (FilterFacilityInventoryState != null)
                        stateIndex = (short)FilterFacilityInventoryState.MDFacilityInventoryStateIndex;
                    WSResponse<List<FacilityInventory>> wsResponseFacilityInventories = await _WebService.GetFacilityInventoriesAsync(
                        stateIndex != null ? stateIndex.Value.ToString() : "", 
                        FilterInventoryStartDate.ToString("o", CultureInfo.InvariantCulture), 
                        FilterInventoryEndDate.ToString("o", CultureInfo.InvariantCulture));
                    FacilityInventories = wsResponseFacilityInventories.Data;

                    success = wsResponseFacilityInventories.Suceeded
                                && wSResponsemDFacilityInventoryState.Suceeded;
                }
                catch (Exception ex)
                {
                    Message = new Msg(eMsgLevel.Exception, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
            return success;
        }

        /// <summary>
        /// Calling GetFacilityInventoriesAsync
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExecuteGetFacilityInventoriesAsync()
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    short? mdFacilityInventoryStateIndex = null;
                    if (FilterFacilityInventoryState != null)
                        mdFacilityInventoryStateIndex = (short)FilterFacilityInventoryState.MDFacilityInventoryStateIndex;

                    WSResponse<List<FacilityInventory>> wSResponse = await _WebService.GetFacilityInventoriesAsync(mdFacilityInventoryStateIndex != null ? mdFacilityInventoryStateIndex.Value.ToString() : "",
                        FilterInventoryStartDate.ToString(), FilterInventoryEndDate.ToString());
                    FacilityInventories = wSResponse.Data;
                    WSResponse = wSResponse;
                    success = wSResponse.Suceeded;
                }
                catch (Exception ex)
                {
                    Message = new Msg(eMsgLevel.Exception, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
            return success;
        }
        #endregion

    }
}
