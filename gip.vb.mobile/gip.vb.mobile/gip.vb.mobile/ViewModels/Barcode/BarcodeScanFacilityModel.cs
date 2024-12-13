// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace gip.vb.mobile.ViewModels
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

        public override Task<bool> ExecuteInvokeBarcodeCommand()
        {
            string currentBarcode = Item?.CurrentBarcode;
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
                    return base.ExecuteInvokeBarcodeCommand();
                }
            }
            return new Task<bool>(() => false);
        }

        public override BarcodeSequence Item 
        { 
            get => base.Item;
            set
            { 
                base.Item = value;
                Facility facility = value?.LastAddedSequence?.ValidEntity as Facility;
                if (BarcodeScannerHost != null)
                    BarcodeScannerHost.OnFacilityScanned(facility);
            }
        }

    }
}
