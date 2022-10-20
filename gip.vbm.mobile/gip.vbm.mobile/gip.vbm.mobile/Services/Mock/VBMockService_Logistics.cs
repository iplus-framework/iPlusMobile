using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using gip.mes.webservices;
using gip.core.autocomponent;
using gip.core.datamodel;
using System.Linq;

namespace gip.vbm.mobile.Services
{
    public partial class VBMockService
    {
        public Task<WSResponse<List<MDPickingType>>> GetPickingTypesAsync()
        {
            return null;
        }

        private WSResponse<List<Picking>> _GetPickings;
        public async Task<WSResponse<List<Picking>>> GetPickingsAsync()
        {
            if (_GetPickings == null)
                _GetPickings = JsonConvert.DeserializeObject<WSResponse<List<Picking>>>(ReadJsonFile("Models.JsonMock.Pickings.json"));
            return await Task.FromResult(_GetPickings);
        }

        public async Task<WSResponse<Picking>> GetPickingAsync(string pickingID)
        {
            var pickings = await GetPickingsAsync();
            Guid guid = Guid.Parse(pickingID);
            Picking picking = pickings.Data.Where(c => c.PickingID == guid).FirstOrDefault();
            if (picking == null)
                picking = pickings.Data.FirstOrDefault();
            return await Task.FromResult(new WSResponse<Picking>(picking));
        }

        public Task<WSResponse<List<Picking>>> SearchPickingsAsync(string pType, string fromFacility, string toFacility, string fromDate, string toDate)
        {
            return null;
        }

        public async Task<WSResponse<bool>> InsertPickingAsync(Picking item)
        {
            if (item == null || item.PickingID == null)
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "pickingID is null")));
            var pickings = await GetPickingsAsync();
            pickings.Data.Add(item);
            return await Task.FromResult(new WSResponse<bool>(true));
        }

        public async Task<WSResponse<bool>> UpdatePickingAsync(Picking item)
        {
            if (item == null || item.PickingID == null)
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "item is null")));
            return await Task.FromResult(new WSResponse<bool>(true));
        }

        public async Task<WSResponse<bool>> DeletePickingAsync(string pickingID)
        {
            if (string.IsNullOrEmpty(pickingID))
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "pickingID is empty")));
            var pickings = await GetPickingsAsync();
            Guid guid = Guid.Parse(pickingID);
            Picking picking = pickings.Data.Where(c => c.PickingID == guid).FirstOrDefault();
            if (picking == null)
                return await Task.FromResult(new WSResponse<bool>(false));
            bool succ = pickings.Data.Remove(picking);
            return await Task.FromResult(new WSResponse<bool>(succ));
        }

        public async Task<WSResponse<PickingPos>> GetPickingPosAsync(string pickingPosID)
        {
            var result = await GetPickingsAsync();
            Guid guid = Guid.Parse(pickingPosID);
            var resPickingPos = result.Data.Where(c => c.PickingPos_Picking.Where(d => d.PickingPosID == guid).Any())
                                .Select(c => c.PickingPos_Picking.Where(d => d.PickingPosID == guid).FirstOrDefault())
                                .FirstOrDefault();
            return await Task.FromResult(new WSResponse<PickingPos>(resPickingPos));
        }

        public async Task<WSResponse<PostingOverview>> GetPickingPosPostingsAsync(string pickingPosID)
        {
            return await Task.FromResult(new WSResponse<PostingOverview>(new PostingOverview()));
        }

        public async Task<WSResponse<PickingPosList>> GetPickingPosByMaterialAsync(PickingPosList pickingPos)
        {
            return await Task.FromResult(new WSResponse<PickingPosList>());
        }
    }
}