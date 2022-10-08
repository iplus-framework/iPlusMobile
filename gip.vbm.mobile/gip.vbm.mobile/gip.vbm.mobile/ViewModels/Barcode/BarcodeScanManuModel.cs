using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.ViewModels
{
    public class BarcodeScanManuModel : BarcodeScanModelBase
    {
        public BarcodeScanManuModel() : base()
        {
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
            ShowDialog(new core.datamodel.Msg(core.datamodel.eMsgLevel.QuestionPrompt, Strings.AppStrings.ReleaseMachine_Question), "",  Keyboard.Numeric, "", 10);
        }

        public async Task PauseOrderOnMachine()
        {
            ProdOrderPartslistWFInfo wfInfo = SelectedSequence as ProdOrderPartslistWFInfo;
            wfInfo.InfoState = POPartslistInfoState.Pause;
            await InvokeActionOnMachine();
        }

        public async Task InvokeActionOnMachine()
        {
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

            result.AddRange(_TempSequenceList.OfType<ProdOrderPartslistWFInfo>().Where(c => c.ProdOrderPartslist.Partslist.Material.MaterialName1.ToLower().Contains(searchWord)));

            BarcodeSequence = result;
        }
    }
}
