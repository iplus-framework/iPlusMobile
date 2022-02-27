﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.facility;
using gip.mes.webservices;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class ProdOrderInOutViewModel : BaseViewModel
    {
        #region c'tors

        public ProdOrderInOutViewModel(bool isInward, ProdOrderPartslistPosRelation relation, ProdOrderPartslistPos intermOrIntermBatch)
        {
            IsInward = isInward;
            PosRelation = relation;
            IntermOrIntermBatch = intermOrIntermBatch;
            RebuildTitle();

            ReadPostingsCommand = new Command(async () => await ExecuteReadPostingsCommand());
            LoadBarcodeEntityCommand = new Command(async () => await ExecuteLoadBarcodeEntityCommand());
            BookFacilityCommand = new Command(async () => await ExecuteBookFacilityCommand());
            PrintCommand = new Command(async () => await ExecutePrintCommand());
        }

        #endregion

        #region Properties

        public bool IsInward
        {
            get;
            set;
        }

        public ProdOrderPartslistPosRelation _PosRelation;
        public ProdOrderPartslistPosRelation PosRelation
        {
            get => _PosRelation;
            set => SetProperty(ref _PosRelation, value, "PosRelation");
        }

        public ProdOrderPartslistPos _IntermOrIntermBatch;
        public ProdOrderPartslistPos IntermOrIntermBatch
        {
            get => _IntermOrIntermBatch;
            set => SetProperty(ref _IntermOrIntermBatch, value, "IntermOrIntermBatch");
        }

        private List<Facility> _TargetFacilities;
        public List<Facility> TargetFacilities
        {
            get => _TargetFacilities;
            set => SetProperty(ref _TargetFacilities, value, "TargetFacilities");
        }


        private Facility _CurrentFacility;
        public Facility CurrentFacility
        {
            get => _CurrentFacility;
            set => SetProperty(ref _CurrentFacility, value, "CurrentFacility");
        }

        private PostingOverview _Overview;
        public PostingOverview Overview
        {
            get
            {
                return _Overview;
            }
            set
            {
                SetProperty(ref _Overview, value);
            }
        }

        private double _BookingQuantity;
        public double BookingQuantity
        {
            get
            {
                return _BookingQuantity;
            }
            set
            {
                SetProperty(ref _BookingQuantity, value);
            }
        }

        private string _PropertyACUrl;
        public string PropertyACUrl
        {
            get
            {
                return _PropertyACUrl;
            }
            set
            {
                SetProperty(ref _PropertyACUrl, value);
            }
        }

        public string _CurrentBarcode;
        public string CurrentBarcode
        {
            get
            {
                return _CurrentBarcode;
            }
            set
            {
                SetProperty(ref _CurrentBarcode, value);
            }
        }

        public List<object> _CurrentBarcodeEntity;
        public List<object> CurrentBarcodeEntity
        {
            get
            {
                return _CurrentBarcodeEntity;
            }
            set
            {
                SetProperty(ref _CurrentBarcodeEntity, value);
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
            }
        }

        public string _BookingMessage;
        public string BookingMessage
        {
            get
            {
                return _BookingMessage;
            }
            set
            {
                SetProperty(ref _BookingMessage, value);
            }
        }

        #endregion

        #region Mehods & Commands

        public Command ReadPostingsCommand { get; set; }
        public async Task ExecuteReadPostingsCommand()
        {
            if (IsBusy)
                return;

            if (IsInward)
                await ReadPostignsIn();
            else
                await ReadPostingsOut();
        }

        public async Task ReadPostignsIn()
        {
            if (IntermOrIntermBatch == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetProdOrderPosFacilityBookingAsync(IntermOrIntermBatch.ProdOrderPartslistPosID.ToString());
                this.WSResponse = response;
                if (response != null && response.Suceeded)
                    Overview = response.Data;
                else
                    Overview = new PostingOverview();
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

        public async Task RefreshIntermOrIntermBatch()
        {
            if (IntermOrIntermBatch == null || IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetProdOrderPartslistPosAsync(IntermOrIntermBatch.ProdOrderPartslistPosID.ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                    IntermOrIntermBatch = response.Data;
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

        public async Task ReadPostingsOut()
        {
            if (PosRelation == null || IsBusy)
                return;

            IsBusy = true;

            try
            {
                await Task.Delay(100); // hack for ListView - because IsRefreshing won't stop
                var response = await _WebService.GetProdOrderPosRelFacilityBookingAsync(PosRelation.ProdOrderPartslistPosRelationID.ToString());
                this.WSResponse = response;
                if (response != null && response.Suceeded)
                    Overview = response.Data;
                else
                    Overview = new PostingOverview();
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

        public async Task RefreshPosRelation()
        {
            if (PosRelation == null || IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetProdOrderPartslistPosRelAsync(PosRelation.ProdOrderPartslistPosRelationID.ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                    PosRelation = response.Data;
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

        public async Task RefreshInItems()
        {
            await RefreshIntermOrIntermBatch();
            await ReadPostignsIn();
        }

        public async Task RefreshOutItems()
        {
            await RefreshPosRelation();
            await ReadPostingsOut();
        }

        public Command LoadBarcodeEntityCommand { get; set; }
        public async Task ExecuteLoadBarcodeEntityCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetBarcodeEntityAsync(CurrentBarcode);
                this.WSResponse = response;
                this.WSBarcodeEntityResult = response.Data;

                if (response.Suceeded)
                {
                    if (IsInward)
                    {
                        if (response.Data.Facility != null)
                        {
                            var currentFacility = TargetFacilities.FirstOrDefault(c => c.FacilityID == response.Data.Facility.FacilityID);
                            if (currentFacility != null)
                                CurrentFacility = currentFacility;
                            else
                            {
                                //TODO: add facility to list or return error
                            }
                        }
                    }
                    else
                    { 
                        if (response.Data.FacilityCharge != null)
                        {
                            List<object> entries = new List<object>();
                            entries.Add(response.Data.ValidEntity);
                            CurrentBarcodeEntity = entries;

                            double requiredQuantity = PosRelation.TargetQuantity - PosRelation.ActualQuantityUOM;
                            if (requiredQuantity > response.Data.FacilityCharge.StockQuantity)
                            {
                                BookingQuantity = response.Data.FacilityCharge.StockQuantity;
                            }
                            else
                            {
                                BookingQuantity = requiredQuantity;
                            }
                            
                        }
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

        public Command BookFacilityCommand { get; set; }
        public async Task ExecuteBookFacilityCommand()
        {
            if (IsBusy 
                || FacilityConst.IsDoubleZeroForPosting(BookingQuantity))
                return;

            if (IsInward)
                await BookFacilityIn();
            else
                await BookFacilityOut();
        }

        private async Task BookFacilityIn()
        {
            if (CurrentFacility == null)
                return;

            IsBusy = true;
            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_ProdOrderPosInward.ToString();
                aCMethodBooking.PartslistPosID = IntermOrIntermBatch.ProdOrderPartslistPosID;
                aCMethodBooking.InwardQuantity = BookingQuantity;
                aCMethodBooking.InwardFacilityID = CurrentFacility.FacilityID;
                aCMethodBooking.InwardMaterialID = IntermOrIntermBatch.BookingMaterialID;
                aCMethodBooking.InwardFacilityLotID = IntermOrIntermBatch.FacilityLotID;
                aCMethodBooking.MDUnitID = IntermOrIntermBatch.MDUnit?.MDUnitID;
                aCMethodBooking.PropertyACUrl = this.PropertyACUrl;
                BookingQuantity = 0;
                PropertyACUrl = null;
                var response = await _WebService.BookFacilityAsync(aCMethodBooking);
                this.WSResponse = response;
                if (!response.Suceeded)
                    Message = response.Message != null ? response.Message : new Msg(eMsgLevel.Error, "Booking Error");
                else
                {
                    if (response.Data != null && !String.IsNullOrEmpty(response.Data.DetailsAsText))
                        Message = response.Data;
                    else
                    {
                        IsBusy = false;
                        await ExecuteReadPostingsCommand();
                        IsBusy = false;
                        Message = new Msg(eMsgLevel.Info, Strings.AppStrings.PostingSuccesful_Text);
                        Print(Strings.AppStrings.PickingBookSuccAndPrint_Question);
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

        private async Task BookFacilityOut()
        {
            BarcodeEntity barcodeEntity = WSBarcodeEntityResult;
            if (barcodeEntity == null || barcodeEntity.FacilityCharge == null)
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_ProdOrderPosOutward.ToString();
                aCMethodBooking.PartslistPosRelationID = PosRelation.ProdOrderPartslistPosRelationID;
                aCMethodBooking.OutwardQuantity = BookingQuantity;
                aCMethodBooking.OutwardFacilityID = barcodeEntity.FacilityCharge.Facility.FacilityID;
                aCMethodBooking.OutwardFacilityChargeID = barcodeEntity.FacilityCharge.FacilityChargeID;
                aCMethodBooking.OutwardMaterialID = PosRelation.SourcePos.Material.MaterialID;
                aCMethodBooking.PropertyACUrl = PropertyACUrl;
                BookingQuantity = 0;
                PropertyACUrl = null;
                var response = await _WebService.BookFacilityAsync(aCMethodBooking);
                this.WSResponse = response;
                if (!response.Suceeded)
                    Message = response.Message != null ? response.Message : new Msg(eMsgLevel.Error, "Booking Error");
                else
                {
                    if (response.Data != null && !String.IsNullOrEmpty(response.Data.DetailsAsText))
                        Message = response.Data;
                    else
                    {
                        CurrentBarcodeEntity = null;
                        CurrentBarcode = null;

                        IsBusy = false;
                        await ExecuteReadPostingsCommand();
                        IsBusy = false;
                        await RefreshPosRelation();
                        IsBusy = false;
                        Message = new Msg(eMsgLevel.Info, Strings.AppStrings.PostingSuccesful_Text);
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

        void RebuildTitle()
        {
            if (PosRelation != null && PosRelation.SourcePos != null)
                Title = PosRelation.SourcePos.Material?.MaterialName1;
            else if (PosRelation != null && PosRelation.TargetPos != null)
                Title = PosRelation.TargetPos.Material?.MaterialName1;
            else if (IntermOrIntermBatch != null && IntermOrIntermBatch.Material != null)
                Title = IntermOrIntermBatch.Material.MaterialName1;
            else
                Title = "ProdOrderInOutViewModel";
        }

        public override async void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            if (DialogOptions.RequestID == 1)
            {
                if (result == Global.MsgResult.OK)
                {
                    int copies = 0;
                    if (int.TryParse(entredValue, out copies))
                    {
                        if (copies > 0)
                        {
                            PrintEntity printEntity = new PrintEntity();
                            printEntity.CopyCount = copies;

                            var facilityBookingCharge = GetPrintFacilityBookingCharge();
                            if (facilityBookingCharge == null)
                            {
                                Msg msg = new Msg(eMsgLevel.Error, Strings.AppStrings.PrintFBCMissing_Text);
                                ShowDialog(msg, requestID: 2);
                                return;
                            }

                            Guid? facilityChargeID = facilityBookingCharge.InwardFacilityChargeID.HasValue ? facilityBookingCharge.InwardFacilityChargeID : facilityBookingCharge.OutwardFacilityChargeID;

                            if (!facilityChargeID.HasValue)
                            {
                                Msg msg = new Msg(eMsgLevel.Error, Strings.AppStrings.PrintQuantMissing_Text);
                                ShowDialog(msg, requestID: 2);
                                return;
                            }

                            FacilityCharge fc = new FacilityCharge() { FacilityChargeID = facilityChargeID.Value };
                            printEntity.Sequence = new List<BarcodeEntity>()
                                                    {
                                                        new BarcodeEntity(){ FacilityCharge = fc }
                                                    };

                            await ExecutePrintCommand(printEntity);
                        }
                    }
                }
            }
        }

        public async Task LoadTargetFacilities()
        {
            if (IntermOrIntermBatch != null)
            {
                var result = await _WebService.GetPOBatchTargetFacilitiesAsync(IntermOrIntermBatch.ProdOrderPartslistPosID.ToString());
                if (result.Suceeded && result.Data != null)
                {
                    TargetFacilities = result.Data;
                    if (TargetFacilities.Count == 1)
                        CurrentFacility = TargetFacilities.FirstOrDefault();
                }
                else
                    TargetFacilities = new List<Facility>();
            }
        }

        public Command PrintCommand { get; set; }
        public async Task<bool> ExecutePrintCommand(PrintEntity printEntity = null)
        {

            if (IsBusy)
                return false;

            IsBusy = true;
            bool success = false;
            try
            {
                if (printEntity == null)
                    return false;

                WSResponse<bool> result = await _WebService.Print(printEntity);
                success = result.Data;
                Message = result.Message;
            }
            catch (Exception ex)
            {
                Message = new Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
            return success;
        }

        public void Print(string message)
        {
            var facilityBookingCharge = GetPrintFacilityBookingCharge();
            if (facilityBookingCharge == null)
            {
                Msg msgError = new Msg(eMsgLevel.Error, Strings.AppStrings.PrintFBCMissing_Text);
                Message = msgError;
                return;
            }

            Msg msg = new Msg(eMsgLevel.QuestionPrompt, message);
            ShowDialog(msg, "", Keyboard.Numeric, "1", 1);
        }

        public FacilityBookingChargeOverview GetPrintFacilityBookingCharge()
        {
            if (Overview == null || Overview.PostingsFBC == null)
                return null;
            var result = Overview.PostingsFBC.Where(c => c.InwardFacilityChargeID.HasValue).OrderByDescending(c => c.InsertDate).FirstOrDefault();
            if (result == null)
                result = Overview.PostingsFBC.Where(c => c.OutwardFacilityChargeID.HasValue).OrderByDescending(c => c.InsertDate).FirstOrDefault();
            return result;
        }



        #endregion
    }
}
