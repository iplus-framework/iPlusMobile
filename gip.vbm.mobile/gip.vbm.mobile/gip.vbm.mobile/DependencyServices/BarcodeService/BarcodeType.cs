using System;
using System.Globalization;

namespace gip.vbm.mobile.barcode
{
    public enum BarcodeType
    {
        /// <summary>
        /// The interleaved25
        /// </summary>
        Interleaved25,
        /// <summary>
        /// Matrix 2/5.
        /// </summary>
        Matrix25,
        /// <summary>
        /// Maxicode.
        /// </summary>
        Maxicode,
        /// <summary>
        /// MicroPDF417.
        /// </summary>
        Micropdf417,
        /// <summary>
        /// Micro QR Code
        /// </summary>
        MicroQr,
        /// <summary>
        /// MSI.
        /// </summary>
        Msi,
        /// <summary>
        /// The decoded data is not recognized.
        /// </summary>
        NotDefined,
        /// <summary>
        /// PDF-417.
        /// </summary>
        Pdf417,
        /// <summary>
        /// Postal USPS 4-State.
        /// </summary>
        Postal4state,
        /// <summary>
        /// UPC-E + 2.
        /// </summary>
        UpceAddon2,
        /// <summary>
        /// Postal Australian.
        /// </summary>
        PostalAustralian,
        /// <summary>
        /// Postal Kix.
        /// </summary>
        PostalKix,
        /// <summary>
        /// Postal Planet.
        /// </summary>
        PostalPlanet,
        /// <summary>
        /// Postal Postnet.
        /// </summary>
        PostalPostnet,
        /// <summary>
        /// Postal Royal Mail.
        /// </summary>
        PostalRoyalmail,
        /// <summary>
        /// QR Code.
        /// </summary>
        Qrcode,
        /// <summary>
        /// Trioptic.
        /// </summary>
        Trioptic,
        /// <summary>
        /// UPC-A.
        /// </summary>
        Upca,
        /// <summary>
        /// UPC-A + 2
        /// </summary>
        UpcaAddon2,
        /// <summary>
        /// Postal Japan.
        /// </summary>
        PostalJapan,
        /// <summary>
        /// GS1 Databar Limited.
        /// </summary>
        Gs1Limit,
        /// <summary>
        /// GS1 Databar Expanded.
        /// </summary>
        Gs1Exp,
        /// <summary>
        /// GS1 Databar-14.
        /// </summary>
        Gs114,
        /// <summary>
        /// UPC-E + 5.
        /// </summary>
        UpceAddon5,
        /// <summary>
        /// Aztec Code.
        /// </summary>
        Aztec,
        /// <summary>
        /// Codabar.
        /// </summary>
        Codabar,
        /// <summary>
        /// Code 128.
        /// </summary>
        Code128,
        /// <summary>
        /// Code 32.
        /// </summary>
        Code32,
        /// <summary>
        /// Code 39.
        /// </summary>
        Code39,
        /// <summary>
        /// Code 39 Full ASCII
        /// </summary>
        Code39Fullascii,
        /// <summary>
        /// Code 93.
        /// </summary>
        Code93,
        /// <summary>
        /// Datamatrix.
        /// </summary>
        Datamatrix,
        /// <summary>
        /// Discrete 2/5.
        /// </summary>
        Discrete25,
        /// <summary>
        /// EAN-13.
        /// </summary>
        Ean13,
        /// <summary>
        /// EAN-13 + 2.
        /// </summary>
        Ean13Addon2,
        /// <summary>
        /// EAN-13 + 5.
        /// </summary>
        Ean13Addon5,
        /// <summary>
        /// ISBN (ISBN-10 and ISBN-13)
        /// </summary>
        Ean13Isbn,
        /// <summary>
        /// ISSN
        /// </summary>
        Ean13Issn,
        /// <summary>
        /// EAN-8.
        /// </summary>
        Ean8,
        /// <summary>
        /// EAN-8.
        /// </summary>
        Ean8Addon2,
        /// <summary>
        /// EAN-8.
        /// </summary>
        Ean8Addon5,
        /// <summary>
        /// GS1-128.
        /// </summary>
        Gs1128,
        /// <summary>
        /// UPC-A + 5.
        /// </summary>
        UpcaAddon5,
        /// <summary>
        /// UPC-E.
        /// </summary>
        Upce,

    }
}
