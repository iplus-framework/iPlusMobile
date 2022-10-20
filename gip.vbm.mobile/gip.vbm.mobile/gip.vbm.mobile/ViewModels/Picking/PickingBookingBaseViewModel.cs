using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace gip.vbm.mobile.ViewModels
{
    public abstract class PickingBookingBaseViewModel : BarcodeScanModelBase
    {

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

        public Guid? ZeroBookFacilityChargeID
        {
            get;
            set;
        }

        public async Task ExecuteBookZeroStockCommand()
        {
            if (IsBusy)
                return;

            if (!ZeroBookFacilityChargeID.HasValue)
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_ZeroStock_FacilityCharge.ToString();
                aCMethodBooking.InwardFacilityChargeID = ZeroBookFacilityChargeID.Value;
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


    }
}
