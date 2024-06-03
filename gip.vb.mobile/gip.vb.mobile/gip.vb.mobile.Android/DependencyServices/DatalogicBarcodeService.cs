using System;
using Xamarin.Forms;
using System.Threading;
using gip.vb.mobile.ViewModels;
using System.Globalization;
using gip.vb.mobile.barcode;
using Xamarin.Essentials;
using System.Collections.Generic;

#if DATALOGIC
using Com.Datalogic.Decode;
#elif ZEBRA
using Symbol.XamarinEMDK.Barcode;
using static Symbol.XamarinEMDK.Barcode.ScanDataCollection;
#endif

[assembly: Dependency(typeof(gip.vb.mobile.Droid.DatalogicBarcodeService))]

namespace gip.vb.mobile.Droid
{
    public class DatalogicBarcodeService : gip.vb.mobile.barcode.IBarcodeService
    {
        public event BarcodeReadEventHandler Read;

#if DATALOGIC
        internal void OnRead(IDecodeResult p0, object sender)
        {
            if (Read == null)
                return;

            BarcodeReadEventArgs barcodeReadEventArgs =
                new BarcodeReadEventArgs(ConvertBarcodeType(p0.BarcodeID), p0.Text, p0.GetRawData());

            Read.Invoke(sender, barcodeReadEventArgs);
        }

        private BarcodeType ConvertBarcodeType(BarcodeID barcodeID)
        {
            if (barcodeID == BarcodeID.Interleaved25)
                return BarcodeType.Interleaved25;
            else if (barcodeID == BarcodeID.Matrix25)
                return BarcodeType.Matrix25;
            else if (barcodeID == BarcodeID.Maxicode)
                return BarcodeType.Maxicode;
            else if (barcodeID == BarcodeID.Micropdf417)
                return BarcodeType.Micropdf417;
            else if (barcodeID == BarcodeID.MicroQr)
                return BarcodeType.MicroQr;
            else if (barcodeID == BarcodeID.Msi)
                return BarcodeType.Msi;
            else if (barcodeID == BarcodeID.NotDefined)
                return BarcodeType.NotDefined;
            else if (barcodeID == BarcodeID.Pdf417)
                return BarcodeType.Pdf417;
            else if (barcodeID == BarcodeID.Postal4state)
                return BarcodeType.Postal4state;
            else if (barcodeID == BarcodeID.UpceAddon2)
                return BarcodeType.UpceAddon2;
            else if (barcodeID == BarcodeID.PostalAustralian)
                return BarcodeType.PostalAustralian;
            else if (barcodeID == BarcodeID.PostalKix)
                return BarcodeType.PostalKix;
            else if (barcodeID == BarcodeID.PostalPlanet)
                return BarcodeType.PostalPlanet;
            else if (barcodeID == BarcodeID.PostalPostnet)
                return BarcodeType.PostalPostnet;
            else if (barcodeID == BarcodeID.PostalRoyalmail)
                return BarcodeType.PostalRoyalmail;
            else if (barcodeID == BarcodeID.Qrcode)
                return BarcodeType.Qrcode;
            else if (barcodeID == BarcodeID.Interleaved25)
                return BarcodeType.Interleaved25;
            else if (barcodeID == BarcodeID.Trioptic)
                return BarcodeType.Trioptic;
            else if (barcodeID == BarcodeID.Upca)
                return BarcodeType.Upca;
            else if (barcodeID == BarcodeID.UpcaAddon2)
                return BarcodeType.UpcaAddon2;
            else if (barcodeID == BarcodeID.PostalJapan)
                return BarcodeType.PostalJapan;
            else if (barcodeID == BarcodeID.Gs1Limit)
                return BarcodeType.Gs1Limit;
            else if (barcodeID == BarcodeID.Gs1Exp)
                return BarcodeType.Gs1Exp;
            else if (barcodeID == BarcodeID.Gs114)
                return BarcodeType.Gs114;
            else if (barcodeID == BarcodeID.UpceAddon5)
                return BarcodeType.UpceAddon5;
            else if (barcodeID == BarcodeID.Aztec)
                return BarcodeType.Aztec;
            else if (barcodeID == BarcodeID.Codabar)
                return BarcodeType.Codabar;
            else if (barcodeID == BarcodeID.Code128)
                return BarcodeType.Code128;
            else if (barcodeID == BarcodeID.Code32)
                return BarcodeType.Code32;
            else if (barcodeID == BarcodeID.Code39)
                return BarcodeType.Code39;
            else if (barcodeID == BarcodeID.Code39Fullascii)
                return BarcodeType.Code39Fullascii;
            else if (barcodeID == BarcodeID.Code93)
                return BarcodeType.Code93;
            else if (barcodeID == BarcodeID.Datamatrix)
                return BarcodeType.Datamatrix;
            else if (barcodeID == BarcodeID.Discrete25)
                return BarcodeType.Discrete25;
            else if (barcodeID == BarcodeID.Ean13)
                return BarcodeType.Ean13;
            else if (barcodeID == BarcodeID.Ean13Addon2)
                return BarcodeType.Ean13Addon2;
            else if (barcodeID == BarcodeID.Ean13Addon5)
                return BarcodeType.Ean13Addon5;
            else if (barcodeID == BarcodeID.Ean13Isbn)
                return BarcodeType.Ean13Isbn;
            else if (barcodeID == BarcodeID.Ean13Issn)
                return BarcodeType.Ean13Issn;
            else if (barcodeID == BarcodeID.Ean8)
                return BarcodeType.Ean8;
            else if (barcodeID == BarcodeID.Ean8Addon2)
                return BarcodeType.Ean8Addon2;
            else if (barcodeID == BarcodeID.Ean8Addon5)
                return BarcodeType.Ean8Addon5;
            else if (barcodeID == BarcodeID.Gs1128)
                return BarcodeType.Gs1128;
            else if (barcodeID == BarcodeID.UpcaAddon5)
                return BarcodeType.UpcaAddon5;
            else if (barcodeID == BarcodeID.Upce)
                return BarcodeType.Upce;
            return BarcodeType.NotDefined;
        }

#elif ZEBRA
        internal void Scanner_Data(object sender, Scanner.DataEventArgs e)
        {
            if (Read == null)
                return;
            ScanDataCollection scanDataCollection = e.P0;

            if ((scanDataCollection != null) && (scanDataCollection.Result == ScannerResults.Success))
            {
                IList<ScanDataCollection.ScanData> scanData = scanDataCollection.GetScanData();
                foreach (ScanDataCollection.ScanData data in scanData)
                {
                    BarcodeReadEventArgs barcodeReadEventArgs =
                        new BarcodeReadEventArgs(ConvertBarcodeType(data.LabelType), data.Data, data.GetRawData());
                    Read.Invoke(sender, barcodeReadEventArgs);
                }
            }
        }

        internal void Scanner_Status(object sender, Scanner.StatusEventArgs e)
        {
        }

        private BarcodeType ConvertBarcodeType(LabelType barcodeID)
        {
            if (barcodeID == LabelType.I2of5)
                return BarcodeType.Interleaved25;
            else if (barcodeID == LabelType.Matrix2of5)
                return BarcodeType.Matrix25;
            else if (barcodeID == LabelType.Maxicode)
                return BarcodeType.Maxicode;
            else if (barcodeID == LabelType.Microqr)
                return BarcodeType.MicroQr;
            else if (barcodeID == LabelType.Msi)
                return BarcodeType.Msi;
            else if (barcodeID == LabelType.Pdf417)
                return BarcodeType.Pdf417;
            else if (barcodeID == LabelType.Us4state)
                return BarcodeType.Postal4state;
            //else if (barcodeID == BarcodeID.UpceAddon2)
            //    return BarcodeType.UpceAddon2;
            //else if (barcodeID == BarcodeID.PostalAustralian)
            //    return BarcodeType.PostalAustralian;
            //else if (barcodeID == BarcodeID.PostalKix)
            //    return BarcodeType.PostalKix;
            //else if (barcodeID == BarcodeID.PostalPlanet)
            //    return BarcodeType.PostalPlanet;
            //else if (barcodeID == BarcodeID.PostalPostnet)
            //    return BarcodeType.PostalPostnet;
            //else if (barcodeID == BarcodeID.PostalRoyalmail)
            //    return BarcodeType.PostalRoyalmail;
            else if (barcodeID == LabelType.Qrcode)
                return BarcodeType.Qrcode;
            //else if (barcodeID == LabelType.Interleaved25)
            //    return BarcodeType.Interleaved25;
            //else if (barcodeID == LabelType.Trioptic)
            //    return BarcodeType.Trioptic;
            else if (barcodeID == LabelType.Upca)
                return BarcodeType.Upca;
            //else if (barcodeID == LabelType.UpcaAddon2)
            //    return BarcodeType.UpcaAddon2;
            else if (barcodeID == LabelType.Jappostal)
                return BarcodeType.PostalJapan;
            else if (barcodeID == LabelType.Gs1DatabarLim)
                return BarcodeType.Gs1Limit;
            else if (barcodeID == LabelType.Gs1DatabarExp)
                return BarcodeType.Gs1Exp;
            //else if (barcodeID == LabelType.Gs114)
            //    return BarcodeType.Gs114;
            else if (barcodeID == LabelType.Upce1)
                return BarcodeType.UpceAddon5;
            else if (barcodeID == LabelType.Aztec)
                return BarcodeType.Aztec;
            else if (barcodeID == LabelType.Codabar)
                return BarcodeType.Codabar;
            else if (barcodeID == LabelType.Code128)
                return BarcodeType.Code128;
            else if (barcodeID == LabelType.Code32)
                return BarcodeType.Code32;
            else if (barcodeID == LabelType.Code39)
                return BarcodeType.Code39;
            //else if (barcodeID == LabelType.Code39Fullascii)
            //    return BarcodeType.Code39Fullascii;
            else if (barcodeID == LabelType.Code93)
                return BarcodeType.Code93;
            else if (barcodeID == LabelType.Datamatrix)
                return BarcodeType.Datamatrix;
            //else if (barcodeID == LabelType.Discrete25)
            //    return BarcodeType.Discrete25;
            else if (barcodeID == LabelType.Ean13)
                return BarcodeType.Ean13;
            //else if (barcodeID == LabelType.Ean13Addon2)
            //    return BarcodeType.Ean13Addon2;
            //else if (barcodeID == LabelType.Ean13Addon5)
            //    return BarcodeType.Ean13Addon5;
            //else if (barcodeID == LabelType.Ean13Isbn)
            //    return BarcodeType.Ean13Isbn;
            //else if (barcodeID == LabelType.Ean13Issn)
            //    return BarcodeType.Ean13Issn;
            else if (barcodeID == LabelType.Ean8)
                return BarcodeType.Ean8;
            //else if (barcodeID == LabelType.Ean8Addon2)
            //    return BarcodeType.Ean8Addon2;
            //else if (barcodeID == LabelType.Ean8Addon5)
            //    return BarcodeType.Ean8Addon5;
            else if (barcodeID == LabelType.Gs1Databar)
                return BarcodeType.Gs1128;
            //else if (barcodeID == LabelType.UpcaAddon5)
            //    return BarcodeType.UpcaAddon5;
            else if (barcodeID == LabelType.Upce0)
                return BarcodeType.Upce;
            return BarcodeType.NotDefined;
        }

#endif
    }
}