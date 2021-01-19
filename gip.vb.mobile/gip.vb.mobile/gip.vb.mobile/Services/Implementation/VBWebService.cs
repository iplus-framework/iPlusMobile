using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Threading.Tasks;

namespace gip.vb.mobile.Services
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
            return await Get<BarcodeEntity>(String.Format(VBWebServiceConst.UriBarcodeEntity_BarcodeID_F, barcodeID));
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
    }
}