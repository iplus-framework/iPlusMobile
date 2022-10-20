using System;

namespace gip.vbm.mobile.barcode
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
