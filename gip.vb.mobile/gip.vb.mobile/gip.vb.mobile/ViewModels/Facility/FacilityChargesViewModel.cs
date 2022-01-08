using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using gip.mes.webservices;
using gip.core.datamodel;
using System.Collections.Generic;
using gip.core.webservices;
using System.Linq;
using gip.core.autocomponent;

namespace gip.vb.mobile.ViewModels
{
    public class FacilityChargesViewModel : BaseViewModel
    {
        public FacilityChargesViewModel()
        {
            LoadBarcodeEntityCommand = new Command(async () => await ExecuteLoadBarcodeEntityCommand());
            LoadLocationsCommand = new Command(async () => await ExecuteLoadLocationsCommand());
            LoadFilteredFacilitiesCommand = new Command(async () => await ExecuteLoadFilteredFacilitiesCommand());
            ReadFacilityChargeCommand = new Command(async () => await ExecuteReadFacilityCharge());
            BookFacilityCommand = new Command(async () => await ExecuteBookFacilityCommand());
            BookZeroStockCommand = new Command(async () => await ExecuteBookZeroStockCommand());
            BookUnZeroStockCommand = new Command(async () => await ExecuteBookUnZeroStockCommand());
            BookBlockQuantCommand = new Command(async () => await ExecuteBookBlockQuantCommand());
            BookReleaseQuantCommand = new Command(async () => await ExecuteBookReleaseQuantCommand());
            BookRelocateCommand = new Command(async () => await ExecuteBookRelocateCommand());
            PrintCommand = new Command(async () => await ExecutePrintCommand());
        }

        #region Properties
        private FacilityCharge _Item;
        public FacilityCharge Item
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

        public string _FacilitySearch;
        private List<Facility> _Locations = new List<Facility>();
        public List<Facility> Locations
        {
            get
            {
                return _Locations;
            }

            set
            {
                SetProperty(ref _Locations, value);
            }
        }

        public Facility _SelectedLocation;
        public Facility SelectedLocation
        {
            get
            {
                return _SelectedLocation;
            }
            set
            {
                SetProperty(ref _SelectedLocation, value);
            }
        }

        public string FacilitySearch
        {
            get
            {
                return _FacilitySearch;
            }
            set
            {
                SetProperty(ref _FacilitySearch, value);
            }
        }

        private List<Facility> _FilteredFacilites = new List<Facility>();
        public List<Facility> FilteredFacilites
        {
            get
            {
                return _FilteredFacilites;
            }

            set
            {
                SetProperty(ref _FilteredFacilites, value);
            }
        }

        private Facility _SelectedFacility;
        public Facility SelectedFacility
        {
            get
            {
                return _SelectedFacility;
            }
            set
            {
                SetProperty(ref _SelectedFacility, value);
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
                Title = String.Format("{0}: {1}", Item.FacilityLot.LotNo, Item.Material.MaterialName1);
            else
                Title = "QuantOverview";
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
                if (response.Suceeded && response.Data.Facility != null)
                {
                    FilteredFacilites = new List<Facility>() { response.Data.Facility };
                    SelectedFacility = response.Data.Facility;
                }
                else
                {
                    FilteredFacilites = new List<Facility>();
                    SelectedFacility = null;
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

        public Command LoadLocationsCommand { get; set; }
        public async Task ExecuteLoadLocationsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.SearchFacilityAsync(CoreWebServiceConst.EmptyParam, CoreWebServiceConst.EmptyParam, "1000");
                this.WSResponse = response;
                if (response.Suceeded)
                    Locations = response.Data;
                else
                    Locations = new List<Facility>();
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

        public Command LoadFilteredFacilitiesCommand { get; set; }
        public async Task ExecuteLoadFilteredFacilitiesCommand()
        {
            if (IsBusy || SelectedLocation == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.SearchFacilityAsync(FacilitySearch, SelectedLocation.FacilityID.ToString(), CoreWebServiceConst.EmptyParam);
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    FilteredFacilites = response.Data;
                    if (FilteredFacilites.Count == 1)
                        SelectedFacility = FilteredFacilites.FirstOrDefault();
                    else
                        SelectedFacility = null;
                }
                else
                {
                    FilteredFacilites = new List<Facility>();
                    SelectedFacility = null;
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

        public Command ReadFacilityChargeCommand { get; set; }
        public async Task ExecuteReadFacilityCharge()
        {
            if (IsBusy || Item == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetFacilityChargeAsync(Item.FacilityChargeID.ToString());
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

        public Command BookFacilityCommand { get; set; }
        public async Task ExecuteBookFacilityCommand()
        {
            if (IsBusy
                || Item == null
                || (BookingQuantity <= 0.00001 && BookingQuantity >= -0.00001))
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                if (BookingQuantity <= -0.00001)
                {
                    aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_OutwardMovement_FacilityCharge.ToString();
                    aCMethodBooking.OutwardQuantity = Math.Abs(BookingQuantity);
                    aCMethodBooking.OutwardFacilityChargeID = Item.FacilityChargeID;
                }
                else
                {
                    aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_InwardMovement_FacilityCharge.ToString();
                    aCMethodBooking.InwardQuantity = Math.Abs(BookingQuantity);
                    aCMethodBooking.InwardFacilityChargeID = Item.FacilityChargeID;
                }
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
                        await ExecuteReadFacilityCharge();
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

        public Command BookZeroStockCommand { get; set; }
        public async Task ExecuteBookZeroStockCommand()
        {
            if (IsBusy
                || Item == null
                || Item.NotAvailable
                )
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_ZeroStock_FacilityCharge.ToString();
                aCMethodBooking.InwardFacilityChargeID = Item.FacilityChargeID;
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
                        await ExecuteReadFacilityCharge();
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


        public Command BookUnZeroStockCommand { get; set; }
        public async Task ExecuteBookUnZeroStockCommand()
        {
            if (IsBusy
                || Item == null
                || !Item.NotAvailable
                )
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_ZeroStock_FacilityCharge.ToString();
                aCMethodBooking.InwardFacilityChargeID = Item.FacilityChargeID;
                aCMethodBooking.ZeroStockStateIndex = 3; // MDZeroStockState.ZeroStockStates.ResetIfNotAvailable;
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
                        await ExecuteReadFacilityCharge();
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

        public Command BookBlockQuantCommand { get; set; }
        public async Task ExecuteBookBlockQuantCommand()
        {
            if (IsBusy
                || Item == null
                || (Item.MDReleaseState != null && Item.MDReleaseState.ReleaseState >= MDReleaseState.ReleaseStates.Locked)
                )
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_ReleaseState_FacilityCharge.ToString();
                aCMethodBooking.InwardFacilityChargeID = Item.FacilityChargeID;
                aCMethodBooking.ReleaseStateIndex = (int)MDReleaseState.ReleaseStates.Locked;
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
                        await ExecuteReadFacilityCharge();
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


        public Command BookReleaseQuantCommand { get; set; }
        public async Task ExecuteBookReleaseQuantCommand()
        {
            if (IsBusy
                || Item == null
                || (Item.MDReleaseState != null && Item.MDReleaseState.ReleaseState < MDReleaseState.ReleaseStates.Locked)
                )
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_ReleaseState_FacilityCharge.ToString();
                aCMethodBooking.InwardFacilityChargeID = Item.FacilityChargeID;
                aCMethodBooking.ReleaseStateIndex = (int)MDReleaseState.ReleaseStates.Free;
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
                        await ExecuteReadFacilityCharge();
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


        public Command BookRelocateCommand { get; set; }
        public async Task ExecuteBookRelocateCommand()
        {
            if (IsBusy
                || Item == null
                || SelectedFacility == null)
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_Relocation_FacilityCharge_Facility.ToString();
                aCMethodBooking.OutwardFacilityChargeID = Item.FacilityChargeID;
                aCMethodBooking.InwardFacilityID = SelectedFacility.FacilityID;
                aCMethodBooking.OutwardQuantity = BookingQuantity;
                aCMethodBooking.InwardQuantity = BookingQuantity;
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
                        await ExecuteReadFacilityCharge();
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

        public Command PrintCommand { get; set; }
        public async Task<bool> ExecutePrintCommand(int maxPrintJobsInSpooler = 0)
        {

            if (IsBusy
                || Item == null)
                return false;

            IsBusy = true;
            bool success = false;
            try
            {
                PrintEntity printEntity = new PrintEntity();
                printEntity.CopyCount = 1;
                printEntity.MaxPrintJobsInSpooler = maxPrintJobsInSpooler;    
                printEntity.Sequence = new List<BarcodeEntity>()
                {
                    new BarcodeEntity(){ FacilityCharge = Item }
                };
                WSResponse<bool> result = await _WebService.Print(printEntity);

                if (result.Suceeded)
                {
                    success = result.Data;
                    Message = result.Message;
                }
                else
                {
                    if (result.Message != null && result.Message.MessageLevel == eMsgLevel.Question)
                    {
                        result.Message.MessageButton = eMsgButton.YesNo;
                        ShowDialog(result.Message, requestID: 1);
                    }
                }
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

        public async override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            if (DialogOptions.RequestID == 1 && result == Global.MsgResult.Yes)
            {
                await ExecutePrintCommand(3);
            }
        }
        #endregion

    }
}