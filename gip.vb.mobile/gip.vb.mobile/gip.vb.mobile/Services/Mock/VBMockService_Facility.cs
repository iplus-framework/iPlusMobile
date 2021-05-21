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

namespace gip.vb.mobile.Services
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

        #region Inventory -> New
        // WSResponse<string> GetFacilityInventoryNo ()
        //public const string UrlInventory_GetInventoryNo = "FacilityInventory/Get/FacilityInventoryNo";
        public async Task<WSResponse<string>> GetFacilityInventoryNoAsync()
        {
            WSResponse<string> response = new WSResponse<string>() { Data = "0001" };
            return await Task.FromResult(response);
        }

        // WSResponse NewFacilityInventory (string facilityInventoryNo, string facilityInventoryName)
        // public const string UrlInventory_New = "FacilityInventory/New/FacilityInventoryNo/{facilityInventoryNo}/FacilityInventoryName/{facilityInventoryName}";
        public async Task<WSResponse<bool>> NewFacilityInventoryAsync(string facilityInventoryNo, string facilityInventoryName)
        {
            WSResponse<bool> response = new WSResponse<bool>() { Data = true };
            return await Task.FromResult(response);
        }

        #endregion

        #region Inventory -> Lifecycle
        // WSResponse<bool> StartFacilityInventory (string facilityInventoryNo)
        //public const string UrlInventory_Start = "FacilityInventory/Start/FacilityInventoryNo/{facilityInventoryNo}";
        public async Task<WSResponse<bool>> StartFacilityInventoryAsync(string facilityInventoryNo)
        {
            WSResponse<bool> response = new WSResponse<bool>() { Data = false };
            if (_GetFacilityInventoriesAsync != null && _GetFacilityInventoriesAsync.Data != null)
            {
                FacilityInventory facilityInventory = _GetFacilityInventoriesAsync.Data.FirstOrDefault(c => c.FacilityInventoryNo == facilityInventoryNo);
                if (facilityInventory != null)
                {
                    MDFacilityInventoryState inProgressState = MDFacilityInventoryStates.Data.FirstOrDefault(c => c.FacilityInventoryState == MDFacilityInventoryState.FacilityInventoryStates.InProgress);
                    facilityInventory.MDFacilityInventoryState = inProgressState;
                    response.Data = true;
                }
            }
            return await Task.FromResult(response);
        }

        // WSResponse<bool> CloseFacilityInventory (string facilityInventoryNo)
        //public const string UrlInventory_Close = "FacilityInventory/Close/FacilityInventoryNo/{facilityInventoryNo}";
        public async Task<WSResponse<bool>> CloseFacilityInventoryAsync(string facilityInventoryNo)
        {
            WSResponse<bool> response = new WSResponse<bool>() { Data = false };
            if (_GetFacilityInventoriesAsync != null && _GetFacilityInventoriesAsync.Data != null)
            {
                FacilityInventory facilityInventory = _GetFacilityInventoriesAsync.Data.FirstOrDefault(c => c.FacilityInventoryNo == facilityInventoryNo);
                if (facilityInventory != null)
                {
                    MDFacilityInventoryState finishedState = MDFacilityInventoryStates.Data.FirstOrDefault(c => c.FacilityInventoryState == MDFacilityInventoryState.FacilityInventoryStates.Finished);
                    facilityInventory.MDFacilityInventoryState = finishedState;
                    response.Data = true;
                }
            }
            return await Task.FromResult(response);
        }
        #endregion

        #region Inventory -> Pos
        #region Inventory -> Pos - Get
        private WSResponse<List<FacilityInventoryPos>> _GetFacilityInventoryPosesAsync;
        // WSResponse<List<FacilityInventoryPos>> GetFacilityInventoryPoses (string facilityInventoryNo, Guid? inputCode, string facilityNo, string lotNo, string materialNo, short? inventoryPosState, bool? notAvailable, bool?zeroStock)
        //public const string UrlInventory_InventoryPoses = "FacilityInventory/FacilityInventoryNo/{facilityInventoryNo}/InputCode/{inputCode}/FacilityNo/{facilityNo}/LotNo/{lotNo}" +
        //    "/MaterialNo/{materialNo}/InventoryPosState/{inventoryPosState}/NotAvailable/{notAvailable}/ZeroStock/{zeroStock}";
        public async Task<WSResponse<List<FacilityInventoryPos>>> GetFacilityInventoryPosesAsync(string facilityInventoryNo, string inputCode, string facilityNo, string lotNo, string materialNo, string inventoryPosState, string notAvailable, string zeroStock)
        {
            if (_GetFacilityInventoryPosesAsync == null)
            {
                string jsonContent = ReadJsonFile("Models.JsonMock.FacilityInventoryPoses.json");
                _GetFacilityInventoryPosesAsync = JsonConvert.DeserializeObject<WSResponse<List<FacilityInventoryPos>>>(jsonContent);
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

            _GetFacilityInventoryPosesAsync.Data = _GetFacilityInventoryPosesAsync.Data.Where(c =>
            1 == 1
            && c.FacilityInventoryNo == facilityInventoryNo
            && (inputCodeVal == null || c.FacilityChargeID == inputCodeVal)
            && (facilityNo == null || c.FacilityNo == facilityNo)
            && (lotNo == null || c.LotNo == lotNo)
            && (materialNo == null || c.MaterialNo == materialNo)
            && (inventoryPosStateVal == null || c.MDFacilityInventoryPosStateIndex == inventoryPosStateVal)
            && (notAvailableVal == null || c.NotAvailable == notAvailableVal)
            && (zeroStockVal == null || c.StockQuantity == 0 && (zeroStockVal ?? false))
            ).ToList();
            return await Task.FromResult(_GetFacilityInventoryPosesAsync);
        }

        #endregion

        #region Inventory -> Pos -> Lifecycle
        // WSResponse<bool> UpdateFacilityInventoryPos (FacilityInventoryPos facilityInventoryPos)
        //public const string UrlInventory_InventoryPos_Update = "FacilityInventoryPos/Update";
        public async Task<WSResponse<bool>> UpdateFacilityInventoryPosAsync(FacilityInventoryPos facilityInventoryPos)
        {
            WSResponse<bool> response = new WSResponse<bool>() { Data = false };
            if (_GetFacilityInventoryPosesAsync != null && _GetFacilityInventoryPosesAsync.Data != null)
            {
                FacilityInventoryPos dbFaciltiyInventoryPos = _GetFacilityInventoryPosesAsync.Data.FirstOrDefault(c => c.FacilityInventoryPosID == facilityInventoryPos.FacilityInventoryPosID);
                if (dbFaciltiyInventoryPos != null)
                {

                    dbFaciltiyInventoryPos.NewStockQuantity = facilityInventoryPos.NewStockQuantity;
                    dbFaciltiyInventoryPos.Comment = facilityInventoryPos.Comment;
                    dbFaciltiyInventoryPos.NotAvailable = facilityInventoryPos.NotAvailable;
                    response.Data = true;
                }
            }
            return await Task.FromResult(response);
        }
        // WSResponse<bool> StartFacilityInventoryPos (string facilityInventoryNo, Guid facilityChargeID)
        //public const string UrlInventory_InventoryPos_Start = "FacilityInventoryPos/Start/FacilityInventoryNo/{facilityInventoryNo}/FacilityChargeID/{facilityChargeID}";
        public async Task<WSResponse<bool>> StartFacilityInventoryPosAsync(string facilityInventoryNo, string facilityChargeID)
        {
            WSResponse<bool> response = new WSResponse<bool>() { Data = false };
            if (_GetFacilityInventoryPosesAsync != null && _GetFacilityInventoryPosesAsync.Data != null)
            {
                Guid facilityChargeIDVal = Guid.Parse(facilityChargeID);
                FacilityInventoryPos facilityInventoryPos = _GetFacilityInventoryPosesAsync.Data.FirstOrDefault(c => c.FacilityInventoryNo == facilityInventoryNo && c.FacilityChargeID == facilityChargeIDVal);
                if (facilityInventoryPos != null)
                {
                    MDFacilityInventoryPosState inProgressState = MDFacilityInventoryPosStates.Data.FirstOrDefault(c => c.FacilityInventoryPosState == MDFacilityInventoryPosState.FacilityInventoryPosStates.InProgress);
                    facilityInventoryPos.MDFacilityInventoryPosStateIndex = (short)inProgressState.MDFacilityInventoryPosStateIndex;
                    response.Data = true;
                }
            }
            return await Task.FromResult(response);
        }

        // WSResponse<bool> CloseFacilityInventoryPos (string facilityInventoryNo, Guid facilityChargeID)
        //public const string UrlInventory_InventoryPos_Close = "FacilityInventoryPos/Close/FacilityInventoryNo/{facilityInventoryNo}/FacilityChargeID/{facilityChargeID}";
        public async Task<WSResponse<bool>> CloseFacilityInventoryPosAsync(string facilityInventoryNo, string facilityChargeID)
        {
            WSResponse<bool> response = new WSResponse<bool>() { Data = false };
            if (_GetFacilityInventoryPosesAsync != null && _GetFacilityInventoryPosesAsync.Data != null)
            {
                Guid facilityChargeIDVal = Guid.Parse(facilityChargeID);
                FacilityInventoryPos facilityInventoryPos = _GetFacilityInventoryPosesAsync.Data.FirstOrDefault(c => c.FacilityInventoryNo == facilityInventoryNo && c.FacilityChargeID == facilityChargeIDVal);
                if (facilityInventoryPos != null)
                {
                    MDFacilityInventoryPosState finishedState = MDFacilityInventoryPosStates.Data.FirstOrDefault(c => c.FacilityInventoryPosState == MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished);
                    facilityInventoryPos.MDFacilityInventoryPosStateIndex = (short)finishedState.MDFacilityInventoryPosStateIndex;
                    response.Data = true;
                }
            }
            return await Task.FromResult(response);
        }
        #endregion

        #region Inventory -> Pos -> Booings
        // WSResponse<PostingOverview> GetFacilityInventoryPosBookings(string facilityInventoryNo, Guid facilityChargeID)
        //public const string UrlInventory_InventoryPos_Bookings = "FacilityInventoryPos/FacilityInventoryNo/{facilityInventoryNo}/FacilityChargeID/{facilityChargeID}/Bookings";
        public async Task<WSResponse<PostingOverview>> GetFacilityInventoryPosBookingsAsync(string facilityInventoryNo, string facilityChargeID)
        {
            WSResponse<PostingOverview> response = new WSResponse<PostingOverview>();
            return await Task.FromResult(response);
        }
        #endregion

        #endregion
        #endregion
    }
}