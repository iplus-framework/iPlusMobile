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
