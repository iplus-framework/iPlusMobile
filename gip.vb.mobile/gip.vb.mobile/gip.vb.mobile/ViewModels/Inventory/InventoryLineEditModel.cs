using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
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
            UpdateFacilityInventoryPosCommand = new Command(async () => await ExecuteUpdateFacilityInventoryPosAsync());
        }

        #endregion

        #region Overrides

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {

        }

        #endregion

        #region Commands

        public Command UpdateFacilityInventoryPosCommand { get; set; }

        #endregion

        #region Parameters

        public BarcodeScannerModel BarcodeScannerModel { get; set; }

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


        private bool _IsEditPanelVisible;
        public bool IsEditPanelVisible
        {
            get
            {
                return _IsEditPanelVisible;
            }
            set
            {
                SetProperty(ref _IsEditPanelVisible, value);
            }
        }

        private bool _IsChargeCommandPanelVisible;
        public bool IsChargeCommandPanelVisible
        {
            get
            {
                return _IsChargeCommandPanelVisible;
            }
            set
            {
                SetProperty(ref _IsChargeCommandPanelVisible, value);
            }
        }

        private bool _IsChargeAddCommandVisible;
        public bool IsChargeAddCommandVisible
        {
            get
            {
                return _IsChargeAddCommandVisible;
            }
            set
            {
                SetProperty(ref _IsChargeAddCommandVisible, value);
            }
        }

        private bool _IsChargeEditCommandVisible;
        public bool IsChargeEditCommandVisible
        {
            get
            {
                return _IsChargeEditCommandVisible;
            }
            set
            {
                SetProperty(ref _IsChargeEditCommandVisible, value);
            }
        }

        public FacilityCharge CurrentFacilityCharge
        {
            get
            {
                if (BarcodeScannerModel.CurrentBarcodeEntity == null || !BarcodeScannerModel.CurrentBarcodeEntity.Any())
                    return null;
                return BarcodeScannerModel.CurrentBarcodeEntity.FirstOrDefault() as FacilityCharge;
            }
        }

        #endregion

        #region Methods

        public void HideChargeCommandPanel()
        {
            IsChargeCommandPanelVisible = false;
            IsChargeEditCommandVisible = false;
            IsChargeAddCommandVisible = false;
        }

        #endregion

        #region Tasks

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

        public async Task<bool> ExecuteGetFacilityInventorySearchCharge(EditModeEnum editMode, string facilityChargeID)
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    if (CurrentFacilityCharge != null && !string.IsNullOrEmpty(InventoryNavArgument.StorageLocationNo))
                    {
                        string faciltiyNo = InventoryNavArgument.SelectedFacility != null ? InventoryNavArgument.SelectedFacility.FacilityNo : null;
                        WSResponse<SearchFacilityCharge> wSResponse = await _WebService.GetFacilityInventorySearchCharge(InventoryNavArgument.FacilityInventoryNo, InventoryNavArgument.StorageLocationNo, faciltiyNo, facilityChargeID);
                        success = wSResponse.Suceeded;

                        HideChargeCommandPanel();
                        IsEditPanelVisible = editMode == EditModeEnum.Confirm;

                        if (success)
                        {
                            List<FacilityChargeStateEnum> responseStates = wSResponse.Data.States;
                            if (editMode == EditModeEnum.GoAndCount)
                            {
                                Title = AppStrings.Inv_EditLine;

                                if (responseStates.Contains(FacilityChargeStateEnum.NotExist))
                                    // message not exist - FC_NotExist_Text
                                    wSResponse.Message = new Msg(eMsgLevel.Error, AppStrings.FC_NotExist_Text);
                                else
                                {
                                    if (responseStates.Contains(FacilityChargeStateEnum.InDifferentFacility))
                                        wSResponse.Message = new Msg(eMsgLevel.Error, string.Format(AppStrings.FC_QuantOnDifferentFaciltiy, wSResponse.Data.DifferentFacilityNo));
                                    else if (responseStates.Contains(FacilityChargeStateEnum.QuantNotAvailable))
                                    {
                                        // message quant there, can be restored - FC_QuantRestoreNotAvailable
                                        IsChargeCommandPanelVisible = true;
                                        IsChargeAddCommandVisible = true;
                                        wSResponse.Message = new Msg(eMsgLevel.Error, AppStrings.FC_QuantRestoreNotAvailable);
                                    }
                                    else if (responseStates.Contains(FacilityChargeStateEnum.AlreadyFinished))
                                    {
                                        // Message line already finished can be edited again
                                        IsChargeCommandPanelVisible = true;
                                        IsChargeEditCommandVisible = true;
                                        wSResponse.Message = new Msg(eMsgLevel.Error, AppStrings.FC_QuantAlreadyFinished);
                                        SelectedInventoryLine = wSResponse.Data.FacilityInventoryPos;
                                    }
                                    else
                                    {
                                        SelectedInventoryLine = wSResponse.Data.FacilityInventoryPos;
                                        IsEditPanelVisible = true;
                                    }
                                }
                                WriteNewStockQuantity();
                            }
                            else
                            {
                                if (wSResponse.Data.FacilityInventoryPos == null || wSResponse.Data.FacilityInventoryPos.FacilityChargeID != CurrentFacilityCharge.FacilityChargeID)
                                    wSResponse.Message = new Msg(eMsgLevel.Error, string.Format(AppStrings.FC_NotMatch_Text, CurrentFacilityCharge.FacilityChargeID, wSResponse.Data.FacilityInventoryPos == null ? "-" : wSResponse.Data.FacilityInventoryPos.FacilityChargeID.ToString()));
                                else
                                    wSResponse.Message = new Msg(eMsgLevel.Info, AppStrings.FC_Match_Text);
                            }

                        }
                        WSResponse = wSResponse;
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

        public async Task<bool> ExecuteSetFacilityInventoryChargeAvailable()
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    if (CurrentFacilityCharge != null && !string.IsNullOrEmpty(InventoryNavArgument.StorageLocationNo))
                    {
                        WSResponse<FacilityInventoryPos> wSResponse = await _WebService.SetFacilityInventoryChargeAvailable(InventoryNavArgument.FacilityInventoryNo, CurrentFacilityCharge.FacilityChargeID.ToString());
                        WSResponse = wSResponse;
                        success = wSResponse.Suceeded;
                        if (success)
                        {
                            SelectedInventoryLine = wSResponse.Data;
                            IsEditPanelVisible = true;
                        }
                        else
                            SelectedInventoryLine = null;
                        HideChargeCommandPanel();
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

        #region Other methods
        public void WriteNewStockQuantity()
        {
            if (SelectedInventoryLine != null
                                && !SelectedInventoryLine.NotAvailable
                                && SelectedInventoryLine.NewStockQuantity == null)
                SelectedInventoryLine.NewStockQuantity = SelectedInventoryLine.StockQuantity;
        }
        #endregion
    }
}
