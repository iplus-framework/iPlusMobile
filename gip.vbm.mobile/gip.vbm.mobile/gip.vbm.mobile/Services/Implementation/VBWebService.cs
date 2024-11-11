// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gip.vbm.mobile.Services
{
    public partial class VBWebService : CoreWebService, IVBWebService
    {
        public VBWebService() : base()
        {
        }

        public VBWebService(Uri baseAddress) : base(baseAddress)
        {
        }


        #region General
        public async Task<WSResponse<BarcodeEntity>> GetBarcodeEntityAsync(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return await Task.FromResult(new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Error, "barcodeID is empty")));

            return await Post<BarcodeEntity, string>(barcodeID, VBWebServiceConst.UriBarcodeEntity);

            //return await Get<BarcodeEntity>(String.Format(VBWebServiceConst.UriBarcodeEntity_BarcodeID_F, barcodeID));
        }

        public async Task<WSResponse<BarcodeSequence>> InvokeBarcodeSequenceAsync(BarcodeSequence sequence)
        {
            if (sequence == null)
                return await Task.FromResult(new WSResponse<BarcodeSequence>(sequence, new Msg(eMsgLevel.Error, "sequence is null")));
            if (String.IsNullOrEmpty(sequence.CurrentBarcode))
                return await Task.FromResult(new WSResponse<BarcodeSequence>(sequence, new Msg(eMsgLevel.Error, "CurrentBarcode is empty")));
            return await Post<BarcodeSequence, BarcodeSequence>(sequence, VBWebServiceConst.UriBarcodeSequence);
        }

        #endregion

        #region Printer & print

        public async Task<WSResponse<bool>> Print(PrintEntity printEntity)
        {
            if (printEntity.Sequence == null || !printEntity.Sequence.Any())
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "sequence is null.")));
            return await Post<bool, PrintEntity>(printEntity, VBWebServiceConst.UriPrint);
        }

        public async Task<WSResponse<string>> AssignPrinterAsync(string printerID)
        {
            if (string.IsNullOrEmpty(printerID))
            {
                return new WSResponse<string>(new Msg(eMsgLevel.Error, "printerID is null."));
            }

            return await Post<string, string>(printerID, VBWebServiceConst.UriAssignPrinter);
        }

        public async Task<WSResponse<string>> GetAssignedPrinterAsync()
        {
            return await Get<string>(VBWebServiceConst.UriGetAssignedPrinter);
        }

        public async Task<WSResponse<string>> GetScannedPrinterAsync(string printerID)
        {
            if (string.IsNullOrEmpty(printerID))
            {
                return new WSResponse<string>(new Msg(eMsgLevel.Error, "printerID is null."));
            }

            return await Get<string>(string.Format(VBWebServiceConst.UriGetScannedPrinter_F, printerID));
        }

        #endregion
    }
}