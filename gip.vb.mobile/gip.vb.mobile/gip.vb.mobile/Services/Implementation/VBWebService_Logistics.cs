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

        public async Task<WSResponse<List<Picking>>> SearchPickingsAsync(string pType, string fromFacility, string toFacility, string fromDate, string toDate)
        {
            pType = pType.CorrectEmptyUrlString();
            fromFacility = fromFacility.CorrectEmptyUrlString();
            toFacility = toFacility.CorrectEmptyUrlString();
            fromDate = fromDate.CorrectEmptyUrlString();
            toDate = toDate.CorrectEmptyUrlString();

            return await Get<List<Picking>>(string.Format(VBWebServiceConst.UriSearchPickingF, pType, fromFacility, toFacility, fromDate, toDate));
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

        public async Task<WSResponse<PickingPosList>> GetPickingPosByMaterialAsync(PickingPosList pickingPos)
        {
            if (pickingPos == null)
            {
                return await Task.FromResult(new WSResponse<PickingPosList>(null, new Msg(eMsgLevel.Error, "pickingPos is empty!")));
            }

            return await Post<PickingPosList, PickingPosList>(pickingPos, VBWebServiceConst.UriPickingPos_Material);
        }

        public async Task<WSResponse<MsgWithDetails>> FinishPickingOrderAsync(Guid pickingID)
        {
            return await Put<MsgWithDetails, Guid>(pickingID, VBWebServiceConst.UriPicking_Finish);
        }

        public async Task<WSResponse<MsgWithDetails>> FinishPickingOrderWithoutCheckAsync(Guid pickingID)
        {
            return await Put<MsgWithDetails, Guid>(pickingID, VBWebServiceConst.UriPicking_FinishWithoutCheck);
        }

        public async Task<WSResponse<MsgWithDetails>> FinishPickingOrdersByMaterialAsync(BarcodeSequence pickingOrders)
        {
            return await Put<MsgWithDetails, BarcodeSequence>(pickingOrders, VBWebServiceConst.UriPicking_FinishByMaterial);
        }

        public async Task<WSResponse<MsgWithDetails>> BookAndFinishPickingOrderAsync(PickingWorkplace pickingWorkplace)
        {
            return await Put<MsgWithDetails, PickingWorkplace>(pickingWorkplace, VBWebServiceConst.UriPicking_FinishAndBook);
        }
    }
}