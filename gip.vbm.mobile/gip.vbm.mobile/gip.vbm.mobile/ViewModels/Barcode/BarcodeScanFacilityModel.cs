// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace gip.vbm.mobile.ViewModels
{
    public class BarcodeScanFacilityModel : BarcodeScanModelBase
    {
        public BarcodeScanFacilityModel(IBarcodeScanFacilityHost barcodeScannerHost) : base ()
        {
            BarcodeScannerHost = barcodeScannerHost;
        }

        IBarcodeScanFacilityHost BarcodeScannerHost
        {
            get;
            set;
        }

        public override Task<bool> ExecuteInvokeBarcodeSequenceCommand()
        {
            string currentBarcode = ExchangedBarcodeSeq?.CurrentBarcode;
            Guid temp;
            if (!string.IsNullOrEmpty(currentBarcode))
            {
                if (!Guid.TryParse(currentBarcode, out temp))
                {
                    BarcodeScannerHost?.OnSearchFacility(currentBarcode);
                    return new Task<bool>(() => true);
                }
                else
                {
                    return base.ExecuteInvokeBarcodeSequenceCommand();
                }
            }
            return new Task<bool>(() => false);
        }

        public override BarcodeSequence ExchangedBarcodeSeq 
        { 
            get => base.ExchangedBarcodeSeq;
            set
            { 
                base.ExchangedBarcodeSeq = value;
                Facility facility = value?.LastAddedSequence?.ValidEntity as Facility;
                if (BarcodeScannerHost != null)
                    BarcodeScannerHost.OnFacilityScanned(facility);
            }
        }

    }
}
