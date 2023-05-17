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
    public class BarcodeScanModelBase : BaseViewModel
    {
        #region c'tors

        public BarcodeScanModelBase(bool addToListOnScan = false)
        {
            ResetScanSequence();
            InvokeBarcodeCommand = new Command(async () => await ExecuteInvokeBarcodeCommand());
            GetBarcodeEntityCommand = new Command(async () => await ExecuteGetBarcodeEntityCommand());
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

        private protected BarcodeSequence _Item;
        public virtual BarcodeSequence Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetProperty(ref _Item, value);
                if (_AddToListOnScan)
                {
                    List<object> barcodeSequence = Item.Sequence.Where(x => x.MsgResult == null && x.ValidEntity != null)
                                            .Select(c => c.ValidEntity)
                                            .ToList();
                    SelectedSequence = null;
                    BarcodeSequence = barcodeSequence;
                }
            }
        }

        public List<object> _BarcodeSequence;
        public List<object> BarcodeSequence
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

        private object _SelectedSequence;
        public object SelectedSequence
        {
            get
            {
                return _SelectedSequence;
            }
            set
            {
                SetProperty(ref _SelectedSequence, value);
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

        public virtual async Task<bool> ExecuteInvokeBarcode()
        {
            return await ExecuteInvokeBarcodeCommand();
        }

        public Command InvokeBarcodeCommand { get; set; }
        public virtual async Task<bool> ExecuteInvokeBarcodeCommand()
        {
            bool success = false;

            if (IsBusy || Item == null)
                return false;

            IsBusy = true;
            Item.BarcodeIssuer = BarcodeIssuer.HasValue ? BarcodeIssuer.Value : BarcodeIssuerEnum.Production;

            try
            {
                if (Item.State == ActionState.SelectionScanAgain && Item.Sequence.Count == 2)
                {
                    var itemToRemove = Item.Sequence.FirstOrDefault(c => c.ValidEntity == null);
                    if (itemToRemove != null)
                        Item.Sequence.Remove(itemToRemove);
                }

                var response = await _WebService.InvokeBarcodeSequenceAsync(Item);
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    core.datamodel.Msg questionMessage = null;
                    if (response.Data.State == mes.webservices.BarcodeSequence.ActionState.Question &&
                        response.Data.Message != null && (response.Data.Message.MessageLevel == eMsgLevel.Question || response.Data.Message.MessageLevel == eMsgLevel.QuestionPrompt))
                        questionMessage = response.Data.Message;

                    if (questionMessage != null)
                        ShowDialog(questionMessage);

                    Item = response.Data;
                    success = response.Suceeded;
                    IsListVisible = true;
                }
                else
                    Item = new BarcodeSequence();
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

        public Command GetBarcodeEntityCommand { get; set; }

        public async Task<bool> ExecuteGetBarcodeEntityCommand()
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
                if (BarcodeSequence != null)
                {
                    if (BarcodeSequence.Any(c => c is BarcodeEntity && (c as BarcodeEntity).Barcode == CurrentBarcode))
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
                        BarcodeSequence = new List<object> { response.Data.ValidEntity };
                        IsListVisible = true;
                    }
                    else
                        BarcodeSequence = new List<object>();
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

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            var res = Item.Sequence.FirstOrDefault(c => c.MsgResult == Global.MsgResult.Cancel);
            if (res != null)
                res.MsgResult = result;

            if (entredValue != null && result == Global.MsgResult.OK)
                Item.Message = new Msg(eMsgLevel.QuestionPrompt, entredValue);

            InvokeBarcodeCommand.Execute(null);
        }

        public virtual void ResetScanSequence()
        {
            _Item = new BarcodeSequence() { Sequence = new List<BarcodeEntity>() };
        }

        public virtual void Clear()
        {
            if (_Item != null)
                _Item.CurrentBarcode = "";
        }
        #endregion
    }
}
