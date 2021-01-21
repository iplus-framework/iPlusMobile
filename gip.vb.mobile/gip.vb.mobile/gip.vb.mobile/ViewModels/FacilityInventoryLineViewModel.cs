using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class FacilityInventoryLineViewModel : BaseViewModel
    {
        #region ctor's

        public FacilityInventoryLineViewModel(FacilityInventoryPos facilityInventoryPos, FacilityInventoryLinesViewModel facilityInventoryLinesViewModel = null)
        {
            FacilityInventoryPos = facilityInventoryPos;

            GetListsCommand = new Command(async () => await ExecuteGetListsAsync());
            FacilityInventoryLinesViewModel = facilityInventoryLinesViewModel;
        }

        #endregion

        #region Properties
        public FacilityInventoryLinesViewModel FacilityInventoryLinesViewModel { get; set; }


        private FacilityInventoryPos _FacilityInventoryPos;
        public FacilityInventoryPos FacilityInventoryPos
        {
            get
            {
                return _FacilityInventoryPos;
            }
            set
            {
                SetProperty(ref _FacilityInventoryPos, value);
            }
        }

        private List<MDFacilityInventoryPosState> _MDFacilityInventoryPosStates;
        public List<MDFacilityInventoryPosState> MDFacilityInventoryPosStates
        {
            get
            {
                return _MDFacilityInventoryPosStates;
            }
            set
            {
                SetProperty(ref _MDFacilityInventoryPosStates, value);
            }
        }

        public bool IsEnabledUpdateFacilityInventoryPosCommand
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Commands
        public Command GetListsCommand { get; set; }

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
                    WSResponse<List<MDFacilityInventoryPosState>> wSResponsemDFacilityInventoryPosState = await _WebService.GetMDFacilityInventoryPosStatesAsync();
                    MDFacilityInventoryPosStates = wSResponsemDFacilityInventoryPosState.Data;

                    success = wSResponsemDFacilityInventoryPosState.Suceeded;
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

        public async Task<bool> ExecuteUpdateFacilityInventoryPosAsync()
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    if (FacilityInventoryPos != null)
                    {
                        FacilityInventoryPos.MDFacilityInventoryPosState = MDFacilityInventoryPosStates.FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.InProgress);
                        WSResponse<bool> wSResponse = await _WebService.UpdateFacilityInventoryPosAsync(FacilityInventoryPos);
                        WSResponse = wSResponse;
                        success = wSResponse.Suceeded;
                    }
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
        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {

        }

        #endregion



    }
}
