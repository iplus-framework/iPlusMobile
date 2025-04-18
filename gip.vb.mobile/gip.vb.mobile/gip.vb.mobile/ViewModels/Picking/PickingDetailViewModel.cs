// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.webservices;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class PickingDetailViewModel : BaseViewModel
    {
        public PickingDetailViewModel(Picking item = null)
        {
            Item = item;
            UpdatePickingCommand = new Command(async () => await UpdatePicking());
            FinishOrderCommand = new Command(async () => await FinishOrder());
            FinishAndBookOrderCommand = new Command(async () => await ExecuteFinishAndBookOrder());
        }

        #region Properties
        private Picking _Item;
        public Picking Item
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

        public ACClass RegisteredWorkplace
        {
            get;
            set;
        }

        private bool _IsPickingWorkplaceError;
        public bool IsPickingWorkplaceError
        {
            get => _IsPickingWorkplaceError;
            private set
            {
                SetProperty(ref _IsPickingWorkplaceError, value);
            }
        }
        public void RefreshItem()
        {            
        }
        #endregion

        #region Methods
        void RebuildTitle()
        {
            if (Item != null)
                Title = Item.PickingNo;
            else
                Title = "PickingDetailViewModel";
        }

        public Command UpdatePickingCommand { get; set; }
        public async Task<bool> UpdatePicking()
        {
            bool result = false;
            if (IsBusy)
                return result;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetPickingAsync(Item.PickingID.ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    Item = response.Data;
                    if (Item != null)
                    {
                        Item.RefreshPickingPosInView();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
            return result;
        }

        public Command FinishOrderCommand { get; set; }
        public async Task<bool> FinishOrder(bool skipCheck = false)
        {
            MsgWithDetails result = null;
            if (IsBusy)
                return false;

            IsBusy = true;

            try
            {
                core.autocomponent.WSResponse<MsgWithDetails> response = null;
                if (!skipCheck)
                {
                    response = await _WebService.FinishPickingOrderAsync(Item.PickingID);
                }
                else
                {
                    response = await _WebService.FinishPickingOrderWithoutCheckAsync(Item.PickingID);
                }

                this.WSResponse = response;
                if (response.Suceeded)
                {
                    result = response.Data;
                    if (result != null)
                    {
                        if (!skipCheck && result.MessageLevel == eMsgLevel.Question)
                        {
                            //result.MessageLevel = eMsgLevel.Question;
                            result.MessageButton = eMsgButton.YesNo;
                            ShowDialog(result, requestID : 1);
                        }
                        else
                        {
                            ShowDialog(result);
                        }
                    }
                    else
                    {
                        Message = new Msg(eMsgLevel.Info, Strings.AppStrings.OrderFinished_Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
            return true;
        }

        public Command FinishAndBookOrderCommand
        {
            get;
            set;
        }
        public async Task ExecuteFinishAndBookOrder()
        {
            if (IsBusy)
                return;

            if (IsPickingWorkplaceError)
                return;

            try
            {
                var requiredQuants = Item.PickingPos_Picking
                                         .Where(x => x.CompleteFactor < 70)
                                         .Select(p => new FacilityCharge()
                                         {
                                             Material = p.Material,
                                             MDUnit = p.MDUnit
                                         }).ToList();

                IsBusy = true;

                var activatedQuants = await _WebService.GetRegisteredFacilityChargesAsync(RegisteredWorkplace.ACClassID.ToString());

                if (activatedQuants.Suceeded)
                {
                    if (activatedQuants.Message != null)
                    {
                        Message = activatedQuants.Message;
                        return;
                    }

                    List<FacilityCharge> actQuants = activatedQuants.Data;

                    if (actQuants != null)
                    {
                        foreach (FacilityCharge fc in actQuants)
                        {
                            var matchingQuants = requiredQuants.Where(c => c.Material != null && c.Material.MaterialID == fc.Material.MaterialID).ToArray();
                            foreach (var matchQuant in matchingQuants)
                            {
                                if (fc.NotAvailable)
                                {
                                    matchQuant.NotAvailable = true;
                                    continue;
                                }

                                requiredQuants.Remove(matchQuant);
                            }
                        }
                    }

                    if (requiredQuants.Any())
                    {
                        string message = Strings.AppStrings.BookAndFinishPickingError_Text + System.Environment.NewLine;
                        foreach (var quant in requiredQuants)
                        {
                            message += quant.Material.MaterialName1;
                            if (quant.NotAvailable)
                            {
                                message += " - " + Strings.AppStrings.InventoryPosNotAvailable_Text;
                            }

                            message += System.Environment.NewLine;
                        }

                        ShowDialog(new Msg(eMsgLevel.Error, message));
                        
                        return;
                    }

                    PickingWorkplace pickingWorkplace = new PickingWorkplace() { PickingID = Item.PickingID, WorkplaceID = RegisteredWorkplace.ACClassID };
                    var response = await _WebService.BookAndFinishPickingOrderAsync(pickingWorkplace);

                    if (response.Suceeded)
                    {
                        IsBusy = false;

                        UpdatePickingCommand.Execute(null);

                        MsgWithDetails result = response.Data;
                        if (result != null && result.MsgDetailsCount > 0)
                        {
                            IsPickingWorkplaceError = true;
                            ShowDialog(new Msg(eMsgLevel.Error, response.Data.DetailsAsText));
                        }
                        else
                        {
                            Message = new Msg(eMsgLevel.Info, Strings.AppStrings.PostingSuccesful_Text);
                        }

                    }
                    else if (response.Message != null)
                    {
                        Message = response.Message;
                    }

                }
                else if (activatedQuants.Message != null)
                {
                    Message = activatedQuants.Message;
                }
                else
                {
                    Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Error, "Problem with retriving activated quants.");
                }
            }
            catch (Exception e)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override async void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            if (DialogOptions.RequestID == 1)
            {
                if (result == Global.MsgResult.Yes)
                {
                    await FinishOrder(true);
                }
            }
        }

        #endregion
    }
}
