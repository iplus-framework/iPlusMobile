using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.webservices;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class PickingPosDetailViewModel : PickingBookingBaseViewModel
    {
        #region c'tors

        public PickingPosDetailViewModel(PickingPos item = null, Picking pickingItem = null)
        {
            Item = item;
            PickingItem = pickingItem;

            ReadPickingPosCommand = new Command(async () => await ReadPickingPos());
            LoadBarcodeEntityCommand = new Command(async () => await ExecuteLoadBarcodeEntityCommand());
            ReadPostingsCommand = new Command(async () => await ExecuteReadPostingsCommand());
            BookFacilityCommand = new Command(async () => await ExecuteBookFacilityCommand());
            PrintCommand = new Command(async () => await ExecutePrintCommand());
        }

        #endregion

        #region Properties
        private Picking _PickingItem;
        public Picking PickingItem
        {
            get
            {
                return _PickingItem;
            }
            set
            {
                SetProperty(ref _PickingItem, value);
                RebuildTitle();
            }
        }

        private PickingPos _Item;
        public PickingPos Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetProperty(ref _Item, value);
                RebuildTitle();
                if (_Item != null)
                {
                    double restQuantity = _Item.TargetQuantity - _Item.ActualQuantity;
                    BookingQuantity = restQuantity;
                }
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

        private FacilityBookingChargeOverview _SelectedPosting;
        public FacilityBookingChargeOverview SelectedPosting
        {
            get => _SelectedPosting;
            set
            {
                SetProperty<FacilityBookingChargeOverview>(ref _SelectedPosting, value);
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

        public string _ScanMessage;
        public string ScanMessage
        {
            get
            {
                return _ScanMessage;
            }
            set
            {
                SetProperty(ref _ScanMessage, value);
            }
        }

        private SumQuantityByBarcodeViewModel _SumByBarcodeModel;

        #endregion

        #region Methods
        void RebuildTitle()
        {
            if (Item != null)
                Title = Item.Material.MaterialName1;
            else if (PickingItem != null)
                Title = PickingItem.PickingNo;
            else
                Title = "PickingPosDetailViewModel";
        }

        public Command ReadPickingPosCommand { get; set; }
        public async Task ReadPickingPos()
        {
            if (IsBusy || Item == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetPickingPosAsync(Item.PickingPosID.ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                    Item = response.Data;
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
            return;
        }

        public Command ReadPostingsCommand { get; set; }
        public async Task ExecuteReadPostingsCommand()
        {
            if (IsBusy || Item == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetPickingPosPostingsAsync(Item.PickingPosID.ToString());
                this.WSResponse = response;
                if (response.Suceeded)
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
                {
                    if (CurrentBarcodeEntity != null && CurrentBarcodeEntity.Count() == 1 && Item != null 
                        && (Item.PostingType == PostingTypeEnum.Relocation || Item.PostingType == PostingTypeEnum.NotDefined))
                    {
                        FacilityCharge fcNew = response.Data.ValidEntity as FacilityCharge;
                        if (fcNew != null)
                        {
                            object addedEntity = CurrentBarcodeEntity.FirstOrDefault();
                            FacilityCharge fc = addedEntity as FacilityCharge;
                            if (fc != null)
                            {
                                CurrentBarcodeEntity.Remove(addedEntity);
                            }

                            if (fcNew.NotAvailable)
                            {
                                Message = new Msg(eMsgLevel.Warning, Strings.AppStrings.QuantIsNotAvailableCheck);
                            }

                            double requiredQuantity = Item.TargetQuantity - Item.ActualQuantity;
                            if (requiredQuantity > 0)
                            {
                                if (requiredQuantity > fcNew.StockQuantity)
                                {
                                    BookingQuantity = fcNew.StockQuantity;
                                }
                                else if (BookingQuantity != requiredQuantity)
                                {
                                    BookingQuantity = requiredQuantity;
                                }
                            }
                        }

                        Facility fNew = response.Data.ValidEntity as Facility;
                        if (fNew != null)
                        {
                            object addedEntity = CurrentBarcodeEntity.FirstOrDefault();
                            Facility f = addedEntity as Facility;
                            if (f != null)
                            {
                                CurrentBarcodeEntity.Remove(addedEntity);
                            }
                        }

                        if (fcNew != null || fNew != null)
                        {
                            CurrentBarcodeEntity.Add(response.Data.ValidEntity);
                            CurrentBarcodeEntity = CurrentBarcodeEntity.ToList();
                            ScanMessage = null;
                        }
                    }
                    else
                    {
                        CurrentBarcodeEntity = new List<object> { response.Data.ValidEntity };

                        FacilityCharge fc = response.Data.ValidEntity as FacilityCharge;
                        if (fc != null)
                        {
                            double requiredQuantity = Item.TargetQuantity - Item.ActualQuantity;
                            if (requiredQuantity > 0)
                            {
                                if (requiredQuantity > fc.StockQuantity)
                                {
                                    BookingQuantity = fc.StockQuantity;
                                }
                                else if (BookingQuantity != requiredQuantity)
                                {
                                    BookingQuantity = requiredQuantity;
                                }
                            }
                        }

                        if (Item != null && (Item.PostingType == PostingTypeEnum.Relocation || Item.PostingType == PostingTypeEnum.NotDefined))
                        {
                            var entity = CurrentBarcodeEntity.FirstOrDefault();
                            if (entity != null && entity is FacilityCharge)
                            {
                                ScanMessage = Strings.AppStrings.PickingRelocationScanFacility_Text;
                            }
                            else if (entity is Facility)
                            {
                                ScanMessage = Strings.AppStrings.PickingRelocationScanFacilityCharge_Text;
                            }
                        }
                    }
                }
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
        public async Task ExecuteBookFacilityCommand()
        {
            BookingMessage = null;
            ScanMessage = null;

            if (IsBusy 
                || FacilityConst.IsDoubleZeroForPosting(BookingQuantity))
                return;

            if (BookingQuantity > FacilityConst.C_MaxQuantityPerPosting)
            {
                ShowDialog(new Msg(eMsgLevel.Error, Strings.AppStrings.QuantityPerPostingIsTooLarge_Text));
                return;
            }

            BarcodeEntity barcodeEntity = WSBarcodeEntityResult;
            if (barcodeEntity == null)
                return;

            IsBusy = true;

            try
            {
                ZeroBookFacilityChargeID = null;

                ACMethodBooking aCMethodBooking = new ACMethodBooking();

                if (Item.PostingType == PostingTypeEnum.Inward)
                {
                    PrepareParamForPickingInward(aCMethodBooking, barcodeEntity);
                    aCMethodBooking.InwardQuantity = BookingQuantity;
                }
                else if (Item.PostingType == PostingTypeEnum.Outward)
                {
                    PrepareParamForPickingOutward(aCMethodBooking, barcodeEntity);
                    aCMethodBooking.OutwardQuantity = BookingQuantity;
                }
                else
                {
                    PrepareParamForPickingRelocation(aCMethodBooking, barcodeEntity, Item);
                    aCMethodBooking.OutwardQuantity = BookingQuantity;
                    aCMethodBooking.InwardQuantity = BookingQuantity;
                }

                aCMethodBooking.PickingPosID = Item.PickingPosID;

                var response = await _WebService.BookFacilityAsync(aCMethodBooking);
                this.WSResponse = response;
                if (!response.Suceeded)
                {
                    BookingQuantity = 0;
                    BookingMessage = response.Message != null ? response.Message.Message : "Booking Error";
                    Msg msg = new Msg(eMsgLevel.Error, BookingMessage);
                    ShowDialog(msg);
                }
                else
                {
                    if (response.Data != null && (!String.IsNullOrEmpty(response.Data.DetailsAsText) || response.Data.MessageLevel == eMsgLevel.Question))
                    {
                        BookingQuantity = 0;

                        if (response.Data.MessageLevel == eMsgLevel.Question)
                        {
                            Msg msg = response.Data;
                            msg.MessageButton = eMsgButton.YesNo;
                            ZeroBookFacilityChargeID = aCMethodBooking.OutwardFacilityChargeID;
                            ShowDialog(msg, requestID: 2);
                        }
                        else
                        {
                            BookingMessage = response.Data.DetailsAsText.TrimEnd();
                            Msg msg = new Msg(eMsgLevel.Error, BookingMessage);
                            ShowDialog(msg);
                        }
                    }
                    else
                    {
                        IsBusy = false;
                        await ExecuteReadPostingsCommand();
                        IsBusy = false;
                        await ReadPickingPos();
                        BookingMessage = "";
                        if (PickingItem != null && Item != null)
                            PickingItem.ReplacePickingPosItem(Item);

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

        private void PrepareParamForPickingInward(ACMethodBooking acMethodBooking, BarcodeEntity barcodeEntity)
        {
            acMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_PickingInward;
            if (barcodeEntity != null)
            {
                if (barcodeEntity.FacilityCharge != null)
                {   
                    acMethodBooking.InwardFacilityChargeID = barcodeEntity.FacilityCharge.FacilityChargeID;

                }
                else if (barcodeEntity.Facility != null)
                {
                    acMethodBooking.InwardFacilityID = barcodeEntity.Facility.FacilityID;
                }
            }
        }

        private void PrepareParamForPickingOutward(ACMethodBooking acMethodBooking, BarcodeEntity barcodeEntity)
        {
            acMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_PickingOutward;
            if (barcodeEntity != null)
            {
                if (barcodeEntity.FacilityCharge != null)
                {
                    acMethodBooking.OutwardFacilityChargeID = barcodeEntity.FacilityCharge.FacilityChargeID;
                    acMethodBooking.OutwardFacilityID = barcodeEntity.FacilityCharge.Facility.FacilityID;
                }
                else if (barcodeEntity.Facility != null)
                {
                    acMethodBooking.OutwardFacilityID = barcodeEntity.Facility.FacilityID;
                }
            }
        }

        private void PrepareParamForPickingRelocation(ACMethodBooking acMethodBooking, BarcodeEntity barcodeEntity, PickingPos pickingPos)
        {
            acMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_PickingRelocation;
            if (barcodeEntity != null)
            {
                if (CurrentBarcodeEntity != null && CurrentBarcodeEntity.Count == 2)
                {
                    FacilityCharge fc = CurrentBarcodeEntity.OfType<FacilityCharge>().FirstOrDefault();
                    Facility facility = CurrentBarcodeEntity.OfType<Facility>().FirstOrDefault();
                    if (facility != null && fc != null)
                    {
                        acMethodBooking.InwardFacilityID = facility.FacilityID;
                        acMethodBooking.OutwardFacilityChargeID = fc.FacilityChargeID;
                        acMethodBooking.OutwardFacilityID = fc.Facility.FacilityID;
                    }
                }
                else if (barcodeEntity.FacilityCharge != null)
                {
                    acMethodBooking.OutwardFacilityChargeID = barcodeEntity.FacilityCharge.FacilityChargeID;
                    acMethodBooking.OutwardFacilityID = barcodeEntity.FacilityCharge.Facility.FacilityID;
                }
            }
            if (pickingPos != null && pickingPos.ToFacility != null && pickingPos.ToFacility.FacilityID != Guid.Empty)
            {
                acMethodBooking.InwardFacilityID = pickingPos.ToFacility.FacilityID;
            }
        }

        public Command PrintCommand { get; set; }
        public async Task<bool> ExecutePrintCommand(PrintEntity printEntity = null)
        {

            if (IsBusy
                || Item == null)
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

            ////Labels are not required.
            //if (facilityBookingCharge.InwardFacilityPostingBehaviour == (short)PostingBehaviourEnum.ZeroStockOnRelocation)
            //{
            //    return;
            //}

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

        public SumQuantityByBarcodeViewModel GetSumByBarcodeModel()
        {
            string material = "Material";
            if (Item != null && Item.Material != null)
                material = Item.Material.MaterialName1;

            _SumByBarcodeModel = new SumQuantityByBarcodeViewModel(material);
            return _SumByBarcodeModel;
        }

        public double? GetQuantityFromSumModel()
        {
            if (_SumByBarcodeModel != null 
                && !FacilityConst.IsDoubleZeroForPosting(_SumByBarcodeModel.SumQuantity))
                return _SumByBarcodeModel.SumQuantity;
            return null;
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

                            FacilityCharge fc = new FacilityCharge() { FacilityChargeID = facilityChargeID.Value};
                            printEntity.Sequence = new List<BarcodeEntity>()
                                                    {
                                                        new BarcodeEntity(){ FacilityCharge = fc }
                                                    };

                            await ExecutePrintCommand(printEntity);
                        }
                    }
                }
            }
            else if (DialogOptions.RequestID == 2 )
            {
                if (result == Global.MsgResult.Yes)
                {
                    await ExecuteBookZeroStockCommand();
                }

                await ExecuteReadPostingsCommand();
                IsBusy = false;
                await ReadPickingPos();
                BookingMessage = "";
                if (PickingItem != null && Item != null)
                    PickingItem.ReplacePickingPosItem(Item);

                Print(Strings.AppStrings.PickingBookSuccAndPrint_Question);
            }
        }

        #endregion
    }
}
