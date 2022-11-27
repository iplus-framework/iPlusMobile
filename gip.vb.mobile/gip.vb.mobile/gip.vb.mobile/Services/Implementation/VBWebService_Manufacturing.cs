using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using gip.mes.webservices;
using gip.core.autocomponent;
using gip.core.datamodel;

namespace gip.vb.mobile.Services
{
    public partial class VBWebService
    {
        public async Task<WSResponse<List<ProdOrderPartslist>>> GetProdOrderPartslistsAsync()
        {
            return await Get<List<ProdOrderPartslist>>(VBWebServiceConst.UriProdOrderPartslists);
        }

        public async Task<WSResponse<ProdOrderPartslist>> GetProdOrderPartslistAsync(string prodOrderPartslistID)
        {
            return await Get<ProdOrderPartslist>(string.Format(VBWebServiceConst.UriProdOrderPartslistID_F, prodOrderPartslistID));
        }

        public async Task<WSResponse<List<ProdOrderPartslistPos>>> GetProdOrderPLIntermediatesAsync(string prodOrderPartslistID)
        {
            return await Get<List<ProdOrderPartslistPos>>(string.Format(VBWebServiceConst.UriProdOrderPLIntermediates_F, prodOrderPartslistID));
        }

        public async Task<WSResponse<ProdOrderPartslistPos>> GetProdOrderPLIntermediateAsync(string intermediateID)
        {
            return await Get<ProdOrderPartslistPos>(string.Format(VBWebServiceConst.UriProdOrderPLIntermediatesID_F, intermediateID));
        }

        public async Task<WSResponse<List<ProdOrderPartslistPos>>> GetProdOrderIntermBatchesAsync(string parentIntermediateID)
        {
            return await Get<List<ProdOrderPartslistPos>>(string.Format(VBWebServiceConst.UriProdOrderIntermBatches_F, parentIntermediateID));
        }

        public async Task<WSResponse<ProdOrderPartslistPos>> GetProdOrderIntermBatchAsync(string intermediateID)
        {
            return await Get<ProdOrderPartslistPos>(string.Format(VBWebServiceConst.UriProdOrderIntermBatchID_F, intermediateID));
        }

        public async Task<WSResponse<List<ProdOrderPartslistPosRelation>>> GetProdOrderInputMaterialsAsync(string targetPOPLPosID)
        {
            return await Get<List<ProdOrderPartslistPosRelation>>(string.Format(VBWebServiceConst.UriProdOrderInputMaterials_F, targetPOPLPosID));
        }

        public async Task<WSResponse<PostingOverview>> GetProdOrderPosRelFacilityBookingAsync(string POPLPosRelID)
        {
            return await Get<PostingOverview>(string.Format(VBWebServiceConst.UriProdOrderPosRelFacilityBooking_F, POPLPosRelID));
        }

        public async Task<WSResponse<PostingOverview>> GetProdOrderPosFacilityBookingAsync(string POPLPosID)
        {
            return await Get<PostingOverview>(string.Format(VBWebServiceConst.UriProdOrderPosFacilityBooking_F, POPLPosID));
        }

        public async Task<WSResponse<ProdOrderPartslistPos>> GetProdOrderIntermOrIntermBatchByMachineAsync(string machineID)
        {
            return await Get<ProdOrderPartslistPos>(string.Format(VBWebServiceConst.UriProdOrderIntermOrIntermBatchByMachine_F, machineID));
        }

        public async Task<WSResponse<ProdOrderPartslistPos>> GetProdOrderPartslistPosAsync(string POPLPosID)
        {
            return await Get<ProdOrderPartslistPos>(string.Format(VBWebServiceConst.UriProdOrderPLPos_F, POPLPosID));
        }

        public async Task<WSResponse<ProdOrderPartslistPosRelation>> GetProdOrderPartslistPosRelAsync(string POPLPosRelID)
        {
            return await Get<ProdOrderPartslistPosRelation>(string.Format(VBWebServiceConst.UriProdOrderPLPosRel_F, POPLPosRelID));
        }

        public async Task<WSResponse<List<Facility>>> GetPOBatchTargetFacilitiesAsync(string intermBatchID)
        {
            return await Get<List<Facility>>(string.Format(VBWebServiceConst.UriProdOrderBatchTargetFacilities_F, intermBatchID));
        }

        public async Task<WSResponse<Msg>> VerifyOrderPostingsOnReleaseAsync(BarcodeEntity entity)
        {
            if (entity == null)
                return await Task.FromResult(new WSResponse<Msg>(null, new Msg(eMsgLevel.Error, "entity is null")));
            return await Post<Msg, BarcodeEntity>(entity, VBWebServiceConst.UriProdOrderVerOrderPostings);
        }
    }
}