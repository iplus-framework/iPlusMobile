using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels.Inventory
{
    public class InventoryModel : BaseViewModel
    {

        #region ctor's
        
        public InventoryModel()
        {
            FilterInventoryEndDate = new DateTime(DateTime.Now.Year + 1, 1, 1);
            FilterInventoryStartDate = FilterInventoryEndDate.AddYears(-1);

            // Commands
            GetFacilityInventoriesCommand = new Command(async () => await ExecuteGetFacilityInventoriesAsync());
        }

        #endregion

        #region Overrides

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
        public Command GetFacilityInventoriesCommand { get; private set; }

        #endregion

        #region Tasks

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
                    short? mdFacilityInventoryStateIndex = (short)MDFacilityInventoryState.FacilityInventoryStates.InProgress;;
                    WSResponse<List<FacilityInventory>> wSResponse = 
                        await _WebService.GetFacilityInventoriesAsync(
                            mdFacilityInventoryStateIndex != null ? mdFacilityInventoryStateIndex.Value.ToString() : "",
                            FilterInventoryStartDate.ToString("o"), 
                            FilterInventoryEndDate.ToString("o"));
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
