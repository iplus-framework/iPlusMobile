using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using gip.mes.webservices;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;

namespace gip.vb.mobile.ViewModels
{
    public class BarcodeTasksManuViewModel : BaseViewModel
    {
        public BarcodeTasksManuViewModel()
        {
            ResetScanSequence();
            InvokeBarcodeCommand = new Command(async () => await ExecuteInvokeBarcodeCommand());
        }

        #region Properties
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
                OnPropertyChanged();
            }
        }

        private BarcodeSequence _Item;
        public BarcodeSequence Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetProperty(ref _Item, value);
                OnPropertyChanged();
                if (_Item != null 
                    && Item.Sequence != null 
                    && Item.Sequence.Any())
                {
                    List<object> barcodeSequence = Item.Sequence.Where(x => x.MsgResult == null && x.ValidEntity != null)
                                                                .Select(c => c.ValidEntity)
                                                                .ToList();
                    if (Item.State == mes.datamodel.BarcodeSequenceBase.ActionState.Selection)
                    {
                        ProdOrderPartslistWFInfo[] latestOrderInfos = Item.Sequence.Where(c => c.OrderWFInfos != null && c.OrderWFInfos.Any())
                                                        .Select(c => c.OrderWFInfos)
                                                        .LastOrDefault();
                        if (latestOrderInfos != null && latestOrderInfos.Any())
                            barcodeSequence.AddRange(latestOrderInfos);
                    }
                    SelectedSequence = null;
                    BarcodeSequence = barcodeSequence;
                }
                else
                {
                    SelectedSequence = null;
                    BarcodeSequence = new List<object>();
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
        #endregion

        #region Methods
        public Command InvokeBarcodeCommand { get; set; }
        public async Task ExecuteInvokeBarcodeCommand()
        {
            if (IsBusy || Item == null)
                return;

            IsBusy = true;

            try
            {
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

        public void ResetScanSequence()
        {
            _Item = new BarcodeSequence() { Sequence = new List<BarcodeEntity>() };
        }


        public void Clear()
        {
            if (_Item != null)
                _Item.CurrentBarcode = "";
        }
        #endregion

    }
}