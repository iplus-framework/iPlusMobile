﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using gip.mes.webservices;
using gip.core.datamodel;
using System.Collections.Generic;
using gip.core.webservices;
using System.Linq;
using gip.core.autocomponent;
using gip.mes.facility;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.ViewModels
{
    public class FacilityChargesViewModel : BaseViewModel, IBarcodeScanFacilityHost
    {
        #region c'tors

        public FacilityChargesViewModel()
        {
            ReadFacilityChargeCommand = new Command(async () => await ExecuteReadFacilityCharge());
            BookFacilityCommand = new Command(async () => await ExecuteBookFacilityCommand());
            BookZeroStockCommand = new Command(async () => await ExecuteBookZeroStockCommand());
            BookUnZeroStockCommand = new Command(async () => await ExecuteBookUnZeroStockCommand());
            BookBlockQuantCommand = new Command(async () => await ExecuteBookBlockQuantCommand());
            BookReleaseQuantCommand = new Command(async () => await ExecuteBookReleaseQuantCommand());
            BookRelocateCommand = new Command(async () => await ExecuteBookRelocateCommand());
            BookReassignCommand = new Command(async () => await ExecuteBookReassignCommand());
            BookReassignRelocateCommand = new Command(async () => await ExecuteBookReassignRelocateCommand());
            PrintCommand = new Command(async () => await ExecutePrintCommand());
            SplitQuantCommand = new Command(async () => await ExecuteSplitQuantCommand());
            GetMovementReasonsCommand = new Command(async () => await ExecuteGetMovementReasons());
            SearchMaterialCommand = new Command(async () => await ExecuteSearchMaterial());
            GetSuggestedMaterials = new Command(async () => await ExecuteGetSuggestedMaterials());

            FacilityScanViewModel = new BarcodeScanFacilityModel(this);
        }

        #endregion

        #region Properties
        private FacilityCharge _FacilityChargeItem;
        public FacilityCharge FacilityChargeItem
        {
            get
            {
                return _FacilityChargeItem;
            }
            set
            {
                SetProperty(ref _FacilityChargeItem, value);
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

        public FacilitySelectorViewModel FacilitySelector
        {
            get;
            set;
        }

        private SumQuantityByBarcodeViewModel _SumByBarcodeModel;

        public BarcodeScanFacilityModel FacilityScanViewModel;

        #region Properties => Reassignment

        private string _MaterialSearchText;
        public string MaterialSearchText
        {
            get => _MaterialSearchText;
            set
            {
                SetProperty(ref _MaterialSearchText, value);
                if (string.IsNullOrEmpty(_MaterialSearchText))
                {
                    SelectedMaterial = null;
                }
            }
        }

        private Material _SelectedMaterial;
        public Material SelectedMaterial
        {
            get => _SelectedMaterial;
            set
            {
                SetProperty(ref _SelectedMaterial, value);
                IsSelectMaterialVisible = false;
            }
        }

        private List<Material> _MaterialList;
        public List<Material> MaterialList
        {
            get => _MaterialList;
            set
            {
                SetProperty(ref _MaterialList, value);
            }
        }

        private bool _IsSelectMaterialVisible;
        public bool IsSelectMaterialVisible
        {
            get => _IsSelectMaterialVisible;
            set => SetProperty(ref _IsSelectMaterialVisible, value);
        }

        #endregion

        #endregion

        #region Methods
        void RebuildTitle()
        {
            if (FacilityChargeItem != null)
                Title = String.Format("{0}: {1}", FacilityChargeItem.FacilityLot.LotNo, FacilityChargeItem.Material.MaterialName1);
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
                FacilityChargeItem = facilityCharge;
            }

            if (MovementReasons == null || !MovementReasons.Any())
                GetMovementReasonsCommand.Execute(null);

            double? sumQuantity = GetQuantityFromSumModel();
            if (sumQuantity != null)
            {
                BookingQuantity = sumQuantity.Value;
            }
        }

        public Command ReadFacilityChargeCommand { get; set; }
        public async Task ExecuteReadFacilityCharge()
        {
            if (IsBusy || FacilityChargeItem == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetFacilityChargeAsync(FacilityChargeItem.FacilityChargeID.ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                    FacilityChargeItem = response.Data;
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
        public async Task<FacilityCharge> ExecuteReadFacilityChargeByFacilityMaterialLot(Guid facilityID, string materialNo, string splitNo)
        {
            if (IsBusy || FacilityChargeItem == null)
                return null;

            IsBusy = true;

            int temp = 0;
            string splitNoString = CoreWebServiceConst.EmptyParam;
            if (int.TryParse(splitNo, out temp))
                splitNoString = splitNo;

            try
            {
                var response = await _WebService.GetFacilityChargeFromFacilityMaterialLotAsync(facilityID.ToString(), materialNo, 
                                                                                               FacilityChargeItem.FacilityLot.FacilityLotID.ToString(), splitNoString);
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
                || FacilityChargeItem == null
                || FacilityConst.IsDoubleZeroForPosting(BookingQuantity))
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                if (BookingQuantity <= -0.00001)
                {
                    aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_OutwardMovement_FacilityCharge.ToString();
                    aCMethodBooking.OutwardQuantity = Math.Abs(BookingQuantity);
                    aCMethodBooking.OutwardFacilityChargeID = FacilityChargeItem.FacilityChargeID;
                }
                else
                {
                    aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_InwardMovement_FacilityCharge.ToString();
                    aCMethodBooking.InwardQuantity = Math.Abs(BookingQuantity);
                    aCMethodBooking.InwardFacilityChargeID = FacilityChargeItem.FacilityChargeID;
                }

                if (SelectedMovementReason != null)
                {
                    aCMethodBooking.MovementReasonID = SelectedMovementReason.MDMovementReasonID;
                    aCMethodBooking.MovementReasonIndex = SelectedMovementReason.MDMovementReasonIndex;
                }

                BookingQuantity = 0;
                SelectedMovementReason = null;
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
                || FacilityChargeItem == null
                || FacilityChargeItem.NotAvailable
                )
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_ZeroStock_FacilityCharge.ToString();
                aCMethodBooking.InwardFacilityChargeID = FacilityChargeItem.FacilityChargeID;
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
                || FacilityChargeItem == null
                || !FacilityChargeItem.NotAvailable
                )
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_ZeroStock_FacilityCharge.ToString();
                aCMethodBooking.InwardFacilityChargeID = FacilityChargeItem.FacilityChargeID;
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
                || FacilityChargeItem == null
                || (FacilityChargeItem.MDReleaseState != null && FacilityChargeItem.MDReleaseState.ReleaseState >= MDReleaseState.ReleaseStates.Locked)
                )
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_ReleaseState_FacilityCharge.ToString();
                aCMethodBooking.InwardFacilityChargeID = FacilityChargeItem.FacilityChargeID;
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
                || FacilityChargeItem == null
                || (FacilityChargeItem.MDReleaseState != null && FacilityChargeItem.MDReleaseState.ReleaseState < MDReleaseState.ReleaseStates.Locked)
                )
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_ReleaseState_FacilityCharge.ToString();
                aCMethodBooking.InwardFacilityChargeID = FacilityChargeItem.FacilityChargeID;
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
                || FacilityChargeItem == null
                || SelectedFacility == null)
                return;

            if (FacilityConst.IsDoubleZeroForPosting(BookingQuantity))
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_Relocation_FacilityCharge_Facility.ToString();
                aCMethodBooking.OutwardFacilityChargeID = FacilityChargeItem.FacilityChargeID;
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
                        _TempFacilityCharge = await ExecuteReadFacilityChargeByFacilityMaterialLot(SelectedFacility.FacilityID, FacilityChargeItem.Material.MaterialID.ToString(), FacilityChargeItem.SplitNo.ToString());
                        if (_TempFacilityCharge == null)
                        {
                            Message = new Msg(eMsgLevel.Error, Strings.AppStrings.RelocatedQuantDataMissing_Text);
                        }
                        else
                        {
                            ShowDialog(new Msg(eMsgLevel.QuestionPrompt, Strings.AppStrings.PickingBookSuccAndPrint_Question) { MessageButton = eMsgButton.YesNo }, "",
                                       SettingsViewModel.C_NumericKeyBoard, "1", 2);
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

        public void ShowPrintDialog()
        {
            ShowDialog(new Msg(eMsgLevel.QuestionPrompt, Strings.AppStrings.PrintCopies_Question) { MessageButton = eMsgButton.OKCancel }, "", SettingsViewModel.C_NumericKeyBoard, "1", 3);
        }

        public Command PrintCommand { get; set; }
        public async Task<bool> ExecutePrintCommand(int maxPrintJobsInSpooler = 0, PrintEntity printEntity = null, int copyCount = 1)
        {
            if (IsBusy
                || FacilityChargeItem == null)
                return false;

            IsBusy = true;
            bool success = false;
            try
            {
                if (printEntity == null || printEntity.Sequence == null)
                {
                    printEntity = new PrintEntity();
                    printEntity.CopyCount = copyCount;
                    printEntity.MaxPrintJobsInSpooler = maxPrintJobsInSpooler;
                    printEntity.Sequence = new List<BarcodeEntity>()
                    {
                        new BarcodeEntity(){ FacilityCharge = FacilityChargeItem }
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
            if (IsBusy || FacilityChargeItem == null)
                return;

            IsBusy = true;
            try
            {
                Message = null;

                int splitNo = 0;
                int.TryParse(QuantSplitNumber, out splitNo);

                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_Split_FacilityCharge.ToString();
                aCMethodBooking.OutwardFacilityChargeID = FacilityChargeItem.FacilityChargeID;
                aCMethodBooking.OutwardQuantity = BookingQuantity;
                if (splitNo == 0)
                    aCMethodBooking.InwardAutoSplitQuant = 1;
                else
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
                        _TempFacilityCharge = await ExecuteReadFacilityChargeByFacilityMaterialLot(FacilityChargeItem.Facility.FacilityID, FacilityChargeItem.Material.MaterialID.ToString(), QuantSplitNumber);
                        if (_TempFacilityCharge == null)
                        {
                            Message = new Msg(eMsgLevel.Error, Strings.AppStrings.RelocatedQuantDataMissing_Text);
                        }
                        else
                        {
                            ShowDialog(new Msg(eMsgLevel.QuestionPrompt, Strings.AppStrings.PickingBookSuccAndPrint_Question) { MessageButton = eMsgButton.YesNo }, "",
                                       SettingsViewModel.C_NumericKeyBoard, "1", 2);
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

        #region Methods => SumByBarcode

        public SumQuantityByBarcodeViewModel GetSumByBarcodeModel()
        {
            string material = "Material";
            if (FacilityChargeItem != null && FacilityChargeItem.Material != null)
                material = FacilityChargeItem.Material.MaterialName1;

            _SumByBarcodeModel = new SumQuantityByBarcodeViewModel(material);
            return _SumByBarcodeModel;
        }

        public double? GetQuantityFromSumModel()
        {
            if (   _SumByBarcodeModel != null 
                && !FacilityConst.IsDoubleZeroForPosting(_SumByBarcodeModel.SumQuantity))
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
            else if (DialogOptions.RequestID == 3 && result == Global.MsgResult.OK)
            {
                int copyCount = 1;
                int.TryParse(entredValue, out copyCount);

                await (ExecutePrintCommand(3, null, copyCount));
            }
        }

        public void OnFacilityScanned(Facility facility)
        {
            SelectedFacility = facility;
        }

        public void OnSearchFacility(string searchText)
        {
            
        }

        public void TakeBookingQuantityFromQuant()
        {
            BookingQuantity = FacilityChargeItem.StockQuantity;
            if (_BookingQuantity != FacilityChargeItem.StockQuantity)
                BookingQuantity = FacilityChargeItem.StockQuantity;
        }

        #region Reassignment

        public Command SearchMaterialCommand { get; set; }
        public async Task ExecuteSearchMaterial()
        {
            if (IsBusy || string.IsNullOrEmpty(MaterialSearchText))
                return;

            try
            {
                IsBusy = true;

                var response = await _WebService.SearchMaterialAsync(MaterialSearchText);
                if (response.Suceeded)
                {
                    MaterialList = response.Data;
                }
                else if (response.Message != null)
                {
                    Message = response.Message;
                }

                IsSelectMaterialVisible = true;
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

        public Command GetSuggestedMaterials { get; set; }
        public async Task ExecuteGetSuggestedMaterials()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var response = await _WebService.GetSuggestedMaterialsAsync(FacilityChargeItem.Material.MaterialID.ToString());
                if (response.Suceeded)
                {
                    MaterialList = response.Data;
                    if (MaterialList.Count == 1)
                    {
                        SelectedMaterial = MaterialList.FirstOrDefault();
                        MaterialSearchText = SelectedMaterial.MaterialNo + " " + SelectedMaterial.MaterialName1;
                        return;
                    }
                }
                else if (response.Message != null)
                {
                    Message = response.Message;
                }

                IsSelectMaterialVisible = true;
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

        public Command BookReassignCommand { get; set; }
        public async Task ExecuteBookReassignCommand()
        {
            if (IsBusy
                || FacilityChargeItem == null
                || SelectedMaterial == null)
                return;

            if (FacilityConst.IsDoubleZeroForPosting(BookingQuantity))
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_Reassign_FacilityCharge.ToString();
                aCMethodBooking.OutwardFacilityChargeID = FacilityChargeItem.FacilityChargeID;
                aCMethodBooking.InwardMaterialID = SelectedMaterial.MaterialID;
                aCMethodBooking.OutwardQuantity = BookingQuantity;
                BookingQuantity = 0;
                var response = await _WebService.BookFacilityAsync(aCMethodBooking);
                this.WSResponse = response;
                if (!response.Suceeded)
                    Message = response.Message != null ? response.Message : new Msg(eMsgLevel.Error, "Booking Error");
                else
                {
                    if (response.Data != null && !String.IsNullOrEmpty(response.Data.DetailsAsText))
                    {
                        MsgWithDetails resp = response.Data;
                        if (string.IsNullOrEmpty(resp.Message))
                            resp.Message = resp.DetailsAsText;

                        Message = resp;
                    }
                    else
                    {
                        IsBusy = false;
                        await ExecuteReadFacilityCharge();
                        _TempFacilityCharge = null;
                        _TempFacilityCharge = await ExecuteReadFacilityChargeByFacilityMaterialLot(FacilityChargeItem.Facility.FacilityID, SelectedMaterial.MaterialID.ToString(), FacilityChargeItem.SplitNo.ToString());
                        if (_TempFacilityCharge == null)
                        {
                            Message = new Msg(eMsgLevel.Error, Strings.AppStrings.RelocatedQuantDataMissing_Text);
                        }
                        else
                        {
                            ShowDialog(new Msg(eMsgLevel.QuestionPrompt, Strings.AppStrings.PickingBookSuccAndPrint_Question) { MessageButton = eMsgButton.YesNo }, "",
                                       SettingsViewModel.C_NumericKeyBoard, "1", 2);
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

        public Command BookReassignRelocateCommand { get; set; }
        public async Task ExecuteBookReassignRelocateCommand()
        {
            if (IsBusy
                || FacilityChargeItem == null
                || SelectedMaterial == null
                || SelectedFacility == null)
                return;

            if (FacilityConst.IsDoubleZeroForPosting(BookingQuantity))
                return;

            IsBusy = true;

            try
            {
                ACMethodBooking aCMethodBooking = new ACMethodBooking();
                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_Reassign_FacilityCharge.ToString();
                aCMethodBooking.OutwardFacilityChargeID = FacilityChargeItem.FacilityChargeID;
                aCMethodBooking.InwardMaterialID = SelectedMaterial.MaterialID;
                aCMethodBooking.OutwardQuantity = BookingQuantity;
                
                var response = await _WebService.BookFacilityAsync(aCMethodBooking);
                this.WSResponse = response;
                if (!response.Suceeded)
                {
                    BookingQuantity = 0;
                    Message = response.Message != null ? response.Message : new Msg(eMsgLevel.Error, "Booking Error");
                }
                else
                {
                    if (response.Data != null && !String.IsNullOrEmpty(response.Data.DetailsAsText))
                    {
                        MsgWithDetails resp = response.Data;
                        if (string.IsNullOrEmpty(resp.Message))
                            resp.Message = resp.DetailsAsText;

                        Message = resp;
                    }
                    else
                    {
                        IsBusy = false;
                        await ExecuteReadFacilityCharge();
                        var tempFacilityCharge = await ExecuteReadFacilityChargeByFacilityMaterialLot(FacilityChargeItem.Facility.FacilityID, SelectedMaterial.MaterialID.ToString(), FacilityChargeItem.SplitNo.ToString());
                        if (tempFacilityCharge == null)
                        {
                            Message = new Msg(eMsgLevel.Error, "Reassignment is not successful!");
                        }
                        else
                        {
                            aCMethodBooking = new ACMethodBooking();
                            aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_Relocation_FacilityCharge_Facility.ToString();
                            aCMethodBooking.OutwardFacilityChargeID = tempFacilityCharge.FacilityChargeID;
                            aCMethodBooking.InwardFacilityID = SelectedFacility.FacilityID;
                            aCMethodBooking.OutwardQuantity = BookingQuantity;
                            aCMethodBooking.InwardQuantity = BookingQuantity;
                            BookingQuantity = 0;
                            response = await _WebService.BookFacilityAsync(aCMethodBooking);
                            this.WSResponse = response;
                            if (!response.Suceeded)
                                Message = response.Message != null ? response.Message : new Msg(eMsgLevel.Error, "Booking Error");
                            else
                            {
                                if (response.Data != null && !String.IsNullOrEmpty(response.Data.DetailsAsText))
                                {
                                    MsgWithDetails resp = response.Data;
                                    if (string.IsNullOrEmpty(resp.Message))
                                        resp.Message = resp.DetailsAsText;

                                    Message = resp;
                                }
                                else
                                {
                                    IsBusy = false;
                                    await ExecuteReadFacilityCharge();
                                    _TempFacilityCharge = null;
                                    _TempFacilityCharge = await ExecuteReadFacilityChargeByFacilityMaterialLot(SelectedFacility.FacilityID, tempFacilityCharge.Material.MaterialID.ToString(),
                                                                                                                                            tempFacilityCharge.SplitNo.ToString());
                                    if (_TempFacilityCharge == null)
                                    {
                                        Message = new Msg(eMsgLevel.Error, Strings.AppStrings.RelocatedQuantDataMissing_Text);
                                    }
                                    else
                                    {
                                        ShowDialog(new Msg(eMsgLevel.QuestionPrompt, Strings.AppStrings.PickingBookSuccAndPrint_Question) { MessageButton = eMsgButton.YesNo }, "",
                                                   SettingsViewModel.C_NumericKeyBoard, "1", 2);
                                    }
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
        }

        #endregion

        #endregion

    }
}