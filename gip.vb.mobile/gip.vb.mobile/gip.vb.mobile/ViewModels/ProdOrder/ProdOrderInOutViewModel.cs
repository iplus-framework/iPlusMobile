using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.webservices;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class ProdOrderInOutViewModel : BaseViewModel
    {
        #region c'tors

        public ProdOrderInOutViewModel(bool isInput, ProdOrderPartslistPosRelation relation, ProdOrderPartslistPos intermOrIntermBatch)
        {
            IsInput = isInput;
            PosRelation = relation;
            IntermOrIntermBatch = intermOrIntermBatch;
            RebuildTitle();

            ReadPostingsCommand = new Command(async () => await ExecuteReadPostingsCommand());
            LoadBarcodeEntityCommand = new Command(async () => await ExecuteLoadBarcodeEntityCommand());
            BookFacilityCommand = new Command(async () => await ExecuteBookFacilityCommand());
        }

        #endregion

        #region Properties

        public bool IsInput
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

            if (IsInput)
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
                    if (IsInput)
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
            if (IsBusy || (BookingQuantity <= 0.00001 && BookingQuantity >= -0.00001))
                return;

            if (IsInput)
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
                        await ExecuteReadPostingsCommand();
                        IsBusy = false;
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
                        CurrentBarcodeEntity = null;
                        CurrentBarcode = null;

                        IsBusy = false;
                        await ExecuteReadPostingsCommand();
                        IsBusy = false;
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

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {

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


        #endregion
    }
}
