// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using gip.mes.webservices;
using gip.core.autocomponent;
using System.Linq;

namespace gip.vb.mobile.Services
{
    public partial class VBMockService
    {
        private WSResponse<List<ProdOrderPartslist>> _GetProdOrderPartslist;
        public async Task<WSResponse<List<ProdOrderPartslist>>> GetProdOrderPartslistsAsync()
        {
            if (_GetProdOrderPartslist == null)
                _GetProdOrderPartslist = JsonConvert.DeserializeObject<WSResponse<List<ProdOrderPartslist>>>(ReadJsonFile("Models.JsonMock.ProdOrderPartslists.json"));
            return await Task.FromResult(_GetProdOrderPartslist);
        }

        public async Task<WSResponse<ProdOrderPartslist>> GetProdOrderPartslistAsync(string prodOrderPartslistID)
        {
            var partsLists = await GetProdOrderPartslistsAsync();
            Guid guid = Guid.Parse(prodOrderPartslistID);
            ProdOrderPartslist pList = partsLists.Data.Where(c => c.ProdOrderPartslistID == guid).FirstOrDefault();
            return await Task.FromResult(new WSResponse<ProdOrderPartslist>(pList));
        }

        private WSResponse<List<ProdOrderPartslistPos>> _GetProdOrderPLIntermediates;
        public async Task<WSResponse<List<ProdOrderPartslistPos>>> GetProdOrderPLIntermediatesAsync(string prodOrderPartslistID)
        {
            if (_GetProdOrderPLIntermediates == null)
                _GetProdOrderPLIntermediates = JsonConvert.DeserializeObject<WSResponse<List<ProdOrderPartslistPos>>>(ReadJsonFile("Models.JsonMock.ProdOrderPLIntermediates.json"));
            return await Task.FromResult(_GetProdOrderPLIntermediates);
        }

        public async Task<WSResponse<ProdOrderPartslistPos>> GetProdOrderPLIntermediateAsync(string intermediateID)
        {
            var partsLists = await GetProdOrderPLIntermediatesAsync("");
            Guid guid = Guid.Parse(intermediateID);
            ProdOrderPartslistPos pList = partsLists.Data.Where(c => c.ProdOrderPartslistPosID == guid).FirstOrDefault();
            return await Task.FromResult(new WSResponse<ProdOrderPartslistPos>(pList));
        }

        public async Task<WSResponse<List<ProdOrderPartslistPos>>> GetProdOrderIntermBatchesAsync(string parentIntermediateID)
        {
            var obj = JsonConvert.DeserializeObject<WSResponse<List<ProdOrderPartslistPos>>>(ReadJsonFile(string.Format("Models.JsonMock.POPLIm-{0}.json", parentIntermediateID)));
            return await Task.FromResult(obj);
        }

        public async Task<WSResponse<ProdOrderPartslistPos>> GetProdOrderIntermBatchAsync(string intermediateID)
        {
            return await GetProdOrderPLIntermediateAsync(intermediateID);
        }

        public async Task<WSResponse<List<ProdOrderPartslistPosRelation>>> GetProdOrderInputMaterialsAsync(string targetPOPLPosID)
        {
            var obj = JsonConvert.DeserializeObject<WSResponse<List<ProdOrderPartslistPosRelation>>>(ReadJsonFile(string.Format("Models.JsonMock.POInM-{0}.json", targetPOPLPosID)));
            return await Task.FromResult(obj);
        }

        public async Task<WSResponse<PostingOverview>> GetProdOrderPosRelFacilityBookingAsync(string POPLPosRelID)
        {
            var obj = JsonConvert.DeserializeObject<WSResponse<PostingOverview>>(ReadJsonFile(string.Format("Models.JsonMock.PORelFB-{0}.json", POPLPosRelID)));
            return await Task.FromResult(obj);
        }

        public async Task<WSResponse<PostingOverview>> GetProdOrderPosFacilityBookingAsync(string POPLPosID)
        {
            string json = "";
            try
            {
                json = ReadJsonFile(string.Format("Models.JsonMock.POPosFB-{0}.json", POPLPosID));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            var obj = JsonConvert.DeserializeObject<WSResponse<PostingOverview>>(json);
            return await Task.FromResult(obj);
        }

        public async Task<WSResponse<ProdOrderPartslistPos>> GetProdOrderIntermOrIntermBatchByMachineAsync(string machineID)
        {
            return await Task.FromResult<WSResponse<ProdOrderPartslistPos>>(null);
        }

        public async Task<WSResponse<ProdOrderPartslistPos>> GetProdOrderPartslistPosAsync(string POPLPosID)
        {
            return await Task.FromResult<WSResponse<ProdOrderPartslistPos>>(null);
        }

        public async Task<WSResponse<ProdOrderPartslistPosRelation>> GetProdOrderPartslistPosRelAsync(string POPLPosRelID)
        {
            return await Task.FromResult<WSResponse<ProdOrderPartslistPosRelation>>(null);
        }

        public async Task<WSResponse<List<Facility>>> GetPOBatchTargetFacilitiesAsync(string intermBatchID)
        {
            return await Task.FromResult<WSResponse<List<Facility>>>(null);
        }

        public async Task<WSResponse<Facility>> GetNFBatchTargetFacilityAsync(string machineFunctionID)
        {
            return await Task.FromResult<WSResponse<Facility>>(null);
        }
    }
}