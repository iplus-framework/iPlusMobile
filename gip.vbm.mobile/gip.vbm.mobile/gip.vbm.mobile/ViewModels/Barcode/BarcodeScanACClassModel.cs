// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.webservices;
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
