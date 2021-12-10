﻿using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace gip.vb.mobile.ViewModels
{
    public class PickingByMaterialViewModel : BaseViewModel
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
                    MissingBookingQuanity = WSBarcodeEntityResult.FacilityCharge.StockQuantity - _TotalBookingQantity;
                }
            }
        }

        private double _MissingBookingQuanity;
        public double MissingBookingQuanity
        {
            get => _MissingBookingQuanity;
            set
            {
                SetProperty<double>(ref _MissingBookingQuanity, value);
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

                if (_WSBarcodeEntityResult != null && _WSBarcodeEntityResult.FacilityCharge != null)
                {
                    MissingBookingQuanity = WSBarcodeEntityResult.FacilityCharge.StockQuantity - _TotalBookingQantity;
                }
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
            if (IsBusy || Item == null || Item.PickingItems == null)
                return;

            if (MissingBookingQuanity < 0)
            {
                //error 
                return;
            }

            BarcodeEntity barcodeEntity = WSBarcodeEntityResult;
            if (barcodeEntity == null)
                return;

            IsBusy = true;

            try
            {
                ACMethodBookingList bookings = new ACMethodBookingList();

                foreach (PickingPos pp in Item.PickingItems)
                {
                    if (pp.PostingQuantity > 0.0001)
                    {
                        ACMethodBooking aCMethodBooking = new ACMethodBooking();
                        aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_Relocation_FacilityCharge_Facility;
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

                var response = await _WebService.BookFacilitiesAsync(bookings);
                this.WSResponse = response;

                BookingMessage = response.Data.DetailsAsText;

                if (response.Suceeded)
                {
                    IsBusy = false;
                    //await ExecuteReadPostingsCommand();
                    IsBusy = false;
                    //await ReadPickingPos();
                    BookingMessage = "";
                    //if (PickingItem != null && Item != null)
                    //    PickingItem.ReplacePickingPosItem(Item);
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
            Msg msg = new Msg(eMsgLevel.QuestionPrompt, "Change the posting quantity: ");
            ShowDialog(msg, "", Keyboard.Numeric, SelectedPickingPos?.PostingQuantity.ToString(), 1);
        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
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
        }
    }
}
