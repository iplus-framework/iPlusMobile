// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;

namespace gip.vb.mobile.barcode
{
    public class BarcodeReadEventArgs : EventArgs
    {
        public BarcodeReadEventArgs(BarcodeType bcType, string text)
        {
            _BCType = bcType;
            _Text = text;
        }

        public BarcodeReadEventArgs(BarcodeType bcType, string text, byte[] rawData) 
            : this(bcType, text)
        {
            _RawData = rawData;
        }


        private BarcodeType _BCType;
        public BarcodeType BCType
        {
            get
            {
                return _BCType;
            }
        }

        private string _Text;
        public string Text
        {
            get
            {
                return _Text;
            }
        }

        private byte[] _RawData;
        public byte[] RawData
        {
            get
            {
                return _RawData;
            }
        }
    }
}
