using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using gip.mes.webservices;
using gip.core.autocomponent;
using gip.mes.facility;
using gip.core.datamodel;
using System.IO;
using System.Linq;
using gip.core.webservices;

namespace gip.vbm.mobile.Services
{
    public partial class VBMockService
    {
        #region FacilityCharge

        private WSResponse<List<FacilityCharge>> _GetFacilityCharges;
        public async Task<WSResponse<List<FacilityCharge>>> GetFacilityChargesAsync()
        {
            if (_GetFacilityCharges == null)
                _GetFacilityCharges = JsonConvert.DeserializeObject<WSResponse<List<FacilityCharge>>>(ReadJsonFile("Models.JsonMock.FacilityCharges.json"));
            return await Task.FromResult(_GetFacilityCharges);
        }

        public async Task<WSResponse<FacilityCharge>> GetFacilityChargeAsync(string facilityChargeID)
        {
            var facilityCharges = await GetFacilityChargesAsync();
            Guid guid = Guid.Parse(facilityChargeID);
            FacilityCharge fCharge = facilityCharges.Data.Where(c => c.FacilityChargeID == guid).FirstOrDefault();
            //if (fCharge == null)
            //    fCharge = facilityCharges.Data.FirstOrDefault();
            return await Task.FromResult(new WSResponse<FacilityCharge>(fCharge));
        }

        public async Task<WSResponse<FacilityCharge>> GetFacilityChargeByBarcodeAsync(string barcodeID)
        {
            return await GetFacilityChargeAsync(barcodeID);
        }

        private WSResponse<PostingOverview> _GetFacilityChargeBookings;
        public async Task<WSResponse<PostingOverview>> GetFacilityChargeBookingsAsync(string facilityChargeID, string dateFrom, string dateTo)
        {
            if (_GetFacilityChargeBookings == null)
                _GetFacilityChargeBookings = JsonConvert.DeserializeObject<WSResponse<PostingOverview>>(ReadJsonFile("Models.JsonMock.FacilityChargesBookings.json"));
            return await Task.FromResult(_GetFacilityChargeBookings);
        }

        public async Task<WSResponse<List<FacilityCharge>>> GetOperationLogFacilityChargesAsync(string machineID)
        {
            if (string.IsNullOrEmpty(machineID))
                return await Task.FromResult(new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Error, "machineID is empty")));

            return await Task.FromResult<WSResponse<List<FacilityCharge>>>(null);
        }

        public async Task<WSResponse<BarcodeEntity>> GetLastPostingOrderAsync(string facilityChargeID)
        {
            if (string.IsNullOrEmpty(facilityChargeID))
                return await Task.FromResult(new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Error, "facilityChargeID is empty")));

            return await Task.FromResult<WSResponse<BarcodeEntity>>(null);
        }

        #endregion

        #region FacilityLot

        private WSResponse<List<FacilityLot>> _GetFacilityLots;
        public async Task<WSResponse<List<FacilityLot>>> GetFacilityLotsAsync()
        {
            if (_GetFacilityLots == null)
                _GetFacilityLots = JsonConvert.DeserializeObject<WSResponse<List<FacilityLot>>>(ReadJsonFile("Models.JsonMock.FacilityLots.json"));
            return await Task.FromResult(_GetFacilityLots);
        }

        public async Task<WSResponse<FacilityLot>> GetFacilityLotAsync(string facilityLotID)
        {
            var facilityLots = await GetFacilityLotsAsync();
            Guid guid = Guid.Parse(facilityLotID);
            FacilityLot fLot = facilityLots.Data.Where(c => c.FacilityLotID == guid).FirstOrDefault();
            //if (fLot == null)
            //    fLot = facilityLots.Data.FirstOrDefault();
            return await Task.FromResult(new WSResponse<FacilityLot>(fLot));
        }

        public async Task<WSResponse<List<FacilityLot>>> SearchFacilityLotAsync(string term)
        {
            if (string.IsNullOrEmpty(term) || term == CoreWebServiceConst.EmptyParam)
                term = null;
            var facilityLots = await GetFacilityLotsAsync();
            List<FacilityLot> fLots = facilityLots.Data.Where(c => (term == null || c.LotNo == term)).ToList();
            if (!fLots.Any())
                fLots = new List<FacilityLot>() { facilityLots.Data.FirstOrDefault() };
            return await Task.FromResult(new WSResponse<List<FacilityLot>>(fLots));
        }

        public async Task<WSResponse<FacilityLot>> GetFacilityLotByBarcodeAsync(string barcodeID)
        {
            return await GetFacilityLotAsync(barcodeID);
        }

        private WSResponse<FacilityLotSumOverview> _GetFacilityLotSum;
        public async Task<WSResponse<FacilityLotSumOverview>> GetFacilityLotSumAsync(string facilityLotID)
        {
            if (_GetFacilityLotSum == null)
                _GetFacilityLotSum = JsonConvert.DeserializeObject<WSResponse<FacilityLotSumOverview>>(ReadJsonFile("Models.JsonMock.FacilityLotsSum.json"));
            return await Task.FromResult(_GetFacilityLotSum);
        }

        private WSResponse<PostingOverview> _GetFacilityLotBookings;
        public async Task<WSResponse<PostingOverview>> GetFacilityLotBookingsAsync(string facilityLotID, string dateFrom, string dateTo)
        {
            if (_GetFacilityLotBookings == null)
                _GetFacilityLotBookings = JsonConvert.DeserializeObject<WSResponse<PostingOverview>>(ReadJsonFile("Models.JsonMock.FacilityLotsBookings.json"));
            return await Task.FromResult(_GetFacilityLotBookings);
        }

        #endregion

        #region Material

        private WSResponse<MaterialSumOverview> _GetMaterialSum;
        public async Task<WSResponse<MaterialSumOverview>> GetMaterialSumAsync(string materialID)
        {
            if (_GetMaterialSum == null)
                _GetMaterialSum = JsonConvert.DeserializeObject<WSResponse<MaterialSumOverview>>(ReadJsonFile("Models.JsonMock.MaterialsSum.json"));
            return await Task.FromResult(_GetMaterialSum);
        }

        private WSResponse<PostingOverview> _GetMaterialBookings;
        public async Task<WSResponse<PostingOverview>> GetMaterialBookingsAsync(string materialID, string dateFrom, string dateTo)
        {
            if (_GetMaterialBookings == null)
                _GetMaterialBookings = JsonConvert.DeserializeObject<WSResponse<PostingOverview>>(ReadJsonFile("Models.JsonMock.MaterialsBookings.json"));
            return await Task.FromResult(_GetMaterialBookings);
        }

        #endregion

        #region Facility

        private WSResponse<FacilitySumOverview> _GetFacilitySum;
        public async Task<WSResponse<FacilitySumOverview>> GetFacilitySumAsync(string facilityID)
        {
            if (_GetFacilitySum == null)
                _GetFacilitySum = JsonConvert.DeserializeObject<WSResponse<FacilitySumOverview>>(ReadJsonFile("Models.JsonMock.FacilitiesSum.json"));
            return await Task.FromResult(_GetFacilitySum);
        }

        private WSResponse<PostingOverview> _GetFacilityBookings;
        public async Task<WSResponse<PostingOverview>> GetFacilityBookingsAsync(string facilityID, string dateFrom, string dateTo)
        {
            if (_GetFacilityBookings == null)
                _GetFacilityBookings = JsonConvert.DeserializeObject<WSResponse<PostingOverview>>(ReadJsonFile("Models.JsonMock.FacilitiesBookings.json"));
            return await Task.FromResult(_GetFacilityBookings);
        }

        #endregion

        #region FacilityLocation

        private WSResponse<FacilityLocationSumOverview> _GetFacilityLocationSum;
        public async Task<WSResponse<FacilityLocationSumOverview>> GetFacilityLocationSumAsync(string facilityID)
        {
            if (_GetFacilityLocationSum == null)
                _GetFacilityLocationSum = JsonConvert.DeserializeObject<WSResponse<FacilityLocationSumOverview>>(ReadJsonFile("Models.JsonMock.FacilityLocationSum.json"));
            return await Task.FromResult(_GetFacilityLocationSum);
        }


        private WSResponse<PostingOverview> _GetFacilityLocationBookings;
        public async Task<WSResponse<PostingOverview>> GetFacilityLocationBookingsAsync(string facilityID, string dateFrom, string dateTo)
        {
            if (_GetFacilityLocationBookings == null)
                _GetFacilityLocationBookings = JsonConvert.DeserializeObject<WSResponse<PostingOverview>>(ReadJsonFile("Models.JsonMock.FacilityLocationBookings.json"));
            return await Task.FromResult(_GetFacilityLocationBookings);
        }

        #endregion

        #region Booking
        public async Task<WSResponse<MsgWithDetails>> BookFacilityAsync(ACMethodBooking bpParam)
        {
            if (bpParam == null)
                return await Task.FromResult(new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "bpParam is null")));
            return await Task.FromResult(new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "Not implemented")));
        }
        #endregion

        #region Inventory

        #region Inventory-> MD

        private WSResponse<List<MDFacilityInventoryState>> _MDFacilityInventoryStates;
        public WSResponse<List<MDFacilityInventoryState>> MDFacilityInventoryStates
        {
            get
            {
                if (_MDFacilityInventoryStates == null)
                    _MDFacilityInventoryStates = JsonConvert.DeserializeObject<WSResponse<List<MDFacilityInventoryState>>>(ReadJsonFile("Models.JsonMock.MDFacilityInventoryStates.json"));
                return _MDFacilityInventoryStates;
            }
        }

        private WSResponse<List<MDFacilityInventoryPosState>> _MDFacilityInventoryPosStates;
        public WSResponse<List<MDFacilityInventoryPosState>> MDFacilityInventoryPosStates
        {
            get
            {
                if (_MDFacilityInventoryPosStates == null)
                    _MDFacilityInventoryPosStates = JsonConvert.DeserializeObject<WSResponse<List<MDFacilityInventoryPosState>>>(ReadJsonFile("Models.JsonMock.MDFacilityInventoryPosStates.json"));
                return _MDFacilityInventoryPosStates;
            }
        }

        //public const string UrlInventory_MDFacilityInventoryState = "FacilityInventory/Get/MDFacilityInventoryState";
        public async Task<WSResponse<List<MDFacilityInventoryState>>> GetMDFacilityInventoryStatesAsync()
        {
            return await Task.FromResult(MDFacilityInventoryStates);
        }

        //public const string UrlInventory_MDFacilityInventoryPosState = "FacilityInventory/Get/MDFacilityInventoryPosState";
        public async Task<WSResponse<List<MDFacilityInventoryPosState>>> GetMDFacilityInventoryPosStatesAsync()
        {
            return await Task.FromResult(MDFacilityInventoryPosStates);
        }
        #endregion

        #region Inventory -> Get

        private WSResponse<List<FacilityInventory>> _GetFacilityInventoriesAsync;
        // WSResponse<List<FacilityInventory>> GetFacilityInventories (short? inventoryState, DateTime dateFrom, DateTime dateTo)
        //public const string UrlInventory_Inventories = "FacilityInventory/FacilityInventories/InventoryState/{inventoryState}/from/{dateFrom}/to/{dateTo}";
        public async Task<WSResponse<List<FacilityInventory>>> GetFacilityInventoriesAsync(string inventoryState, string dateFrom, string dateTo)
        {
            if (_GetFacilityInventoriesAsync == null)
                _GetFacilityInventoriesAsync = JsonConvert.DeserializeObject<WSResponse<List<FacilityInventory>>>(ReadJsonFile("Models.JsonMock.FacilityInventories.json"));
            return await Task.FromResult(_GetFacilityInventoriesAsync);
        }

        #endregion

        #region Inventory -> Pos
        #region Inventory -> Pos - Get
        private WSResponse<List<FacilityInventoryPos>> _GetFacilityInventoryLinesAsync;
        public async Task<WSResponse<List<FacilityInventoryPos>>> GetFacilityInventoryLinesAsync(string facilityInventoryNo, string inputCode, string storageLocationNo, string facilityNo, string lotNo, string materialNo, string inventoryPosState, string notAvailable, string zeroStock, string notProcessed)
        {
            if (_GetFacilityInventoryLinesAsync == null)
            {
                string jsonContent = ReadJsonFile("Models.JsonMock.FacilityInventoryLines.json");
                _GetFacilityInventoryLinesAsync = JsonConvert.DeserializeObject<WSResponse<List<FacilityInventoryPos>>>(jsonContent);
            }

            // make filtering
            Guid? inputCodeVal = null;
            if (!string.IsNullOrEmpty(inputCode))
                inputCodeVal = Guid.Parse(inputCode);

            short? inventoryPosStateVal = null;
            if (!string.IsNullOrEmpty(inventoryPosState))
                inventoryPosStateVal = short.Parse(inventoryPosState);

            bool? notAvailableVal = null;
            if (!string.IsNullOrEmpty(notAvailable))
                notAvailableVal = bool.Parse(notAvailable);

            bool? zeroStockVal = null;
            if (!string.IsNullOrEmpty(zeroStock))
                zeroStockVal = bool.Parse(zeroStock);

            bool notProcessedVal = false;
            if (!string.IsNullOrEmpty(notProcessed))
                notProcessedVal = bool.Parse(notProcessed);

            List<FacilityInventoryPos> facilityInventoryLines = _GetFacilityInventoryLinesAsync.Data.Where(c =>
            c.FacilityInventoryNo == facilityInventoryNo
            && (inputCodeVal == null || c.FacilityChargeID == inputCodeVal)
            && (storageLocationNo == null || c.ParentFacilityNo == storageLocationNo)
            && (facilityNo == null || c.FacilityNo == facilityNo)
            && (lotNo == null || c.LotNo == lotNo)
            && (materialNo == null || c.MaterialNo == materialNo)
            && (inventoryPosStateVal == null || c.MDFacilityInventoryPosStateIndex == inventoryPosStateVal)
            && (notAvailableVal == null || c.NotAvailable == notAvailableVal)
            && (zeroStockVal == null || c.StockQuantity == 0 && (zeroStockVal ?? false))
            && (!notProcessedVal || (!c.NotAvailable && c.NewStockQuantity == null))
            ).ToList();
            WSResponse<List<FacilityInventoryPos>> wSResponse = new WSResponse<List<FacilityInventoryPos>>(facilityInventoryLines);
            return await Task.FromResult(wSResponse);
        }

        #endregion

        #region Inventory -> Pos -> Lifecycle
        // WSResponse<bool> UpdateFacilityInventoryPos (FacilityInventoryPos facilityInventoryPos)
        //public const string UrlInventory_InventoryPos_Update = "FacilityInventoryPos/Update";
        public async Task<WSResponse<bool>> UpdateFacilityInventoryPosAsync(FacilityInventoryPos facilityInventoryPos)
        {
            WSResponse<bool> response = new WSResponse<bool>() { Data = false };
            if (_GetFacilityInventoryLinesAsync != null && _GetFacilityInventoryLinesAsync.Data != null)
            {
                FacilityInventoryPos dbFaciltiyInventoryPos = _GetFacilityInventoryLinesAsync.Data.FirstOrDefault(c => c.FacilityInventoryPosID == facilityInventoryPos.FacilityInventoryPosID);
                if (dbFaciltiyInventoryPos != null)
                {
                    _GetFacilityInventoryLinesAsync.Data.Remove(dbFaciltiyInventoryPos);
                    dbFaciltiyInventoryPos.NewStockQuantity = facilityInventoryPos.NewStockQuantity;
                    dbFaciltiyInventoryPos.Comment = facilityInventoryPos.Comment;
                    dbFaciltiyInventoryPos.NotAvailable = facilityInventoryPos.NotAvailable;
                    _GetFacilityInventoryLinesAsync.Data.Add(dbFaciltiyInventoryPos);
                    response.Data = true;
                }
            }
            return await Task.FromResult(response);
        }


        // WSResponse<SearchFacilityCharge> GetFacilityInventorySearchCharge(string facilityInventoryNo, string storageLocationNo, string facilityNo, string facilityChargeID)
        // public const string UrlInventory_SearchCharge = "FacilityInventoryPos/FacilityInventoryNo/{facilityInventoryNo}/StorageLocationNo/{storageLocationNo}/FacilityNo/{facilityNo}/FacilityChargeID/{facilityChargeID}";
        public async Task<WSResponse<SearchFacilityCharge>> GetFacilityInventorySearchCharge(string facilityInventoryNo, string storageLocationNo, string facilityNo, string facilityChargeID)
        {
            if (_GetFacilityInventoryLinesAsync == null)
            {
                string jsonContent = ReadJsonFile("Models.JsonMock.FacilityInventoryLines.json");
                _GetFacilityInventoryLinesAsync = JsonConvert.DeserializeObject<WSResponse<List<FacilityInventoryPos>>>(jsonContent);
            }
            SearchFacilityCharge result = new SearchFacilityCharge();

            FacilityInventoryPos item = _GetFacilityInventoryLinesAsync.Data.FirstOrDefault(c => c.FacilityChargeID == new Guid(facilityChargeID));
            if (item == null)
                result.States = new List<FacilityChargeStateEnum> { FacilityChargeStateEnum.NotExist };
            else
            {
                result.FacilityInventoryPos = item;
                result.States = new List<FacilityChargeStateEnum>();
                result.States.Add(FacilityChargeStateEnum.Available);
                if (item.MDFacilityInventoryPosStateIndex == (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished)
                    result.States.Add(FacilityChargeStateEnum.AlreadyFinished);
                if (string.IsNullOrEmpty(facilityNo))
                {
                    if (item.ParentFacilityNo != storageLocationNo)
                        result.States.Add(FacilityChargeStateEnum.InDifferentFacility);
                }
                else
                {
                    if (item.FacilityNo != facilityNo)
                        result.States.Add(FacilityChargeStateEnum.InDifferentFacility);
                }
            }
            WSResponse<SearchFacilityCharge> wSResponse = new WSResponse<SearchFacilityCharge>(result);
            return await Task.FromResult(wSResponse);
        }

        // WSResponse<FacilityInventoryPos> SetFacilityInventoryChargeAvailable (string facilityInventoryNo, string facilityChargeID)
        // public const string UrlInventory_SetChargeAvailable = "FacilityInventoryChargeAvailable/FacilityInventoryNo/{facilityInventoryNo}/FacilityChargeID/{facilityChargeID}";
        public async Task<WSResponse<FacilityInventoryPos>> SetFacilityInventoryChargeAvailable(string facilityInventoryNo, string facilityChargeID)
        {
            FacilityInventoryPos result = new FacilityInventoryPos();
            WSResponse<FacilityInventoryPos> wSResponse = new WSResponse<FacilityInventoryPos>(result);

            result.FacilityInventoryPosID = Guid.NewGuid();
            result.FacilityChargeID = Guid.NewGuid();
            result.Sequence = 0;
            result.Comment = null;
            result.LotNo = "FL01010101";
            result.ParentFacilityNo = "Mixery";
            result.FacilityNo = "Silo10";
            result.FacilityName = "Silo 10";
            result.MaterialNo = "FW01";
            result.MaterialName = "Finished product 01";
            result.FacilityInventoryNo = "00000003";
            result.MDFacilityInventoryPosStateIndex = 2;
            result.StockQuantity = 10;
            result.NewStockQuantity = null;
            result.NotAvailable = false;
            result.UpdateName = "aagincic";
            result.UpdateDate = DateTime.Now;

            if (_GetFacilityInventoryLinesAsync == null)
            {
                string jsonContent = ReadJsonFile("Models.JsonMock.FacilityInventoryLines.json");
                _GetFacilityInventoryLinesAsync = JsonConvert.DeserializeObject<WSResponse<List<FacilityInventoryPos>>>(jsonContent);
            }
            result.Sequence = _GetFacilityInventoryLinesAsync.Data.Count() + 1;
            _GetFacilityInventoryLinesAsync.Data.Add(result);
            return await Task.FromResult(wSResponse);
        }
        #endregion

        #endregion
        #endregion

        #region OEEReason

        public async Task<WSResponse<List<core.webservices.ACClassMessage>>> GetOEEReasonsAsync(string acClassID)
        {
            if (string.IsNullOrEmpty(acClassID))
                return await Task.FromResult(new WSResponse<List<core.webservices.ACClassMessage>>(null, new Msg(eMsgLevel.Error, "acClassID is empty")));
            return await Task.FromResult(new WSResponse<List<core.webservices.ACClassMessage>>(null, null));
        }

        #endregion
    }
}