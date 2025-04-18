// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using gip.mes.facility;

namespace gip.vb.mobile.ViewModels
{
    public class PickingByMaterialViewModel : PickingBookingBaseViewModel
    {

        public PickingByMaterialViewModel()
        {
            LoadBarcodeEntityCommand = new Command(async () => await ExecuteLoadBarcodeEntityCommand());
            BookFacilityCommand = new Command(async () => await ExecuteBookFacilityCommand());
        }

        private PickingMaterial _Item;
        public PickingMaterial Item
        {
            get => _Item;
            set
            {
                SetProperty<PickingMaterial>(ref _Item, value);
                if (_Item != null)
                {
                    Title = _Item.Material.MaterialName1;
                    TotalBookingQantity = _Item.PickingItems.Sum(c => c.PostingQuantity);
                }
            }
        }

        private double _TotalBookingQantity;
        public double TotalBookingQantity
        {
            get => _TotalBookingQantity;
            set
            {
                SetProperty<double>(ref _TotalBookingQantity, value);

                if (WSBarcodeEntityResult != null && WSBarcodeEntityResult.FacilityCharge != null)
                {
                    MissingBookingQuantity = WSBarcodeEntityResult.FacilityCharge.StockQuantity - _TotalBookingQantity;
                }
            }
        }

        private double _MissingBookingQuantity;
        public double MissingBookingQuantity
        {
            get => _MissingBookingQuantity;
            set
            {
                SetProperty<double>(ref _MissingBookingQuantity, value);
            }
        }


        private PickingPos _SelectedPickingPos;
        public PickingPos SelectedPickingPos
        {
            get => _SelectedPickingPos;
            set
            {
                SetProperty<PickingPos>(ref _SelectedPickingPos, value);
            }
        }

        public BarcodeEntity _WSBarcodeEntityResult;
        public BarcodeEntity WSBarcodeEntityResult
        {
            get
            {
                return _WSBarcodeEntityResult;
            }
            set
            {
                SetProperty(ref _WSBarcodeEntityResult, value);

                if (_WSBarcodeEntityResult != null && _WSBarcodeEntityResult.FacilityCharge != null)
                {
                    MissingBookingQuantity = _WSBarcodeEntityResult.FacilityCharge.StockQuantity - _TotalBookingQantity;

                    if (Item == null)
                        return;

                    if (MissingBookingQuantity < -0.000001)
                    {
                        double availableQ = _WSBarcodeEntityResult.FacilityCharge.StockQuantity;

                        foreach(PickingPos pPos in Item.PickingItems)
                        {
                            if (availableQ < 0.0000001)
                            {
                                pPos.PostingQuantity = 0;
                            }
                            else
                            {
                                double calcQ = pPos.TargetQuantity - pPos.ActualQuantity;
                                if (calcQ > availableQ)
                                {
                                    pPos.PostingQuantity = availableQ;
                                    availableQ = 0;
                                }
                                else
                                {
                                    pPos.PostingQuantity = calcQ;
                                    availableQ = availableQ - calcQ;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (PickingPos pPos in Item.PickingItems)
                        {
                            pPos.CalculateDefaultPostingQuantity();
                        }
                    }
                }
            }
        }

        public Command LoadBarcodeEntityCommand { get; set; }
        public async Task ExecuteLoadBarcodeEntityCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetBarcodeEntityAsync(this.CurrentBarcode);
                this.WSResponse = response;
                this.WSBarcodeEntityResult = response.Data;
                if (response.Suceeded)
                    CurrentBarcodeEntity = new List<object> { response.Data.ValidEntity };
                else
                    CurrentBarcodeEntity = new List<object>();
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

        public Command BookFacilityCommand { get; set; }
        public async Task ExecuteBookFacilityCommand(bool skipQuantityCheck = false)
        {
            if (IsBusy || Item == null || Item.PickingItems == null)
                return;
            Message = null;

            if (!skipQuantityCheck && MissingBookingQuantity < 0)
            {
                Msg msg = new Msg(eMsgLevel.Question, Strings.AppStrings.PostingQInsufficient_Question);
                msg.MessageButton = eMsgButton.YesNo;
                ShowDialog(msg, requestID: 2);
                return;
            }

            BarcodeEntity barcodeEntity = WSBarcodeEntityResult;
            if (barcodeEntity == null)
                return;

            IsBusy = true;

            try
            {
                ACMethodBookingList bookings = new ACMethodBookingList();
                ZeroBookFacilityChargeID = null;

                foreach (PickingPos pp in Item.PickingItems)
                {
                    if (!FacilityConst.IsDoubleZeroForPosting(pp.PostingQuantity))
                    {
                        ACMethodBooking aCMethodBooking = new ACMethodBooking();
                        aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_Relocation_FacilityCharge_Facility;

                        if (pp.Picking != null)
                        {
                            if (pp.Picking.PickingType.MDKey == mes.datamodel.GlobalApp.PickingType.Issue.ToString())
                            {
                                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_PickingOutward;
                            }
                            else if (pp.Picking.PickingType.MDKey == mes.datamodel.GlobalApp.PickingType.Receipt.ToString())
                            {
                                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_PickingInward;
                            }
                            else if (pp.Picking.PickingType.PickingType == mes.datamodel.GlobalApp.PickingType.InternalRelocation)
                            {
                                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_PickingRelocation;
                                if (pp.ToFacility == null)
                                    continue;

                                aCMethodBooking.InwardFacilityID = pp.ToFacility.FacilityID;
                                aCMethodBooking.OutwardQuantity = pp.PostingQuantity;
                            }
                        }

                        aCMethodBooking.PickingPosID = pp.PickingPosID;
                        aCMethodBooking.OutwardQuantity = pp.PostingQuantity;
                        if (barcodeEntity.FacilityCharge != null)
                        {
                            aCMethodBooking.OutwardFacilityID = barcodeEntity.FacilityCharge.Facility.FacilityID;
                            aCMethodBooking.OutwardFacilityChargeID = barcodeEntity.FacilityCharge.FacilityChargeID;
                        }
                        aCMethodBooking.InwardQuantity = pp.PostingQuantity;
                        bookings.Add(aCMethodBooking);
                    }
                }

                if (barcodeEntity.FacilityCharge != null)
                    ZeroBookFacilityChargeID = barcodeEntity.FacilityCharge.FacilityChargeID;

                var response = await _WebService.BookFacilitiesAsync(bookings);
                this.WSResponse = response;

                if (response.Suceeded)
                {
                    Msg msg = response.Data.MsgDetails.FirstOrDefault(c => c.MessageLevel == eMsgLevel.Question);
                    if (msg != null)
                    {
                        msg.MessageButton = eMsgButton.YesNo;
                        ShowDialog(msg, requestID: 3);
                    }
                    else
                    {
                        BookingMessage = response.Data.DetailsAsText;
                    }

                    IsBusy = false;

                    await ExecuteRefreshPickingMaterial();
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
        }

        public Command RefreshPickingMaterial { get; set; }
        public async Task ExecuteRefreshPickingMaterial()
        {
            if (IsBusy || Item == null || Item.PickingItems == null)
                return;

            IsBusy = true;

            try
            {
                PickingPosList param = new PickingPosList(Item.PickingItems.Select(x => new PickingPos() { PickingPosID = x.PickingPosID }));
                var response = await _WebService.GetPickingPosByMaterialAsync(param);

                if (response.Suceeded)
                {
                    PickingPosList result = response.Data;
                    if (result != null)
                    {
                        foreach (PickingPos pos in result)
                        {
                            PickingPos existingPickingPos = Item.PickingItems.FirstOrDefault(c => c.PickingPosID == pos.PickingPosID);
                            if (existingPickingPos != null)
                            {
                                existingPickingPos.ActualQuantity = pos.ActualQuantity;
                                existingPickingPos.ActualQuantityUOM = pos.ActualQuantityUOM;
                                existingPickingPos.OnActualQuantityChanged();
                                existingPickingPos.CalculateDefaultPostingQuantity();
                            }
                        }

                        Item.RecalculateActualQuantity();
                        TotalBookingQantity = Item.PickingItems.Sum(c => c.PostingQuantity);

                        Msg msg = new Msg(eMsgLevel.Info, Strings.AppStrings.PostingSuccesful_Text);
                        Message = msg;
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
        }


        public void ChangePostingQuantity()
        {
            Msg msg = new Msg(eMsgLevel.QuestionPrompt, Strings.AppStrings.ChangePostingQ_Text);
            ShowDialog(msg, "", Keyboard.Default, SelectedPickingPos?.PostingQuantity.ToString(), 1);
        }

        public override async void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            if (DialogOptions.RequestID == 1)
            {
                if (result == Global.MsgResult.OK)
                {
                    double postingQuantity = 0;
                    if (double.TryParse(entredValue, out postingQuantity) && SelectedPickingPos != null)
                    {
                        SelectedPickingPos.PostingQuantity = postingQuantity;
                        TotalBookingQantity = _Item.PickingItems.Sum(c => c.PostingQuantity);
                    }
                }
            }
            else if (DialogOptions.RequestID == 2 && result == Global.MsgResult.Yes)
            {
                await ExecuteBookFacilityCommand(true);
            }
            else if (DialogOptions.RequestID == 3)
            {
                if (result == Global.MsgResult.Yes)
                {
                    await ExecuteBookZeroStockCommand();
                }
            }
        }
    }
}
