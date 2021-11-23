using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.webservices;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class PickingPosDetailViewModel : BaseViewModel
    {
        public PickingPosDetailViewModel(PickingPos item = null, Picking pickingItem = null)
        {
            Item = item;
            PickingItem = pickingItem;
            ReadPickingPosCommand = new Command(async () => await ReadPickingPos());
            LoadBarcodeEntityCommand = new Command(async () => await ExecuteLoadBarcodeEntityCommand());
            ReadPostingsCommand = new Command(async () => await ExecuteReadPostingsCommand());
            BookFacilityCommand = new Command(async () => await ExecuteBookFacilityCommand());
        }

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
        public async Task ExecuteBookFacilityCommand()
        {
            if (IsBusy || (BookingQuantity <= 0.00001 && BookingQuantity >= -0.00001))
                return;

            BarcodeEntity barcodeEntity = WSBarcodeEntityResult;
            if (barcodeEntity == null || barcodeEntity.FacilityCharge == null)
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_Relocation_FacilityCharge_Facility.ToString();
                aCMethodBooking.PickingPosID = Item.PickingPosID;
                aCMethodBooking.OutwardQuantity = BookingQuantity;
                aCMethodBooking.OutwardFacilityID = barcodeEntity.FacilityCharge.Facility.FacilityID;
                aCMethodBooking.OutwardFacilityChargeID = barcodeEntity.FacilityCharge.FacilityChargeID;
                aCMethodBooking.InwardQuantity = BookingQuantity;
                aCMethodBooking.InwardFacilityID = Item.ToFacility.FacilityID;
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
                        await ReadPickingPos();
                        BookingMessage = "";
                        if (PickingItem != null && Item != null)
                            PickingItem.ReplacePickingPosItem(Item);
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

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }

        #endregion
    }
}
