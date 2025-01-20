using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.webservices;
using gip.vb.mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static gip.mes.datamodel.BarcodeSequenceBase;

namespace gip.vb.mobile.ViewModels
{
    public class BarcodeScanManuModel : BarcodeScanModelBase
    {
        public BarcodeScanManuModel() : base()
        {
            InvokeVerifyOrderCommand = new Command(async () => await ExecuteInvokeVerifyOrderCommand());
        }

        public override BarcodeSequence Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetProperty(ref _Item, value);
                if (_Item != null
                    && Item.Sequence != null
                    && Item.Sequence.Any())
                {
                    object prevBarcodeSeq = BarcodeSequence;
                    object prevSelectedSequence = SelectedSequence;
                    object newSelectedSequence = null;

                    List<object> barcodeSequence = Item.Sequence.Where(x => x.MsgResult == null && x.ValidEntity != null)
                                                                .Select(c => c.ValidEntity)
                                                                .ToList();
                    if (   Item.State == mes.datamodel.BarcodeSequenceBase.ActionState.Selection
                        || Item.State == ActionState.FastSelection
                        || Item.State == ActionState.SelectionScanAgain
                        || Item.State == mes.datamodel.BarcodeSequenceBase.ActionState.Completed)
                    {
                        List<BarcodeEntity> barcodeEntitiesWithOrderInfos =  Item.Sequence.Where(c => c.OrderWFInfos != null && c.OrderWFInfos.Any()).ToList();
                        foreach (BarcodeEntity barcodeEntity in barcodeEntitiesWithOrderInfos)
                        {
                            bool refreshedWithSameReference = false;
                            //ProdOrderPartslistWFInfo[] latestOrderInfos = barcodeEntitiesWithOrderInfos.Select(c => c.OrderWFInfos).LastOrDefault();
                            var orderInfos = barcodeEntity.OrderWFInfos.ToList();
                            foreach (var wfOrderInfo in barcodeEntity.OrderWFInfos)
                            {
                                ProdOrderPartslistWFInfo existingWFInfo =
                                barcodeSequence.Where(c => c is ProdOrderPartslistWFInfo
                                                            && (c as ProdOrderPartslistWFInfo).ACUrlWF == wfOrderInfo.ACUrlWF
                                                            && (c as ProdOrderPartslistWFInfo).IntermediateBatch != null
                                                            && wfOrderInfo.IntermediateBatch != null
                                                            && (c as ProdOrderPartslistWFInfo).IntermediateBatch.ProdOrderPartslistPosID == wfOrderInfo.IntermediateBatch.ProdOrderPartslistPosID)
                                                .OfType<ProdOrderPartslistWFInfo>()
                                                .FirstOrDefault();
                                if (existingWFInfo == null)
                                    barcodeSequence.Add(wfOrderInfo);
                                else
                                {
                                    orderInfos.Remove(wfOrderInfo);
                                    orderInfos.Add(existingWFInfo);
                                    refreshedWithSameReference = true;
                                }
                            }
                            if (refreshedWithSameReference)
                                barcodeEntity.OrderWFInfos = orderInfos.ToArray();
                        }
                    }
                    foreach (ProdOrderPartslistWFInfo wfInfo in barcodeSequence.Where(c => c is ProdOrderPartslistWFInfo))
                    {
                        if (wfInfo.WFMethod != null)
                            wfInfo.WFMethod.UseCultureInfoForConversion = true;
                        if (newSelectedSequence == null && prevSelectedSequence != null && prevSelectedSequence is ProdOrderPartslistWFInfo)
                        {
                            ProdOrderPartslistWFInfo wfInfoPrev = prevSelectedSequence as ProdOrderPartslistWFInfo;
                            if (wfInfoPrev.IntermediateBatch != null && wfInfo.IntermediateBatch != null && wfInfoPrev.IntermediateBatch.ProdOrderPartslistPosID == wfInfo.IntermediateBatch.ProdOrderPartslistPosID)
                                newSelectedSequence = wfInfo;
                        }
                    }

                    SelectedSequence = newSelectedSequence;
                    BarcodeSequence = barcodeSequence;
                    _TempSequenceList = null;
                }
                else
                {
                    SelectedSequence = null;
                    BarcodeSequence = new List<object>();
                    _TempSequenceList = null;
                }
            }
        }

        public void ReleaseMachine()
        {
            InvokeVerifyOrderCommand.Execute(null);
        }

        public async Task PauseOrderOnMachine()
        {
            ProdOrderPartslistWFInfo wfInfo = SelectedSequence as ProdOrderPartslistWFInfo;
            wfInfo.InfoState = POPartslistInfoState.Pause;
            await InvokeActionOnMachine();
        }

        public async Task InvokeActionOnMachine()
        {
            if (Item.State == ActionState.SelectionScanAgain)
                Item.State = ActionState.Selection;

            ProdOrderPartslistWFInfo wfInfo = SelectedSequence as ProdOrderPartslistWFInfo;
            BarcodeEntity entity = Item.Sequence.LastOrDefault();
            if (entity == null)
            {
                ResetScanSequence();
                return;
            }
            entity.SelectedOrderWF = wfInfo;

            bool success = await ExecuteInvokeBarcodeCommand();
            if (success && Item != null && (Item.Message == null || Item.Message.MessageLevel < eMsgLevel.Warning))
            {
                if (ScannedMachine != null && Item.CurrentBarcode != ScannedMachine.Barcode)
                    Item.CurrentBarcode = ScannedMachine.Barcode;

                success = await ExecuteInvokeBarcodeCommand();
            }
        }

        public async Task InvokeMachineMalfuction(bool reset)
        {
            BarcodeEntity bEntity = Item.Sequence.FirstOrDefault(c => c.ACClass != null);

            if (bEntity != null)
            {
                if (reset)
                    bEntity.MachineMalfunction = false;
                else
                    bEntity.MachineMalfunction = true;
            }

            BarcodeEntity entity = Item.Sequence.LastOrDefault();
            if (entity == null)
            {
                ResetScanSequence();
                return;
            }
            entity.SelectedOrderWF = new ProdOrderPartslistWFInfo();
            Item.State = ActionState.Selection;

            bool success = await ExecuteInvokeBarcodeCommand();
            if (success && Item != null && (Item.Message == null || Item.Message.MessageLevel < eMsgLevel.Warning))
            {
                if (ScannedMachine != null && Item.CurrentBarcode != ScannedMachine.Barcode)
                    Item.CurrentBarcode = ScannedMachine.Barcode;

                OnPropertyChanged(nameof(ScannedMachine));

                success = await ExecuteInvokeBarcodeCommand();
            }
        }


        public override void Clear()
        {
            base.Clear();
            BarcodeSequence = new List<object>();
            CurrentBarcode = null;
            _TempSequenceList = null;
        }

        public async override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            if (DialogOptions.RequestID == 10)
            {
                if (result == Global.MsgResult.OK && entredValue == "1")
                {
                    await InvokeActionOnMachine();
                    OnPropertyChanged(nameof(ReleaseMachine));
                }

                return;
            }
            else if (DialogOptions.RequestID == 20)
            {
                if (result == Global.MsgResult.Yes)
                {
                    ShowDialog(new core.datamodel.Msg(core.datamodel.eMsgLevel.QuestionPrompt, Strings.AppStrings.ReleaseMachine_Question), "", Keyboard.Numeric, "", 10);
                }
                return;
            }
            else if (DialogOptions.RequestID == 1000)
            {
                FilterSequenceList(entredValue);
                return;
            }

            base.DialogResponse(result, entredValue);
        }

        public BarcodeEntity ScannedMachine
        {
            get
            {
                if (this.Item == null || !this.Item.Sequence.Any())
                    return null;
                return this.Item.Sequence.Where(c => c.ACClass != null).FirstOrDefault();
            }
        }

        private List<object> _TempSequenceList;

        public void SearchSequenceList()
        {
            if (BarcodeSequence == null || !BarcodeSequence.Any())
                return;

            ShowDialog(new Msg(eMsgLevel.QuestionPrompt, Strings.AppStrings.FilterProdOrder_Text), Strings.AppStrings.Filter_Text, null, "", 1000);
        }

        public void FilterSequenceList(string searchWord)
        {
            if (string.IsNullOrEmpty(searchWord))
            {
                if (_TempSequenceList != null)
                {
                    BarcodeSequence = _TempSequenceList;
                }
                return;
            }

            Guid temp;
            if (Guid.TryParse(searchWord, out temp))
                return;

            if (_TempSequenceList == null)
                _TempSequenceList = BarcodeSequence;

            List<object> result = new List<object>();

            object machine = _TempSequenceList.FirstOrDefault(c => c is ACClass);
            if (machine == null)
                return;

            result.Add(machine);

            result.AddRange(_TempSequenceList.OfType<ProdOrderPartslistWFInfo>().Where(c => c.ProdOrderPartslist.Partslist.Material.MaterialName1.ToLower().Contains(searchWord)
                                                                                         || c.ProdOrderPartslist.ProdOrder.ProgramNo.ToLower().Contains(searchWord)));

            BarcodeSequence = result;
        }

        public Command InvokeVerifyOrderCommand { get; set; }
        public async Task<bool> ExecuteInvokeVerifyOrderCommand()
        {
            bool success = true;

            if (IsBusy || Item == null)
                return false;

            IsBusy = true;
            Item.BarcodeIssuer = BarcodeIssuer.HasValue ? BarcodeIssuer.Value : BarcodeIssuerEnum.Production;

            try
            {
                ProdOrderPartslistWFInfo wfInfo = SelectedSequence as ProdOrderPartslistWFInfo;
                BarcodeEntity entity = Item.Sequence.LastOrDefault();
                if (entity == null)
                {
                    ResetScanSequence();
                    return false;
                }
                entity.SelectedOrderWF = wfInfo;

                var response = await _WebService.VerifyOrderPostingsOnReleaseAsync(entity);

                if (response.Suceeded)
                {
                    Msg msg = response.Data;
                    if (msg != null && msg.MessageLevel == eMsgLevel.Question)
                    {
                        ShowDialog(msg, "", null, "", 20);
                    }
                    else
                    {
                        ShowDialog(new core.datamodel.Msg(core.datamodel.eMsgLevel.QuestionPrompt, Strings.AppStrings.ReleaseMachine_Question), "", Keyboard.Numeric, "", 10);
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

            return success;
        }

        public void InitUserTime(ProdOrderPartslistWFInfo wfInfo)
        {
            if (wfInfo.UserTime != null)
            {
                wfInfo.UserTime.UserStartDate = wfInfo.UserTime.StartDate;

                //if (!wfInfo.UserTime.UserEndDate.HasValue)
                //    wfInfo.UserTime.UserEndDate = DateTime.Now;
            }
        }
    }
}
