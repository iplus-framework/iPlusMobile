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
            //LoadBarcodeEntityCommand = new Command(async () => await ExecuteLoadBarcodeEntityCommand());
            //LoadLocationsCommand = new Command(async () => await ExecuteLoadLocationsCommand());
            //LoadFilteredFacilitiesCommand = new Command(async () => await ExecuteLoadFilteredFacilitiesCommand());
            ReadFacilityChargeCommand = new Command(async () => await ExecuteReadFacilityCharge());
            BookFacilityCommand = new Command(async () => await ExecuteBookFacilityCommand());
            BookZeroStockCommand = new Command(async () => await ExecuteBookZeroStockCommand());
            BookUnZeroStockCommand = new Command(async () => await ExecuteBookUnZeroStockCommand());
            BookBlockQuantCommand = new Command(async () => await ExecuteBookBlockQuantCommand());
            BookReleaseQuantCommand = new Command(async () => await ExecuteBookReleaseQuantCommand());
            BookRelocateCommand = new Command(async () => await ExecuteBookRelocateCommand());
            PrintCommand = new Command(async () => await ExecutePrintCommand());
            SplitQuantCommand = new Command(async () => await ExecuteSplitQuantCommand());
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

        private FacilityCharge _TempFacilityCharge;

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

        public string _QuantSplitNumber;
        public string QuantSplitNumber
        {
            get => _QuantSplitNumber;
            set
            {
                SetProperty(ref _QuantSplitNumber, value);
            }
        }

        public FacilitySelectorViewModel FacilitySelector
        {
            get;
            set;
        }

        private SumQuantityByBarcodeViewModel _SumByBarcodeModel;

        #endregion

        #region Methods
        void RebuildTitle()
        {
            if (Item != null)
                Title = String.Format("{0}: {1}", Item.FacilityLot.LotNo, Item.Material.MaterialName1);
            else
                Title = "QuantOverview";
        }

        public void OnAppear(FacilityCharge facilityCharge)
        {
            if (FacilitySelector != null)
            {
                SelectedFacility = FacilitySelector.SelectedStorageLocation;
                FacilitySelector = null;
            }
            else if (facilityCharge != null)
            {
                Item = facilityCharge;
            }

            double? sumQuantity = GetQuantityFromSumModel();
            if (sumQuantity != null)
            {
                BookingQuantity = sumQuantity.Value;
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

        //public Command ReadFacilityChargeByFacilityMaterialLotCommand { get; set; }
        public async Task<FacilityCharge> ExecuteReadFacilityChargeByFacilityMaterialLot(Guid facilityID, string splitNo)
        {
            if (IsBusy || Item == null)
                return null;

            IsBusy = true;

            int temp = 0;
            string splitNoString = CoreWebServiceConst.EmptyParam;
            if (int.TryParse(splitNo, out temp))
                splitNoString = splitNo;

            try
            {
                var response = await _WebService.GetFacilityChargeFromFacilityMaterialLotAsync(facilityID.ToString(), Item.Material.MaterialID.ToString(), 
                                                                                               Item.FacilityLot.FacilityLotID.ToString(), splitNoString);
                this.WSResponse = response;
                if (response.Suceeded)
                    return response.Data;
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
            return null;
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
                    Message = response.Message;
                else
                {
                    if (response.Data != null && !String.IsNullOrEmpty(response.Data.DetailsAsText))
                        Message = response.Data;
                    else
                    {
                        IsBusy = false;
                        await ExecuteReadFacilityCharge();
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
                    Message = response.Message != null ? response.Message : new Msg(eMsgLevel.Error, "Booking Error");
                else
                {
                    if (response.Data != null && !String.IsNullOrEmpty(response.Data.DetailsAsText))
                        Message = response.Data;
                    else
                    {
                        IsBusy = false;
                        await ExecuteReadFacilityCharge();
                        _TempFacilityCharge = null;
                        _TempFacilityCharge = await ExecuteReadFacilityChargeByFacilityMaterialLot(SelectedFacility.FacilityID, Item.SplitNo.ToString());
                        if (_TempFacilityCharge == null)
                        {
                            Message = new Msg(eMsgLevel.Error, Strings.AppStrings.RelocatedQuantDataMissing_Text);
                        }
                        else
                        {
                            ShowDialog(new Msg(eMsgLevel.QuestionPrompt, Strings.AppStrings.PickingBookSuccAndPrint_Question) { MessageButton = eMsgButton.YesNo }, "", 
                                       Keyboard.Numeric, "1", 2);
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

        public Command PrintCommand { get; set; }
        public async Task<bool> ExecutePrintCommand(int maxPrintJobsInSpooler = 0, PrintEntity printEntity = null)
        {
            if (IsBusy
                || Item == null)
                return false;

            IsBusy = true;
            bool success = false;
            try
            {
                if (printEntity == null || printEntity.Sequence == null)
                {
                    printEntity = new PrintEntity();
                    printEntity.CopyCount = 1;
                    printEntity.MaxPrintJobsInSpooler = maxPrintJobsInSpooler;
                    printEntity.Sequence = new List<BarcodeEntity>()
                    {
                        new BarcodeEntity(){ FacilityCharge = Item }
                    };
                }
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

        public Command SplitQuantCommand { get; set; }
        public async Task ExecuteSplitQuantCommand()
        {
            if (IsBusy || Item == null)
                return;

            IsBusy = true;
            try
            {
                Message = null;

                int splitNo = 0;
                int.TryParse(QuantSplitNumber, out splitNo);

                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_Split_FacilityCharge.ToString();
                aCMethodBooking.OutwardFacilityChargeID = Item.FacilityChargeID;
                aCMethodBooking.OutwardQuantity = BookingQuantity;
                aCMethodBooking.InwardSplitNo = splitNo;

                WSResponse<MsgWithDetails> response = await _WebService.BookFacilityAsync(aCMethodBooking);

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
                        await ExecuteReadFacilityCharge();
                        _TempFacilityCharge = null;
                        _TempFacilityCharge = await ExecuteReadFacilityChargeByFacilityMaterialLot(Item.Facility.FacilityID, QuantSplitNumber);
                        if (_TempFacilityCharge == null)
                        {
                            Message = new Msg(eMsgLevel.Error, Strings.AppStrings.RelocatedQuantDataMissing_Text);
                        }
                        else
                        {
                            ShowDialog(new Msg(eMsgLevel.QuestionPrompt, Strings.AppStrings.PickingBookSuccAndPrint_Question) { MessageButton = eMsgButton.YesNo }, "", 
                                       Keyboard.Numeric, "1", 2);
                        }
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
        }

        #region Methods => SumByBarcode

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
            if (_SumByBarcodeModel != null && (_SumByBarcodeModel.SumQuantity >= 0.00001 || _SumByBarcodeModel.SumQuantity <= 0.00001))
                return _SumByBarcodeModel.SumQuantity;
            return null;
        }

        #endregion

        public async override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            if (DialogOptions.RequestID == 1 && result == Global.MsgResult.Yes)
            {
                await ExecutePrintCommand(3);
            }
            else if (DialogOptions.RequestID == 2)
            {
                if (result == Global.MsgResult.OK && _TempFacilityCharge != null)
                {
                    int copies = 1;
                    int.TryParse(entredValue, out copies);

                    PrintEntity printEntity = new PrintEntity();
                    printEntity.CopyCount = copies;
                    printEntity.MaxPrintJobsInSpooler = 3;
                    printEntity.Sequence = new List<BarcodeEntity>()
                    {
                        new BarcodeEntity(){ FacilityCharge = _TempFacilityCharge }
                    };

                    await ExecutePrintCommand(3, printEntity);

                }
                _TempFacilityCharge = null;
            }
        }
        #endregion

    }
}