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
using static gip.mes.datamodel.BarcodeSequenceBase;

namespace gip.vbm.mobile.ViewModels
{
    public class BarcodeScanManuModel : BarcodeScanModelBase
    {
        public BarcodeScanManuModel() : base()
        {
        }

        public override BarcodeSequence ExchangedBarcodeSeq
        {
            get
            {
                return _ExchangedBarcodeSeq;
            }
            set
            {
                SetProperty(ref _ExchangedBarcodeSeq, value);
                if (_ExchangedBarcodeSeq != null
                    && ExchangedBarcodeSeq.Sequence != null
                    && ExchangedBarcodeSeq.Sequence.Any())
                {
                    object prevBarcodeSeq = DecodedEntitiesList;
                    object prevSelectedSequence = SelectedEntity;
                    object newSelectedSequence = null;

                    List<object> barcodeSequence = ExchangedBarcodeSeq.Sequence.Where(x => x.MsgResult == null && x.ValidEntity != null)
                                                                .Select(c => c.ValidEntity)
                                                                .ToList();
                    if (   ExchangedBarcodeSeq.State == mes.datamodel.BarcodeSequenceBase.ActionState.Selection
                        || ExchangedBarcodeSeq.State == mes.datamodel.BarcodeSequenceBase.ActionState.Completed)
                    {
                        List<BarcodeEntity> barcodeEntitiesWithOrderInfos =  ExchangedBarcodeSeq.Sequence.Where(c => c.OrderWFInfos != null && c.OrderWFInfos.Any()).ToList();
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

                    DecodedEntitiesList = barcodeSequence;
                    SelectedEntity = newSelectedSequence;
                    _TempSequenceList = null;
                }
                else
                {
                    SelectedEntity = null;
                    DecodedEntitiesList = new List<object>();
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
            ProdOrderPartslistWFInfo wfInfo = SelectedEntity as ProdOrderPartslistWFInfo;
            wfInfo.InfoState = POPartslistInfoState.Pause;
            await InvokeActionOnMachine();
        }

        public async Task InvokeActionOnMachine()
        {
            ProdOrderPartslistWFInfo wfInfo = SelectedEntity as ProdOrderPartslistWFInfo;
            BarcodeEntity entity = ExchangedBarcodeSeq.Sequence.LastOrDefault();
            if (entity == null)
            {
                ResetScanSequence();
                return;
            }
            entity.SelectedOrderWF = wfInfo;

            bool success = await ExecuteInvokeBarcodeSequenceCommand();
            if (success && ExchangedBarcodeSeq != null && (ExchangedBarcodeSeq.Message == null || ExchangedBarcodeSeq.Message.MessageLevel < eMsgLevel.Warning))
            {
                if (ScannedMachine != null && ExchangedBarcodeSeq.CurrentBarcode != ScannedMachine.Barcode)
                    ExchangedBarcodeSeq.CurrentBarcode = ScannedMachine.Barcode;

                success = await ExecuteInvokeBarcodeSequenceCommand();
            }
        }

        public override void Clear()
        {
            base.Clear();
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


            base.DialogResponse(result, entredValue);
        }

        public BarcodeEntity ScannedMachine
        {
            get
            {
                if (this.ExchangedBarcodeSeq == null || !this.ExchangedBarcodeSeq.Sequence.Any())
                    return null;
                return this.ExchangedBarcodeSeq.Sequence.Where(c => c.ACClass != null).FirstOrDefault();
            }
        }

        private List<object> _TempSequenceList;

        public void FilterSequenceList(string searchWord)
        {
            if (string.IsNullOrEmpty(searchWord))
            {
                if (_TempSequenceList != null)
                {
                    DecodedEntitiesList = _TempSequenceList;
                }
                return;
            }

            Guid temp;
            if (Guid.TryParse(searchWord, out temp))
                return;

            if (_TempSequenceList == null)
                _TempSequenceList = DecodedEntitiesList;

            List<object> result = new List<object>();

            object machine = _TempSequenceList.FirstOrDefault(c => c is ACClass);
            if (machine == null)
                return;

            result.Add(machine);

            result.AddRange(_TempSequenceList.OfType<ProdOrderPartslistWFInfo>().Where(c => c.ProdOrderPartslist.Partslist.Material.MaterialName1.ToLower().Contains(searchWord)));

            DecodedEntitiesList = result;
        }

        public Command InvokeVerifyOrderCommand { get; set; }
        public async Task<bool> ExecuteInvokeVerifyOrderCommand()
        {
            bool success = true;

            if (IsBusy || ExchangedBarcodeSeq == null)
                return false;

            IsBusy = true;
            ExchangedBarcodeSeq.BarcodeIssuer = BarcodeIssuer.HasValue ? BarcodeIssuer.Value : BarcodeIssuerEnum.Production;

            try
            {
                ProdOrderPartslistWFInfo wfInfo = SelectedEntity as ProdOrderPartslistWFInfo;
                BarcodeEntity entity = ExchangedBarcodeSeq.Sequence.LastOrDefault();
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
    }
}
