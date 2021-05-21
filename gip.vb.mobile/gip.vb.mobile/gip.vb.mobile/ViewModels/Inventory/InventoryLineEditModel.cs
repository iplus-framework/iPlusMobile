using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.webservices;
using gip.vb.mobile.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels.Inventory
{
    public class InventoryLineEditModel : BaseViewModel
    {

        #region ctor's

        public InventoryLineEditModel()
        {
            // Commands
            GetFacilityInventoryPosesCommand = new Command(async () => await ExecuteGetFacilityInventoryPosesAsync());
            UpdateFacilityInventoryPosCommand = new Command(async () => await ExecuteUpdateFacilityInventoryPosAsync());
        }

        #endregion

        #region Overrides

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {

        }

        #endregion

        #region Commands
        public Command GetFacilityInventoryPosesCommand { get; set; }
        public Command UpdateFacilityInventoryPosCommand { get; set; }
        #endregion

        #region Parameters

        private string _FacilityInventoryNo;
        public string FacilityInventoryNo
        {
            get
            {
                return _FacilityInventoryNo;
            }
            set
            {
                SetProperty(ref _FacilityInventoryNo, value);
            }
        }

        public EditModeEnum EditMode { get; set; }


        private Facility _SelectedStorageLocation;
        public Facility SelectedStorageLocation
        {
            get
            {
                return _SelectedStorageLocation;
            }
            set
            {
                SetProperty(ref _SelectedStorageLocation, value);
            }
        }

        private Facility _SelectedFacility;
        public Facility SelectedFacility
        {
            get
            {
                return _SelectedFacility;
            }
            set
            {
                SetProperty(ref _SelectedFacility, value);
            }
        }

        private FacilityInventoryPos _SelectedInventoryLine;
        public FacilityInventoryPos SelectedInventoryLine
        {
            get
            {
                return _SelectedInventoryLine;
            }
            set
            {
                SetProperty(ref _SelectedInventoryLine, value);
            }
        }
        #endregion

        #region Models
        private string _InputCode;
        public string InputCode
        {
            get
            {
                return _InputCode;
            }
            set
            {
                SetProperty(ref _InputCode, value);
            }
        }
        #endregion

        #region Tasks
        /// <summary>
        /// Calling GetFacilityInventoryPosesAsync
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExecuteGetFacilityInventoryPosesAsync()
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    string facilityNo = null;

                    string materialNo = null;

                    short? mdFacilityInventoryPosStateIndex = null;

                    WSResponse<List<FacilityInventoryPos>> wSResponse = await _WebService.GetFacilityInventoryPosesAsync(
                        FacilityInventoryNo,
                        InputCode,
                        facilityNo,
                        "",
                        materialNo,
                        mdFacilityInventoryPosStateIndex != null ? mdFacilityInventoryPosStateIndex.Value.ToString() : "", null, null);
                    WSResponse = wSResponse;
                    success = WSResponse.Suceeded;
                    if (wSResponse.Suceeded)
                    {
                        List<FacilityInventoryPos> poses = wSResponse.Data;
                        FacilityInventoryPos findedPos = poses.FirstOrDefault();
                        switch (EditMode)
                        {
                            case EditModeEnum.GoAndCount:
                                SelectedInventoryLine = findedPos;
                                if (SelectedInventoryLine != null)
                                {
                                    Message = null;
                                    success = true;
                                }
                                else
                                {
                                    Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = AppStrings.FC_NotExist_Text };
                                    success = false;
                                }
                                break;
                            case EditModeEnum.Confirm:
                                if (findedPos != null)
                                {
                                    if (findedPos.FacilityInventoryPosID == SelectedInventoryLine.FacilityInventoryPosID)
                                    {
                                        Message = new Msg() { MessageLevel = eMsgLevel.Info, Message = AppStrings.FC_Match_Text };
                                        success = true;
                                    }
                                    else
                                    {
                                        Message = new Msg()
                                        {
                                            MessageLevel = eMsgLevel.Error,
                                            Message = string.Format(AppStrings.FC_NotMatch_Text, InputCode, SelectedInventoryLine.FacilityChargeID)
                                        };
                                        success = false;
                                    }
                                }
                                else
                                {
                                    Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = AppStrings.FC_NotExist_Text };
                                    success = false;
                                }
                                break;
                        }
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


        public async Task<bool> ExecuteUpdateFacilityInventoryPosAsync()
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    if (SelectedInventoryLine != null)
                    {
                        SelectedInventoryLine.MDFacilityInventoryPosStateIndex = (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.InProgress;
                        WSResponse<bool> wSResponse = await _WebService.UpdateFacilityInventoryPosAsync(SelectedInventoryLine);
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

        #endregion
    }
}
