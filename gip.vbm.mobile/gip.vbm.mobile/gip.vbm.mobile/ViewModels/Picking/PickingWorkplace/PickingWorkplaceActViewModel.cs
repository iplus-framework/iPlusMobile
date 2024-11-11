// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.ViewModels
{
    public class PickingWorkplaceActViewModel : BaseViewModel
    {
        public PickingWorkplaceActViewModel() : base ()
        {
            BarcodeScanModel = new BarcodeScanModelBase(true);
            ActivateFacilityChargeCommand = new Command(async () => await ExecuteActivateFacilityCharge());
            DeactivateFacilityChargeCommand = new Command(async () => await ExecuteDeactivateFacilityCharge());
        }

        private FacilityCharge _FacilityChargeItem;
        public FacilityCharge FacilityChargeItem
        {
            get => _FacilityChargeItem;
            set
            {
                SetProperty(ref _FacilityChargeItem, value);
            }
        }

        public ACClass Workplace
        {
            get;
            set;
        }

        public BarcodeScanModelBase BarcodeScanModel
        {
            get;
            private set;
        }

        public Command ActivateFacilityChargeCommand
        {
            get;
            set;
        }

        public async Task ExecuteActivateFacilityCharge()
        {
            if (IsBusy || BarcodeScanModel == null || FacilityChargeItem == null)
                return;

            if (Workplace == null)
            {
                Message = new Msg(eMsgLevel.Error, Strings.AppStrings.WorkplaceEmpty_Text);
                return;
            }

            try
            {
                BarcodeSequence sequence = BarcodeScanModel.ExchangedBarcodeSeq;
                if (sequence == null)
                {
                    Message = new Msg(eMsgLevel.Error, Strings.AppStrings.ScanFCActivate_Text);
                    return;
                }

                FacilityCharge fc = sequence.LastAddedSequence?.FacilityCharge;
                if (fc == null)
                {
                    Message = new Msg(eMsgLevel.Error, Strings.AppStrings.ScanFCActivate_Text);
                    return;
                }

                if (fc.Material.MaterialID != FacilityChargeItem.Material.MaterialID)
                {
                    Message = new Msg(eMsgLevel.Error, Strings.AppStrings.WrongMaterial_Text);
                    return;
                }

                FacilityChargeParamItem actItem = new FacilityChargeParamItem()
                {
                    FacilityChargeID = fc.FacilityChargeID,
                    Material = fc.Material,
                    ParamID = Workplace.ACClassID
                };

                IsBusy = true;

                WSResponse<bool> response = await _WebService.ActivateFacilityChargeAsync(actItem);
                if (response.Suceeded)
                {
                    if (response.Message != null)
                    {
                        Message = response.Message;
                    }

                    if (response.Data)
                    {
                        Message = new Msg(eMsgLevel.Info, Strings.AppStrings.ActSuccessful_Text);
                        FacilityChargeItem = fc;
                    }
                    else
                    {
                        Message = new Msg(eMsgLevel.Error, Strings.AppStrings.ActIsNotSuccessful_Text);
                    }    

                }
                else if (response.Message != null)
                {
                    Message = response.Message;
                }
            }
            catch (Exception e)
            {
                Message = new Msg(eMsgLevel.Exception, e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public Command DeactivateFacilityChargeCommand
        {
            get;
            set;
        }

        public async Task ExecuteDeactivateFacilityCharge()
        {
            if (IsBusy || BarcodeScanModel == null || FacilityChargeItem == null)
                return;

            if (Workplace == null)
            {
                Message = new Msg(eMsgLevel.Error, Strings.AppStrings.WorkplaceEmpty_Text);
                return;
            }

            try
            {
                FacilityChargeParamItem deactItem = new FacilityChargeParamItem()
                {
                    FacilityChargeID = FacilityChargeItem.FacilityChargeID,
                    Material = FacilityChargeItem.Material,
                    ParamID = Workplace.ACClassID
                };

                IsBusy = true;

                WSResponse<bool> response = await _WebService.DeactivateFacilityChargeAsync(deactItem);
                if (response.Suceeded)
                {
                    if (response.Message != null)
                    {
                        Message = response.Message;
                    }

                    if (response.Data)
                    {
                        Message = new Msg(eMsgLevel.Info, Strings.AppStrings.DeactSuccessful_Text);
                        //Item = fc;
                    }
                    else
                    {
                        Message = new Msg(eMsgLevel.Error, Strings.AppStrings.ActIsNotSuccessful_Text);
                    }

                }
                else if (response.Message != null)
                {
                    Message = response.Message;
                }
            }
            catch (Exception e)
            {
                Message = new Msg(eMsgLevel.Exception, e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override void DialogResponse(Global.MsgResult result, string enteredValue = null)
        {
            
        }
    }
}
