using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels.Inventory
{
    public class InventoryLinesModel : BaseViewModel
    {
        #region ctor's
        public InventoryLinesModel()
        {
            // Commands
            GetOpenLinesCommand = new Command(async () => await ExecuteGetOpenLines());
        }
        #endregion

        #region Overrides

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {

        }
        #endregion

        #region Commands
        public Command GetOpenLinesCommand { get; private set; }

        #endregion

        #region Params

        public InventoryNavArgument InventoryNavArgument { get; set; }

        #endregion

        #region Data

        public List<MDFacilityInventoryPosState> PosStates { get; set; }

        private List<FacilityInventoryPos> _OpenLines;
        public List<FacilityInventoryPos> OpenLines
        {
            get
            {
                return _OpenLines;
            }
            set
            {
                SetProperty(ref _OpenLines, value);
            }
        }

        private FacilityInventoryPos _SelectedLine;
        public FacilityInventoryPos SelectedLine
        {
            get
            {
                return _SelectedLine;
            }
            set
            {
                SetProperty(ref _SelectedLine, value);
            }
        }
        #endregion

        #region Tasks
        /// <summary>
        /// Calling GetFacilityInventoriesAsync
        /// </summary>
        /// <returns></returns>
        /// 
        public async Task<bool> ExecuteGetOpenLines()
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    if (PosStates == null || !PosStates.Any())
                    {
                        WSResponse<List<MDFacilityInventoryPosState>> posStatesResponse = await _WebService.GetMDFacilityInventoryPosStatesAsync();
                        if (posStatesResponse.Suceeded)
                            PosStates = posStatesResponse.Data;
                    }

                    string faciltiyNo = InventoryNavArgument.SelectedFacility != null ? InventoryNavArgument.SelectedFacility.FacilityNo : null;
                    short? mdFacilityInventoryPosStateIndex = (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.InProgress;
                    if (InventoryNavArgument.IsValidateAndComplete)
                        mdFacilityInventoryPosStateIndex = (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished;
                    WSResponse<List<FacilityInventoryPos>> wSResponse =
                        await _WebService.GetFacilityInventoryLinesAsync(InventoryNavArgument.FacilityInventoryNo, null, InventoryNavArgument.StorageLocationNo, faciltiyNo,
                            null,
                            null,
                            mdFacilityInventoryPosStateIndex.ToString(),
                            null,
                            null,
                            (!InventoryNavArgument.IsValidateAndComplete).ToString()
                     );
                    OpenLines = wSResponse.Data;
                    if (OpenLines != null && OpenLines.Any())
                        foreach (FacilityInventoryPos item in OpenLines)
                            item.MDFacilityInventoryPosState = PosStates.FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == item.MDFacilityInventoryPosStateIndex);
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
