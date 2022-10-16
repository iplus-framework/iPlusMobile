using gip.core.webservices;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Text;

namespace gip.vbm.mobile.ViewModels
{
    public class BarcodeScanACClassModel : BarcodeScanModelBase
    {
        public BarcodeScanACClassModel(IBarcodeScanACClassHost barcodeScanHost) : base ()
        {
            BarcodeScanHost = barcodeScanHost;
        }

        public IBarcodeScanACClassHost BarcodeScanHost
        {
            get;
            set;
        }

        public override BarcodeSequence ExchangedBarcodeSeq 
        { 
            get => base.ExchangedBarcodeSeq; 
            set
            {
                base.ExchangedBarcodeSeq = value;
                ACClass acClass = value?.LastAddedSequence?.ValidEntity as ACClass;
                if (BarcodeScanHost != null)
                    BarcodeScanHost.OnACClassScanned(acClass);
            }
        }
    }
}
