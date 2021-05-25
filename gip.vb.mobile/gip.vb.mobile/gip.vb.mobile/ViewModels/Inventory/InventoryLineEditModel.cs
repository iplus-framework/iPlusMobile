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
            GetFacilityInventoryLinesCommand = new Command(async () => await ExecuteGetFacilityInventoryLinesAsync());
            UpdateFacilityInventoryPosCommand = new Command(async () => await ExecuteUpdateFacilityInventoryPosAsync());
        }

        #endregion

        #region Overrides

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {

        }

        #endregion

        #region Commands
        public Command GetFacilityInventoryLinesCommand { get; set; }
        public Command UpdateFacilityInventoryPosCommand { get; set; }
        #endregion

        #region Parameters

        public InventoryNavArgument InventoryNavArgument { get; set; }

        private FacilityInventoryPos _SelectedInventoryLine;
        public FacilityInventoryPos SelectedInventoryLine
        {
            get
            {
                return _SelectedInventoryLine;
            }
            set
            {
                if (_SelectedInventoryLine != value)
                {
                    SetProperty(ref _SelectedInventoryLine, value);
                    if (_SelectedInventoryLine != null)
                    {
                        _SelectedInventoryLine.PropertyChanged -= _SelectedInventoryLine_PropertyChanged;
                        _SelectedInventoryLine.PropertyChanged += _SelectedInventoryLine_PropertyChanged;
                    }
                }
            }
        }

        private void _SelectedInventoryLine_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FacilityInventoryPos pos = sender as FacilityInventoryPos;
            switch (e.PropertyName)
            {
                case "NotAvailable":
                    pos.NewStockQuantity = null;
                    break;
                case "NewStockQuantity":
                    if (pos.NewStockQuantity != null)
                        pos.NotAvailable = false;
                    break;
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
        /// Calling ExecuteGetFacilityInventoryLinesAsync
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExecuteGetFacilityInventoryLinesAsync()
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    string materialNo = null;
                    short? mdFacilityInventoryPosStateIndex = (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.InProgress;

                    WSResponse<List<FacilityInventoryPos>> wSResponse = await _WebService.GetFacilityInventoryLinesAsync(
                        InventoryNavArgument.FacilityInventoryNo,
                        InputCode,
                        InventoryNavArgument.SelectedFacility.FacilityNo,
                        null,
                        materialNo,
                        mdFacilityInventoryPosStateIndex != null ? mdFacilityInventoryPosStateIndex.Value.ToString() : "",
                        null,
                        null,
                        "false" // notProcessed = true - all open lines, otherwise all lines
                       );

                    WSResponse = wSResponse;
                    success = WSResponse.Suceeded;
                    if (wSResponse.Suceeded)
                    {
                        List<FacilityInventoryPos> lines = wSResponse.Data;
                        FacilityInventoryPos findedPos = lines.FirstOrDefault();
                        switch (InventoryNavArgument.EditMode)
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
                        SelectedInventoryLine.UpdateName = App.SettingsViewModel.LastUser;
                        SelectedInventoryLine.MDFacilityInventoryPosStateIndex = (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.InProgress;
                        if (InventoryNavArgument.EditMode == EditModeEnum.Confirm)
                            SelectedInventoryLine.MDFacilityInventoryPosStateIndex = (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished;
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
