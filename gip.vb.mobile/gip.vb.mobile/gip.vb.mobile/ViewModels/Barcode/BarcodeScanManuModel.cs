using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gip.vb.mobile.ViewModels
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
                    List<object> barcodeSequence = Item.Sequence.Where(x => x.MsgResult == null && x.ValidEntity != null)
                                                                .Select(c => c.ValidEntity)
                                                                .ToList();
                    if (Item.State == mes.datamodel.BarcodeSequenceBase.ActionState.Selection)
                    {
                        ProdOrderPartslistWFInfo[] latestOrderInfos = Item.Sequence.Where(c => c.OrderWFInfos != null && c.OrderWFInfos.Any())
                                                        .Select(c => c.OrderWFInfos)
                                                        .LastOrDefault();
                        if (latestOrderInfos != null && latestOrderInfos.Any())
                        {
                            foreach (ProdOrderPartslistWFInfo wfInfo in latestOrderInfos)
                            {
                                if (wfInfo.WFMethod != null)
                                    wfInfo.WFMethod.UseCultureInfoForConversion = true;
                            }
                            barcodeSequence.AddRange(latestOrderInfos);
                        }
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

        public override void Clear()
        {
            base.Clear();
            BarcodeSequence = new List<object>();
            CurrentBarcode = null;
        }
    }
}
