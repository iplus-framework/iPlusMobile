using gip.core.datamodel;
using gip.mes.webservices;
using gip.vbm.mobile.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static gip.mes.datamodel.BarcodeSequenceBase;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using static Android.Content.ClipData;

namespace gip.vbm.mobile.ViewModels
{
    public class BarcodeScanModelBase : BaseViewModel
    {
        #region c'tors

        public BarcodeScanModelBase(bool addToListOnScan = false)
        {
            ResetScanSequence();
            InvokeBarcodeSequenceCommand = new Command(async () => await ExecuteInvokeBarcodeSequenceCommand());
            DecodeEntityCommand = new Command(async () => await ExecuteDecodeEntityCommand());
            _AddToListOnScan = addToListOnScan;
        }

        #endregion

        #region Properties

        private bool _AddToListOnScan = false;

        private bool _ZXingIsScanning;
        public bool ZXingIsScanning
        {
            get
            {
                return _ZXingIsScanning;
            }
            set
            {
                SetProperty(ref _ZXingIsScanning, value);
            }
        }

        private protected BarcodeSequence _ExchangedBarcodeSeq;
        /// <summary>
        /// Barcode-Sequences that are exchanged beetween the client and server
        /// The server decodes the barcodes and appends the decoded entitites into the Squence-Collection
        /// </summary>
        public virtual BarcodeSequence ExchangedBarcodeSeq
        {
            get
            {
                return _ExchangedBarcodeSeq;
            }
            set
            {
                SetProperty(ref _ExchangedBarcodeSeq, value);
                if (_AddToListOnScan)
                {
                    List<object> barcodeSequence = ExchangedBarcodeSeq.Sequence.Where(x => x.MsgResult == null && x.ValidEntity != null)
                                            .Select(c => c.ValidEntity)
                                            .ToList();
                    SelectedEntity = null;
                    DecodedEntitiesList = barcodeSequence;
                }
            }
        }

        public List<object> _DecodedEntitiesList;
        /// <summary>
        /// This List contains Entities that have been decoded on server-side
        /// Material, Facility, FacilityCharge...
        /// </summary>
        public List<object> DecodedEntitiesList
        {
            get
            {
                return _DecodedEntitiesList;
            }
            set
            {
                SetProperty(ref _DecodedEntitiesList, value);
            }
        }

        private object _SelectedEntity;
        /// <summary>
        /// Selected object from DecodedEntitiesList
        /// </summary>
        public object SelectedEntity
        {
            get
            {
                return _SelectedEntity;
            }
            set
            {
                SetProperty(ref _SelectedEntity, value);
            }
        }

        private protected bool _IsListVisible;
        /// <summary>
        /// For hide list and reduce used vertical space
        /// </summary>
        public virtual bool IsListVisible
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

        private protected BarcodeIssuerEnum? _BarcodeIssuer;
        /// <summary>
        /// Define initial barcode issuer for barcode sequence mode
        /// </summary>
        public virtual BarcodeIssuerEnum? BarcodeIssuer
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
                }
            }
        }

        private string _CurrentBarcode;
        /// <summary>
        /// Current code in search bar
        /// </summary>
        public virtual string CurrentBarcode
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

        #endregion

        #region Methods

        public Command InvokeBarcodeSequenceCommand { get; set; }
        public virtual async Task<bool> ExecuteInvokeBarcodeSequenceCommand()
        {
            bool success = false;

            if (IsBusy || ExchangedBarcodeSeq == null)
                return false;

            IsBusy = true;
            ExchangedBarcodeSeq.BarcodeIssuer = BarcodeIssuer.HasValue ? BarcodeIssuer.Value : BarcodeIssuerEnum.Production;

            try
            {
                if (ExchangedBarcodeSeq.State == ActionState.SelectionScanAgain && ExchangedBarcodeSeq.Sequence.Count == 2)
                {
                    var itemToRemove = ExchangedBarcodeSeq.Sequence.FirstOrDefault(c => c.ValidEntity == null);
                    if (itemToRemove != null)
                        ExchangedBarcodeSeq.Sequence.Remove(itemToRemove);
                }
                var response = await _WebService.InvokeBarcodeSequenceAsync(ExchangedBarcodeSeq);
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    core.datamodel.Msg questionMessage = null;
                    if (response.Data.State == mes.webservices.BarcodeSequence.ActionState.Question &&
                        response.Data.Message != null && (response.Data.Message.MessageLevel == eMsgLevel.Question || response.Data.Message.MessageLevel == eMsgLevel.QuestionPrompt))
                        questionMessage = response.Data.Message;

                    if (questionMessage != null)
                        ShowDialog(questionMessage);

                    ExchangedBarcodeSeq = response.Data;
                    success = response.Suceeded;
                    IsListVisible = true;
                }
                else
                    ExchangedBarcodeSeq = new BarcodeSequence();
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }

            return success;
        }

        public Command DecodeEntityCommand { get; set; }
        public virtual async Task<bool> ExecuteDecodeEntityCommand()
        {
            if (IsBusy)
                return false;

            bool success = false;
            IsBusy = true;
            IsListVisible = false;
            Message = null;
            bool alreadyFetchedBarcode = false;
            try
            {
                if (DecodedEntitiesList != null)
                {
                    if (DecodedEntitiesList.Any(c => c is BarcodeEntity && (c as BarcodeEntity).Barcode == CurrentBarcode))
                    {
                        Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = AppStrings.BarcodeSequenceAlreadyHaveCode_Text };
                        alreadyFetchedBarcode = true;
                    }
                }
                if (!alreadyFetchedBarcode)
                {
                    var response = await _WebService.GetBarcodeEntityAsync(this.CurrentBarcode);
                    success = response.Suceeded;
                    this.WSResponse = response;
                    if (response.Suceeded && response.Data != null)
                    {
                        DecodedEntitiesList = new List<object> { response.Data.ValidEntity };
                        IsListVisible = true;
                    }
                    else
                        DecodedEntitiesList = new List<object>();
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
            return success;
        }

        public Command CustomBarcodeCommand { get; set; }
        public virtual async Task<bool> ExecuteCustomBarcodeCommand()
        {
            await Task.Run(() => _ = true);
            return true;
        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            var res = ExchangedBarcodeSeq.Sequence.FirstOrDefault(c => c.MsgResult == Global.MsgResult.Cancel);
            if (res != null)
                res.MsgResult = result;

            if (entredValue != null && result == Global.MsgResult.OK)
                ExchangedBarcodeSeq.Message = new Msg(eMsgLevel.QuestionPrompt, entredValue);

            InvokeBarcodeSequenceCommand.Execute(null);
        }

        public virtual void ResetScanSequence()
        {
            _ExchangedBarcodeSeq = new BarcodeSequence() { Sequence = new List<BarcodeEntity>() };
        }

        public virtual void Clear()
        {
            if (_ExchangedBarcodeSeq != null)
                _ExchangedBarcodeSeq.CurrentBarcode = "";
            CurrentBarcode = "";
            SelectedEntity = null;
            DecodedEntitiesList = new List<object>();
            ExchangedBarcodeSeq = new BarcodeSequence();
        }
        #endregion
    }
}
