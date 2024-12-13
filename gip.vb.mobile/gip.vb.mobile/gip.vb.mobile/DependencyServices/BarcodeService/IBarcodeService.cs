// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Globalization;

namespace gip.vb.mobile.barcode
{
    public delegate void BarcodeReadEventHandler(object sender, BarcodeReadEventArgs e);

    public interface IBarcodeService
    {
        event BarcodeReadEventHandler Read;
    }
}
