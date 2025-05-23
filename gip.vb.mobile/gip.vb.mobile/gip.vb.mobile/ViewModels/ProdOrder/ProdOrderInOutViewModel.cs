// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.webservices;
using gip.vb.mobile.Helpers;
using gip.vb.mobile.Strings;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class ProdOrderInOutViewModel : BaseViewModel, IMaterialUnitRecalcReceiver
    {
        #region c'tors
        BarcodeScanManuModel _FromTaskModel;
        public ProdOrderInOutViewModel(bool isInward, BarcodeScanManuModel taskModel, ProdOrderPartslistPosRelation relation, ProdOrderPartslistPos intermOrIntermBatch, ACMethod wfMethod,
                                       IEnumerable<ProdOrderPartslistPosRelation> components = null)
        {
            _FromTaskModel = taskModel;
            IsInward = isInward;
            PosRelation = relation;
            IntermOrIntermBatch = intermOrIntermBatch;
            RebuildTitle();

            ReadPostingsCommand = new Command(async () => await ExecuteReadPostingsCommand());
            LoadBarcodeEntityCommand = new Command(async () => await ExecuteLoadBarcodeEntityCommand());
            BookFacilityCommand = new Command(async () => await ExecuteBookFacilityCommand());
            PrintCommand = new Command(async () => await ExecutePrintCommand());
            GetMovementReasonsCommand = new Command(async () => await ExecuteGetMovementReasons());
            LoadAvailableFacilityChargesCommand = new Command(async () => await ExecuteLoadAvailableFacilityCharges());

            PostingQuantitySuggestionMode? mode1 = null, mode2 = null;
            string validSeqNo1 = null, validSeqNo2 = null;

            if (wfMethod != null && !isInward)
            {
                ACValue pqsmACValue = wfMethod.ParameterValueList.GetACValue("PostingQuantitySuggestionMode");
                if (pqsmACValue != null)
                {
                    PostingQuantitySuggestionMode pQSM;

                    if (Enum.TryParse<PostingQuantitySuggestionMode>(pqsmACValue.Value.ToString(), out pQSM))
                        mode1 = pQSM;
                }

                ACValue pqsmACValue2 = wfMethod.ParameterValueList.GetACValue("PostingQuantitySuggestionMode2");
                if (pqsmACValue2 != null)
                {
                    PostingQuantitySuggestionMode pQSM2;

                    if (Enum.TryParse<PostingQuantitySuggestionMode>(pqsmACValue2.Value.ToString(), out pQSM2))
                    {
                        mode2 = pQSM2;
                    }
                }

                ACValue seqNo1 = wfMethod.ParameterValueList.GetACValue("ValidSeqNoPostingQSMode");
                if (seqNo1 != null)
                    validSeqNo1 = seqNo1.ParamAsString;

                ACValue seqNo2 = wfMethod.ParameterValueList.GetACValue("ValidSeqNoPostingQSMode2");
                if (seqNo2 != null)
                    validSeqNo2 = seqNo2.ParamAsString;
            }
            _OutwardSuggestionMode = new PostingSuggestionMode(mode1, validSeqNo1, mode2, validSeqNo2);

            _InwardSuggestionMode = 0;
            _InwardPostingSuggestionQ = 0;
            _InwardAutoSplitQuant = 0;
            if (wfMethod != null && isInward)
            {
                ACValue inwardPostSQ = wfMethod.ParameterValueList.GetACValue(GlobalApp.WFParam_InwardPostingSuggestionQ);
                if (inwardPostSQ != null && inwardPostSQ.Value != null)
                {
                    _InwardPostingSuggestionQ = inwardPostSQ.ParamAsDouble;
                    IntermediateNotFinalProductFacility = null;
                    if (Math.Abs(_InwardPostingSuggestionQ) > 0 && !IntermOrIntermBatch.IsFinalMixure)
                    {
                        LoadIntermediateNotFinalProductFacility();
                        BookingQuantity = Math.Abs(_InwardPostingSuggestionQ);
                    }
                }
                else
                {
                    ACValue inwardSMode = wfMethod.ParameterValueList.GetACValue("QuantityPerRack");
                    if (inwardSMode != null && inwardSMode.Value != null)
                    {
                        _InwardSuggestionMode = inwardSMode.ParamAsDouble;
                    }
                }

                ACValue inwardAutoSplitQuant = wfMethod.ParameterValueList.GetACValue("InwardAutoSplitQuant");
                if (inwardAutoSplitQuant != null && inwardAutoSplitQuant.Value != null)
                    _InwardAutoSplitQuant = inwardAutoSplitQuant.ParamAsInt32;

                ACValue allowEditProductionDate = wfMethod.ParameterValueList.GetACValue("AllowEditProductionTime");
                if (allowEditProductionDate != null && allowEditProductionDate.Value != null)
                    AllowEditProductionTime = allowEditProductionDate.ParamAsBoolean;
            }

            _Components = components;

            AvailableFacilityCharges = new ObservableCollection<FacilityCharge>();

            if (_AllowEditProductionTime)
                SelectedProductionDate = DateTime.Now;

            ResetScanSequence();
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

        private Facility _IntermediateNotFinalProductFacility;
        public Facility IntermediateNotFinalProductFacility
        {
            get => _IntermediateNotFinalProductFacility;
            set => SetProperty(ref _IntermediateNotFinalProductFacility, value, nameof(IntermediateNotFinalProductFacility));
        }

        private FacilityBookingChargeOverview _SelectedPosting;
        public FacilityBookingChargeOverview SelectedPosting
        {
            get
            {
                return _SelectedPosting;
            }
            set
            {
                SetProperty(ref _SelectedPosting, value);
            }
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

        public void SetQuantityFromUnitRecalc(double newValue)
        {
            BookingQuantity = newValue;
        }

        public MDUnit BookingUnit
        {
            get
            {
                if (IntermOrIntermBatch == null)
                    return null;
                if (IntermOrIntermBatch.MDUnit != null)
                    return IntermOrIntermBatch.MDUnit;
                if (IntermOrIntermBatch.BookingMaterial != null)
                    return IntermOrIntermBatch.BookingMaterial.BaseMDUnit;
                return IntermOrIntermBatch.ProdOrderPartslist?.Partslist?.Material.BaseMDUnit;
            }
        }

        public MDUnit BookingUnitOut
        {
            get
            {
                if (PosRelation == null || PosRelation.SourcePos == null)
                    return null;
                if (PosRelation.SourcePos.MDUnit != null)
                    return PosRelation.SourcePos.MDUnit;
                if (PosRelation.SourcePos.Material != null)
                    return PosRelation.SourcePos.Material.BaseMDUnit;
                return null;
            }
        }

        public int? _InwardSplitNo;
        public int? InwardSplitNo
        {
            get => _InwardSplitNo;
            set => SetProperty<int?>(ref _InwardSplitNo, value);
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

        private BarcodeSequence _BarcodeSequence;
        public BarcodeSequence BarcodeSequence
        {
            get => _BarcodeSequence;
            set
            {
                _BarcodeSequence = value;
                OnPropertyChanged();
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

        private MDMovementReason _SelectedMovementReason;
        public MDMovementReason SelectedMovementReason
        {
            get => _SelectedMovementReason;
            set
            {
                SetProperty(ref _SelectedMovementReason, value);
            }
        }

        private IEnumerable<MDMovementReason> _MovementReasons;
        public IEnumerable<MDMovementReason> MovementReasons
        {
            get => _MovementReasons;
            set
            {
                SetProperty(ref _MovementReasons, value);
            }
        }

        private PostingSuggestionMode _OutwardSuggestionMode = new PostingSuggestionMode() { QuantityMode = PostingQuantitySuggestionMode.OrderQuantity };
        private double _InwardSuggestionMode;
        private double _InwardPostingSuggestionQ;
        private int _InwardAutoSplitQuant;
        private IEnumerable<ProdOrderPartslistPosRelation> _Components;
        
        private bool _AllowEditProductionTime = false;
        public bool AllowEditProductionTime
        {
            get => _AllowEditProductionTime;
            set
            {
                _AllowEditProductionTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _SelectedProductionDate;
        public DateTime? SelectedProductionDate
        {
            get => _SelectedProductionDate;
            set
            {
                _SelectedProductionDate = value;
                OnPropertyChanged();
            }
        }

        private bool _ShowZeroStockPostings = false;
        public bool ShowZeroStockPostings
        {
            get => _ShowZeroStockPostings;
            set => SetProperty(ref _ShowZeroStockPostings, value);
        }

        #endregion

        #region Mehods & Commands

        public Command ReadPostingsCommand { get; set; }
        public async Task ExecuteReadPostingsCommand()
        {
            if (IsBusy)
                return;

            if (IsInward)
                await ReadPostingsIn();
            else
                await ReadPostingsOut();
        }

        public async Task ReadPostingsIn()
        {
            if (IntermOrIntermBatch == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetProdOrderPosFacilityBookingAsync(IntermOrIntermBatch.ProdOrderPartslistPosID.ToString());
                this.WSResponse = response;
                if (response != null && response.Suceeded)
                {
                    PostingOverview overview = response.Data;
                    if (!ShowZeroStockPostings)
                        overview.PostingsFBC = overview.PostingsFBC.Where(c => c.FacilityBookingTypeIndex != (int)GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge);

                    Overview = overview;
                }
                else
                    Overview = new PostingOverview();

                if (_InwardSuggestionMode < -double.Epsilon)
                {
                    double diff = IntermOrIntermBatch.TargetQuantityUOM - IntermOrIntermBatch.ActualQuantityUOM;
                    if (diff < 0.00001)
                        diff = 0;

                    BookingQuantity = diff;
                }
                else if (_InwardPostingSuggestionQ < -double.Epsilon)
                {
                    BookingQuantity = Math.Abs(_InwardPostingSuggestionQ);
                }
                else if (_InwardPostingSuggestionQ > double.Epsilon)
                {
                    BookingQuantity = _InwardPostingSuggestionQ;
                }
                else if (_InwardPostingSuggestionQ > double.Epsilon)
                {
                    BookingQuantity = _InwardSuggestionMode;
                }
                else
                {
                    BookingQuantity = 0;
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
            await ReadPostingsIn();
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
                BarcodeSequence.CurrentBarcode = CurrentBarcode;

                var response = await _WebService.InvokeBarcodeSequenceAsync(BarcodeSequence);
                BarcodeSequence = response.Data;
                this.WSResponse = response;

                if (response.Suceeded)
                {
                    if (IsInward)
                    {
                        BarcodeEntity facilityEntity = BarcodeSequence.Sequence.Where(c => c.Facility != null).FirstOrDefault();
                        if (facilityEntity != null)
                        {
                            this.WSBarcodeEntityResult = facilityEntity;
                            Facility facility = facilityEntity.Facility;

                            if (facility != null)
                            {
                                var currentFacility = TargetFacilities.FirstOrDefault(c => c.FacilityID == facility.FacilityID);
                                if (currentFacility != null)
                                    CurrentFacility = currentFacility;
                                else
                                {
                                    TargetFacilities.Add(facility);
                                    TargetFacilities = TargetFacilities.ToList();
                                    CurrentFacility = facility;
                                }
                            }
                            ResetScanSequence();
                        }
                    }
                    else
                    {
                        BarcodeEntity facilityChargeEntity = BarcodeSequence.Sequence.Where(c => c.FacilityCharge != null).FirstOrDefault();
                        if (facilityChargeEntity == null)
                        {
                            this.Message = response.Data.Message;
                            CurrentBarcodeEntity = BarcodeSequence.Sequence.Select(c => c.ValidEntity).ToList();
                        }
                        else
                        {
                            this.WSBarcodeEntityResult = facilityChargeEntity;
                            SetOutwardFacilityCharge(facilityChargeEntity.FacilityCharge, false);
                            this.Message = null;
                            ResetScanSequence();
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

        public void SetOutwardFacilityCharge(FacilityCharge fc, bool setOnEntity)
        {
            if (setOnEntity)
                this.WSBarcodeEntityResult = new BarcodeEntity() { FacilityCharge = fc };

            List<object> entries = new List<object>();
            entries.Add(fc);
            CurrentBarcodeEntity = entries;

            if (_OutwardSuggestionMode.QuantityMode != PostingQuantitySuggestionMode.ProportionallyAnotherComp)
                BookingQuantity = 0;

            PostingQuantitySuggestionMode suggestionMode = _OutwardSuggestionMode.GetPostingSuggestionMode(PosRelation.Sequence);

            if (suggestionMode == PostingQuantitySuggestionMode.ForceQuantQuantity)
            {
                double stockQuantity = fc.StockQuantity;

                if (stockQuantity > 0.0001)
                    BookingQuantity = fc.StockQuantity;
            }
            else if (suggestionMode == PostingQuantitySuggestionMode.ProportionallyAnotherComp)
            {
                if (_Components != null && _Components.Count() > 1)
                {
                    var anotherComp = _Components.OrderBy(c => c.Sequence).FirstOrDefault();
                    if (anotherComp != null && anotherComp.ActualQuantityUOM > 0.00001
                        && anotherComp.ProdOrderPartslistPosRelationID != PosRelation.ProdOrderPartslistPosRelationID)
                    {
                        ScaleAccordingAnotherComponent(anotherComp);
                    }
                }
            }
            else if (suggestionMode == PostingQuantitySuggestionMode.None)
            {
                BookingQuantity = 0;
            }
            else if (suggestionMode == PostingQuantitySuggestionMode.OrderQuantity)
            {
                double requiredQuantity = PosRelation.TargetQuantity - PosRelation.ActualQuantityUOM;
                if (requiredQuantity > fc.StockQuantity && fc.StockQuantity > 0.000001)
                {
                    BookingQuantity = fc.StockQuantity;
                }
                else if (requiredQuantity > 0.0000001)
                {
                    BookingQuantity = requiredQuantity;
                }
            }
        }

        public Command BookFacilityCommand { get; set; }
        public async Task ExecuteBookFacilityCommand()
        {
            if (IsBusy
                || FacilityConst.IsDoubleZeroForPosting(BookingQuantity))
                return;

            if (BookingQuantity > FacilityConst.C_MaxQuantityPerPosting || BookingQuantity < -FacilityConst.C_MaxQuantityPerPosting)
            {
                ShowDialog(new Msg(eMsgLevel.Error, Strings.AppStrings.QuantityPerPostingIsTooLarge_Text));
                return;
            }

            if (IsInward)
            {
                await BookFacilityInward();
            }
            else
            {
                FacilityCharge fc = WSBarcodeEntityResult?.FacilityCharge;

                bool isQuantQMissing = fc != null && (BookingQuantity - fc.StockQuantity) > 0.1;

                if (isQuantQMissing)
                {
                    ShowDialog(new Msg(eMsgLevel.Question, Strings.AppStrings.QuantQIsLess_Text) { MessageButton = eMsgButton.YesNo }, "", null, "", 4);
                }
                else
                {
                    await BookFacilityOutward();
                }
            }
        }

        private async Task BookFacilityInward()
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
                if (Math.Abs(_InwardPostingSuggestionQ) > 0 && IntermediateNotFinalProductFacility != null)
                {
                    //var tempResult = await _WebService.GetNFBatchTargetFacilityAsync(CurrentBarcode);
                    aCMethodBooking.InwardFacilityID = IntermediateNotFinalProductFacility.FacilityID;
                    aCMethodBooking.InwardPartslistID = IntermOrIntermBatch.ProdOrderPartslist.Partslist.PartlistID;
                }

                aCMethodBooking.InwardMaterialID = IntermOrIntermBatch.BookingMaterialID;
                aCMethodBooking.InwardFacilityLotID = IntermOrIntermBatch.FacilityLotID;
                aCMethodBooking.MDUnitID = IntermOrIntermBatch.MDUnit?.MDUnitID;
                aCMethodBooking.PropertyACUrl = this.PropertyACUrl;
                aCMethodBooking.MovementReasonID = SelectedMovementReason?.MDMovementReasonID;
                aCMethodBooking.MovementReasonIndex = SelectedMovementReason?.MDMovementReasonIndex;
                aCMethodBooking.InwardAutoSplitQuant = _InwardAutoSplitQuant;
                aCMethodBooking.InwardSplitNo = InwardSplitNo;
                if (_AllowEditProductionTime)
                    aCMethodBooking.ProductionDateNewSublot = SelectedProductionDate;

                PropertyACUrl = null;
                var response = await _WebService.BookFacilityAsync(aCMethodBooking);
                this.WSResponse = response;
                if (!response.Suceeded)
                    Message = response.Message != null ? response.Message : new Msg(eMsgLevel.Error, "Booking Error");
                else
                {
                    if (response.Data != null && !String.IsNullOrEmpty(response.Data.DetailsAsText))
                        Message = new Msg(eMsgLevel.Error, response.Data.DetailsAsText);
                    else
                    {
                        IsBusy = false;
                        await ExecuteReadPostingsCommand();
                        await RefreshIntermOrIntermBatch();
                        if (_InwardPostingSuggestionQ < 0)
                        {
                            FacilityBookingChargeOverview fbcOverview = Overview.PostingsFBC.Where(c => c.InwardFacilityChargeID.HasValue).OrderByDescending(c => c.InsertDate).FirstOrDefault();
                            if (fbcOverview != null && fbcOverview.InwardFacilityChargeID != null)
                            {
                                await MakeChargeUnavailable(fbcOverview.InwardFacilityChargeID ?? Guid.Empty);
                            }
                        }
                        IsBusy = false;
                        Message = new Msg(eMsgLevel.Info, Strings.AppStrings.PostingSuccesful_Text);
                        if (!CurrentFacility.SkipPrintQuestion)
                            Print(Strings.AppStrings.PickingBookSuccAndPrint_Question);

                        SelectedMovementReason = null;
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

        private async Task MakeChargeUnavailable(Guid inwardFacilityChargeID)
        {
            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_ZeroStock_FacilityCharge.ToString();
                aCMethodBooking.InwardFacilityChargeID = inwardFacilityChargeID;
                aCMethodBooking.ZeroStockStateIndex = 2; // MDZeroStockState.ZeroStockStates.SetNotAvailable;
                BookingQuantity = 0;
                var response = await _WebService.BookFacilityAsync(aCMethodBooking);
                this.WSResponse = response;
                if (!response.Suceeded)
                    BookingMessage = response.Message != null ? response.Message.Message : "Booking Error";
                else
                {
                    if (response.Data != null && !String.IsNullOrEmpty(response.Data.DetailsAsText))
                        BookingMessage = response.Data.DetailsAsText;
                    else
                    {
                        IsBusy = false;
                        BookingMessage = "";
                    }
                }

                if (Math.Abs(_InwardPostingSuggestionQ) > 0)
                {
                    BookingQuantity = Math.Abs(_InwardPostingSuggestionQ);
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

        private async Task BookFacilityOutward()
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
                        Message = new Msg(eMsgLevel.Error, response.Data.DetailsAsText);
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
            else if (IntermOrIntermBatch != null && IntermOrIntermBatch.BookingMaterialInfo != null)
                Title = IntermOrIntermBatch.BookingMaterialInfo;
            else if (IntermOrIntermBatch != null && IntermOrIntermBatch.Material != null)
                Title = IntermOrIntermBatch.Material.MaterialName1;
            else
                Title = "ProdOrderInOutViewModel";
        }

        private PrintEntity _LastPrintEntity;

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

                            if (copies > 25)
                            {
                                _LastPrintEntity = printEntity;
                                Msg question = new Msg(eMsgLevel.Question, string.Format(AppStrings.PrintVerificationQuestion, copies));
                                question.MessageButton = eMsgButton.YesNo;
                                ShowDialog(question, "", null, "", 5);
                                return;
                            }

                            await ExecutePrintCommand(printEntity);
                        }
                    }
                }
            }
            else if (DialogOptions.RequestID == 3 && result == Global.MsgResult.Yes)
            {
                if (WSBarcodeEntityResult == null)
                    return;

                FacilityCharge fc = WSBarcodeEntityResult.FacilityCharge;
                if (fc == null)
                    return;

                BookingQuantity = fc.StockQuantity;
            }
            else if (DialogOptions.RequestID == 4 && result == Global.MsgResult.Yes)
            {
                await BookFacilityOutward();
            }
            else if (DialogOptions.RequestID == 5)
            {
                if (result == Global.MsgResult.Yes && _LastPrintEntity != null)
                {
                    await ExecutePrintCommand(_LastPrintEntity);
                }
                _LastPrintEntity = null;
            }

        }

        public async Task LoadTargetFacilities()
        {
            //CurrentFacility = null;
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

            if (SelectedPosting != null)
            {
                return SelectedPosting;
            }
            else
            {
                var result = Overview.PostingsFBC.Where(c => c.InwardFacilityChargeID.HasValue).OrderByDescending(c => c.InsertDate).FirstOrDefault();
                if (result == null)
                    result = Overview.PostingsFBC.Where(c => c.OutwardFacilityChargeID.HasValue).OrderByDescending(c => c.InsertDate).FirstOrDefault();
                return result;
            }
        }

        public Command GetMovementReasonsCommand { get; set; }
        public async Task ExecuteGetMovementReasons()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var response = await _WebService.GetMovementReasonsAsync();
                WSResponse = response;
                if (response.Suceeded)
                {
                    MovementReasons = response.Data;
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

        public void LoadMovementReasons()
        {
            if (MovementReasons == null || !MovementReasons.Any())
                GetMovementReasonsCommand.Execute(null);
        }

        public void TakePostingQuantityFromQuant()
        {
            if (WSBarcodeEntityResult == null)
                return;

            FacilityCharge fc = WSBarcodeEntityResult.FacilityCharge;
            if (fc == null)
                return;

            ShowDialog(new Msg(eMsgLevel.Question, Strings.AppStrings.QuantityFromQuant_Text) { MessageButton = eMsgButton.YesNo }, "", null, "", 3);
        }

        public void ScaleAccordingAnotherComponent(ProdOrderPartslistPosRelation relativeRelToScale)
        {
            double factor = relativeRelToScale.ActualQuantityUOM / relativeRelToScale.TargetQuantityUOM;
            BookingQuantity = (PosRelation.TargetQuantityUOM * factor) - PosRelation.ActualQuantityUOM;
            if (BookingQuantity > 0.00001)
            {
                _OutwardSuggestionMode = new PostingSuggestionMode() { QuantityMode = PostingQuantitySuggestionMode.ProportionallyAnotherComp };
            }
        }

        private async void LoadIntermediateNotFinalProductFacility()
        {
            WSResponse<Facility> response = await _WebService.GetNFBatchTargetFacilityAsync(_FromTaskModel.CurrentBarcode);
            if (!response.Suceeded)
                Message = response.Message != null ? response.Message : new Msg(eMsgLevel.Error, "Intermediate connected machine not found!");
            IntermediateNotFinalProductFacility = response.Data;
        }

        public void ResetScanSequence()
        {
            BarcodeSequence = new BarcodeSequence();
            CurrentBarcode = null;
        }

        private FacilityCharge _SelectedAvailableFC;
        public FacilityCharge SelectedAvailableFC
        {
            get => _SelectedAvailableFC;
            set
            {
                _SelectedAvailableFC = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FacilityCharge> AvailableFacilityCharges { get; set; }
        public Command LoadAvailableFacilityChargesCommand
        {
            get; set;
        }

        public async Task ExecuteLoadAvailableFacilityCharges()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                AvailableFacilityCharges.Clear();

                Guid machineID, relationID;
                if (_FromTaskModel != null && _FromTaskModel.ScannedMachine != null)
                {
                    machineID = _FromTaskModel.ScannedMachine.ACClass.ACClassID;
                }

                relationID = PosRelation.ProdOrderPartslistPosRelationID;

                WSResponse<List<FacilityCharge>> response = await _WebService.GetPOAvailableFCAsync(machineID.ToString(), relationID.ToString());
                if (!response.Suceeded)
                {
                    Message = response.Message;
                    return;
                }

                if (response.Data == null)
                    return;

                foreach (FacilityCharge fc in response.Data)
                {
                    AvailableFacilityCharges.Add(fc);
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


        #endregion
    }
}
