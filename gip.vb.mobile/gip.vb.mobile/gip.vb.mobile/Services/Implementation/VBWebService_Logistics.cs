﻿using System;
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
        public async Task<WSResponse<List<MDPickingType>>> GetPickingTypesAsync()
        {
            return await Get<List<MDPickingType>>(VBWebServiceConst.UriPickingTypes);
        }

        public async Task<WSResponse<List<Picking>>> GetPickingsAsync()
        {
            return await Get<List<Picking>>(VBWebServiceConst.UriPicking);
        }

        public async Task<WSResponse<Picking>> GetPickingAsync(string pickingID)
        {
            if (string.IsNullOrEmpty(pickingID))
                return await Task.FromResult(new WSResponse<Picking>(null, new Msg(eMsgLevel.Error, "pickingID is empty")));
            return await Get<Picking>(String.Format(VBWebServiceConst.UriPickingID_F, pickingID));
        }

        public async Task<WSResponse<bool>> InsertPickingAsync(Picking item)
        {
            if (item == null || item.PickingID == null)
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "pickingID is null")));
            return await Post<bool, Picking>(item, VBWebServiceConst.UriPicking);
        }

        public async Task<WSResponse<bool>> UpdatePickingAsync(Picking item)
        {
            if (item == null || item.PickingID == null)
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "item is null")));
            return await Put<bool, Picking>(item, VBWebServiceConst.UriPicking);
        }

        public async Task<WSResponse<bool>> DeletePickingAsync(string pickingID)
        {
            if (string.IsNullOrEmpty(pickingID))
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "pickingID is empty")));
            return await Delete<bool>(String.Format(VBWebServiceConst.UriPickingID_F, pickingID));
        }

        public async Task<WSResponse<PickingPos>> GetPickingPosAsync(string pickingPosID)
        {
            if (string.IsNullOrEmpty(pickingPosID))
                return await Task.FromResult(new WSResponse<PickingPos>(null, new Msg(eMsgLevel.Error, "pickingPosID is empty")));
            return await Get<PickingPos>(String.Format(VBWebServiceConst.UriPickingPosID_F, pickingPosID));
        }

        public async Task<WSResponse<PostingOverview>> GetPickingPosPostingsAsync(string pickingPosID)
        {
            if (string.IsNullOrEmpty(pickingPosID))
                return await Task.FromResult(new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "pickingPosID is empty")));
            return await Get<PostingOverview>(String.Format(VBWebServiceConst.UriPickingPos_Postings_F, pickingPosID));
        }

    }
}