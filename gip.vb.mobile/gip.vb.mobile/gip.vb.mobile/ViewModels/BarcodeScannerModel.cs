using gip.core.datamodel;
using gip.mes.webservices;
using gip.vb.mobile.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static gip.mes.datamodel.BarcodeSequenceBase;

namespace gip.vb.mobile.ViewModels
{

    /// <summary>
    /// BarcodeScaner component model
    /// </summary>
    public class BarcodeScannerModel : BaseViewModel
    {

        #region ctor's
        public BarcodeScannerModel()
        {
            LoadBarcodeEntityCommand = new Command(async () => await ExecuteLoadBarcodeEntityCommand());
            LoadInvokeBarcodeSequence = new Command(async () => await ExecuteInvokeBarcodeSequence());

            BarcodeSequence = new BarcodeSequence() { };
        }

        #endregion


        #region Impl. BaseViewModel
        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {

        }

        #endregion

        #region Commands

        /// <summary>
        /// Get single barcode entity command
        /// </summary>
        public Command LoadBarcodeEntityCommand { get; set; }
        /// <summary>
        /// Work with barcode sequence command
        /// </summary>
        public Command LoadInvokeBarcodeSequence { get; set; }

        #endregion

        #region Parameters
        private BarcodeIssuerEnum? _BarcodeIssuer;
        /// <summary>
        /// Define initial barcode issuer for barcode sequence mode
        /// </summary>
        public BarcodeIssuerEnum? BarcodeIssuer
        {
            get
            {
                return _BarcodeIssuer;
            }
            set
            {
                if (_BarcodeIssuer != value)
                {
                    _BarcodeIssuer = value;
                    if (_BarcodeIssuer != null)
                        BarcodeSequence = new BarcodeSequence()
                        {
                            BarcodeIssuer = _BarcodeIssuer.Value
                        };
                    else
                        BarcodeSequence = null;
                }
            }
        }

        private bool _ZXingIsScanning;

        /// <summary>
        /// Flag is ZXing scanner used
        /// </summary>
        public bool ZXingIsScanning
        {
            get
            {
                return _ZXingIsScanning;
            }
            set
            {
                SetProperty(ref _ZXingIsScanning, value);
                OnPropertyChanged();
            }
        }

        private string _CurrentBarcode;
        /// <summary>
        /// Current code in search bar
        /// </summary>
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

        public bool _IsListVisible;
        /// <summary>
        /// For hide list and reduce used vertical space
        /// </summary>
        public bool IsListVisible
        {
            get
            {
                return _IsListVisible;
            }
            set
            {
                SetProperty(ref _IsListVisible, value);
            }
        }

        public List<object> _CurrentBarcodeEntity;
        /// <summary>
        /// Founded Entities in search entity mode
        /// </summary>
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


        private BarcodeSequence _BarcodeSequence;
        /// <summary>
        /// Barcode sequence returned in barcode sequence work mode
        /// </summary>
        public BarcodeSequence BarcodeSequence
        {
            get
            {
                return _BarcodeSequence;
            }
            set
            {
                SetProperty(ref _BarcodeSequence, value);
            }
        }

        #endregion

        #region Tasks

        /// <summary>
        /// Async serarch barcode 
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteLoadBarcodeEntityCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            IsListVisible = false;
            bool alreadyFetchedBarcode = false;
            try
            {
                if (CurrentBarcodeEntity != null)
                {
                    if (CurrentBarcodeEntity.Any(c => c is BarcodeEntity && (c as BarcodeEntity).Barcode == CurrentBarcode))
                    {
                        Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = AppStrings.BarcodeSequenceAlreadyHaveCode_Text };
                        alreadyFetchedBarcode = true;
                    }
                }
                if (!alreadyFetchedBarcode)
                {
                    var response = await _WebService.GetBarcodeEntityAsync(this.CurrentBarcode);
                    this.WSResponse = response;
                    if (response.Suceeded)
                    {
                        CurrentBarcodeEntity = new List<object> { response.Data.ValidEntity };
                        IsListVisible = true;
                    }
                    else
                        CurrentBarcodeEntity = new List<object>();
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

        /// <summary>
        /// Async search barcode sequence
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteInvokeBarcodeSequence()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                if (BarcodeSequence.State == ActionState.ScanAgain)
                {
                    if (
                            BarcodeSequence.Sequence == null
                            || !BarcodeSequence.Sequence.Any(c => c.Barcode.ToLower() == CurrentBarcode.ToLower())
                       )
                    {
                        IsListVisible = false;
                        CurrentBarcodeEntity = null;
                        List<object> barcodeEntryTemp = new List<object>();
                        BarcodeSequence.CurrentBarcode = CurrentBarcode;
                        var response = await _WebService.InvokeBarcodeSequenceAsync(BarcodeSequence);
                        WSResponse = response;
                        if (response.Suceeded && response.Data != null)
                        {
                            BarcodeSequence = response.Data;
                            if (BarcodeSequence.Sequence != null)
                                foreach (BarcodeEntity item in BarcodeSequence.Sequence)
                                    barcodeEntryTemp.Add(item.ValidEntity);
                            CurrentBarcodeEntity = barcodeEntryTemp;
                            Message = response.Data.Message; // now use sequence related message
                            IsListVisible = true;
                        }
                    }
                    else
                        Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = AppStrings.BarcodeSequenceAlreadyHaveCode_Text };

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

        #region Methods
        /// <summary>
        ///  Clean model for new use
        /// </summary>
        public void Clean()
        {
            CurrentBarcode = null;
            CurrentBarcodeEntity = null;
            ZXingIsScanning = false;
            IsListVisible = false;
            BarcodeSequence = null;
            if (BarcodeIssuer != null)
                BarcodeSequence = new BarcodeSequence() { BarcodeIssuer = BarcodeIssuer.Value };
        }
        #endregion
    }
}
