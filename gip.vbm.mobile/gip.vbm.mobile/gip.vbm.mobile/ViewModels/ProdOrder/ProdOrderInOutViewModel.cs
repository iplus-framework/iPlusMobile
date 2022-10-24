using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.facility;
using gip.mes.webservices;
using gip.vbm.mobile.Helpers;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.ViewModels
{
    public class ProdOrderInOutViewModel : BarcodeScanModelBase
    {
        #region c'tors

        public ProdOrderInOutViewModel(bool isInward, ProdOrderPartslistPosRelation relation, ProdOrderPartslistPos intermOrIntermBatch, ACMethod wfMethod,
                                       IEnumerable<ProdOrderPartslistPosRelation> components = null)
        {
            IsInward = isInward;
            PosRelation = relation;
            IntermOrIntermBatch = intermOrIntermBatch;
            RebuildTitle();

            ReadPostingsCommand = new Command(async () => await ExecuteReadPostingsCommand());
            BookFacilityCommand = new Command(async () => await ExecuteBookFacilityCommand());
            PrintCommand = new Command(async () => await ExecutePrintCommand());
            GetMovementReasonsCommand = new Command(async () => await ExecuteGetMovementReasons());

            PostingQuantitySuggestionMode? mode1 = null, mode2 = null;
            string validSeqNo1 = null, validSeqNo2 = null;

            if (wfMethod != null)
            {
                ACValue pqsmACValue = wfMethod.ParameterValueList.GetACValue(nameof(ProdOrderPartslistWFInfo.PostingQSuggestionMode));
                if (pqsmACValue != null)
                    mode1 = pqsmACValue.Value as PostingQuantitySuggestionMode?;

                ACValue pqsmACValue2 = wfMethod.ParameterValueList.GetACValue(nameof(ProdOrderPartslistWFInfo.PostingQSuggestionMode2));
                if (pqsmACValue2 != null)
                    mode2 = pqsmACValue2.Value as PostingQuantitySuggestionMode?;

                ACValue seqNo1 = wfMethod.ParameterValueList.GetACValue("ValidSeqNoPostingQSMode");
                if (seqNo1 != null)
                    validSeqNo1 = seqNo1.ParamAsString;

                ACValue seqNo2 = wfMethod.ParameterValueList.GetACValue("ValidSeqNoPostingQSMode2");
                if (seqNo2 != null)
                    validSeqNo2 = seqNo2.ParamAsString;
            }
            _OutwardSuggestionMode = new PostingSuggestionMode(mode1, validSeqNo1, mode2, validSeqNo2);

            _InwardSuggestionMode = 0;
            _InwardAutoSplitQuant = 0;
            if (wfMethod != null)
            {
                ACValue inwardSMode = wfMethod.ParameterValueList.GetACValue("QuantityPerRack");
                if (inwardSMode != null)
                    _InwardSuggestionMode = inwardSMode.ParamAsDouble;

                ACValue inwardAutoSplitQuant = wfMethod.ParameterValueList.GetACValue("InwardAutoSplitQuant");
                if (inwardAutoSplitQuant != null)
                    _InwardAutoSplitQuant = inwardAutoSplitQuant.ParamAsInt32;
            }

            _Components = components;
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
        private int _InwardAutoSplitQuant;
        private IEnumerable<ProdOrderPartslistPosRelation> _Components;

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
                    Overview = response.Data;
                else
                    Overview = new PostingOverview();

                if (_InwardSuggestionMode < -double.Epsilon)
                {
                    double diff = IntermOrIntermBatch.TargetQuantityUOM - IntermOrIntermBatch.ActualQuantityUOM;
                    if (diff < 0.00001)
                        diff = 0;

                    BookingQuantity = diff;
                }
                else if (_InwardSuggestionMode > double.Epsilon)
                {
                    BookingQuantity = _InwardSuggestionMode;
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

        public virtual async Task<bool> ExecuteDecodeEntityCommand()
        {
            if (IsBusy)
                return false;

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

                            if (_OutwardSuggestionMode.QuantityMode != PostingQuantitySuggestionMode.ProportionallyAnotherComp)
                                BookingQuantity = 0;

                            PostingQuantitySuggestionMode suggestionMode = _OutwardSuggestionMode.GetPostingSuggestionMode(PosRelation.Sequence);

                            if (suggestionMode == PostingQuantitySuggestionMode.ForceQuantQuantity)
                            {
                                double stockQuantity = response.Data.FacilityCharge.StockQuantity;

                                if (stockQuantity > 0.0001)
                                    BookingQuantity = response.Data.FacilityCharge.StockQuantity;
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
                                if (requiredQuantity > response.Data.FacilityCharge.StockQuantity && response.Data.FacilityCharge.StockQuantity > 0.000001)
                                {
                                    BookingQuantity = response.Data.FacilityCharge.StockQuantity;
                                }
                                else if (requiredQuantity > 0.0000001)
                                {
                                    BookingQuantity = requiredQuantity;
                                }
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
            return true;
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
                aCMethodBooking.InwardMaterialID = IntermOrIntermBatch.BookingMaterialID;
                aCMethodBooking.InwardFacilityLotID = IntermOrIntermBatch.FacilityLotID;
                aCMethodBooking.MDUnitID = IntermOrIntermBatch.MDUnit?.MDUnitID;
                aCMethodBooking.PropertyACUrl = this.PropertyACUrl;
                aCMethodBooking.MovementReasonID = SelectedMovementReason?.MDMovementReasonID;
                aCMethodBooking.MovementReasonIndex = SelectedMovementReason?.MDMovementReasonIndex;
                aCMethodBooking.InwardAutoSplitQuant = _InwardAutoSplitQuant;
                aCMethodBooking.InwardSplitNo = InwardSplitNo;
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
                        IsBusy = false;
                        await ExecuteReadPostingsCommand();
                        await RefreshIntermOrIntermBatch();
                        IsBusy = false;
                        Message = new Msg(eMsgLevel.Info, Strings.AppStrings.PostingSuccesful_Text);
                        if (!CurrentFacility.SkipPrintQuestion)
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
            ShowDialog(msg, "", SettingsViewModel.C_NumericKeyBoard, "1", 1);
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

        #endregion
    }
}
