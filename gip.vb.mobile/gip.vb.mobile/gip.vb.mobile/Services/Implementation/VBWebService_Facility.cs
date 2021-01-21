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
using gip.vb.mobile.Services.Implementation;

namespace gip.vb.mobile.Services
{
    public partial class VBWebService
    {
        #region FacilityCharge

        public async Task<WSResponse<List<FacilityCharge>>> GetFacilityChargesAsync()
        {
            return await Get<List<FacilityCharge>>(VBWebServiceConst.UriFacilityCharge);
        }

        public async Task<WSResponse<FacilityCharge>> GetFacilityChargeAsync(string facilityChargeID)
        {
            if (string.IsNullOrEmpty(facilityChargeID))
                return await Task.FromResult(new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "facilityChargeID is empty")));
            return await Get<FacilityCharge>(String.Format(VBWebServiceConst.UriFacilityChargeID_F, facilityChargeID));
        }

        public async Task<WSResponse<FacilityCharge>> GetFacilityChargeByBarcodeAsync(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return await Task.FromResult(new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "barcodeID is empty")));
            return await Get<FacilityCharge>(String.Format(VBWebServiceConst.UriFacilityCharge_BarcodeID_F, barcodeID));
        }

        public async Task<WSResponse<PostingOverview>> GetFacilityChargeBookingsAsync(string facilityChargeID, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(facilityChargeID) || string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo))
                return await Task.FromResult(new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<PostingOverview>(String.Format(VBWebServiceConst.UriFacilityCharge_Bookings_F, facilityChargeID, dateFrom, dateTo));
        }

        #endregion


        #region FacilityLot

        public async Task<WSResponse<List<FacilityLot>>> GetFacilityLotsAsync()
        {
            return await Get<List<FacilityLot>>(VBWebServiceConst.UriFacilityLot);
        }

        public async Task<WSResponse<FacilityLot>> GetFacilityLotAsync(string facilityLotID)
        {
            if (string.IsNullOrEmpty(facilityLotID))
                return await Task.FromResult(new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "facilityLotID is empty")));
            return await Get<FacilityLot>(String.Format(VBWebServiceConst.UriFacilityLotID_F, facilityLotID));
        }

        public async Task<WSResponse<List<FacilityLot>>> SearchFacilityLotAsync(string term)
        {
            if (string.IsNullOrEmpty(term))
                return await Task.FromResult(new WSResponse<List<FacilityLot>>(null, new Msg(eMsgLevel.Error, "term is empty")));
            return await Get<List<FacilityLot>>(String.Format(VBWebServiceConst.UriFacilityLot_Search_F, term));
        }

        public async Task<WSResponse<FacilityLot>> GetFacilityLotByBarcodeAsync(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return await Task.FromResult(new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "barcodeID is empty")));
            return await Get<FacilityLot>(String.Format(VBWebServiceConst.UriFacilityLot_BarcodeID_F, barcodeID));
        }

        public async Task<WSResponse<FacilityLotSumOverview>> GetFacilityLotSumAsync(string facilityLotID)
        {
            if (string.IsNullOrEmpty(facilityLotID))
                return await Task.FromResult(new WSResponse<FacilityLotSumOverview>(null, new Msg(eMsgLevel.Error, "facilityLotID is empty")));
            return await Get<FacilityLotSumOverview>(String.Format(VBWebServiceConst.UriFacilityLot_SumID_F, facilityLotID));
        }

        public async Task<WSResponse<PostingOverview>> GetFacilityLotBookingsAsync(string facilityLotID, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(facilityLotID) || string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo))
                return await Task.FromResult(new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<PostingOverview>(String.Format(VBWebServiceConst.UriFacilityLot_Bookings_F, facilityLotID, dateFrom, dateTo));
        }

        #endregion


        #region Material

        public async Task<WSResponse<MaterialSumOverview>> GetMaterialSumAsync(string materialID)
        {
            if (string.IsNullOrEmpty(materialID))
                return await Task.FromResult(new WSResponse<MaterialSumOverview>(null, new Msg(eMsgLevel.Error, "materialID is empty")));
            return await Get<MaterialSumOverview>(String.Format(VBWebServiceConst.UriMaterial_SumID_F, materialID));
        }

        public async Task<WSResponse<PostingOverview>> GetMaterialBookingsAsync(string materialID, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(materialID) || string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo))
                return await Task.FromResult(new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<PostingOverview>(String.Format(VBWebServiceConst.UriMaterial_Bookings_F, materialID, dateFrom, dateTo));
        }

        #endregion


        #region Facility

        public async Task<WSResponse<FacilitySumOverview>> GetFacilitySumAsync(string facilityID)
        {
            if (string.IsNullOrEmpty(facilityID))
                return await Task.FromResult(new WSResponse<FacilitySumOverview>(null, new Msg(eMsgLevel.Error, "facilityID is empty")));
            return await Get<FacilitySumOverview>(String.Format(VBWebServiceConst.UriFacility_SumID_F, facilityID));
        }

        public async Task<WSResponse<PostingOverview>> GetFacilityBookingsAsync(string facilityID, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(facilityID) || string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo))
                return await Task.FromResult(new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<PostingOverview>(String.Format(VBWebServiceConst.UriFacility_Bookings_F, facilityID, dateFrom, dateTo));
        }

        #endregion


        #region FacilityLocation

        public async Task<WSResponse<FacilityLocationSumOverview>> GetFacilityLocationSumAsync(string facilityID)
        {
            if (string.IsNullOrEmpty(facilityID))
                return await Task.FromResult(new WSResponse<FacilityLocationSumOverview>(null, new Msg(eMsgLevel.Error, "facilityID is empty")));
            return await Get<FacilityLocationSumOverview>(String.Format(VBWebServiceConst.UriFacilityLocation_SumID_F, facilityID));
        }


        public async Task<WSResponse<PostingOverview>> GetFacilityLocationBookingsAsync(string facilityID, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(facilityID) || string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo))
                return await Task.FromResult(new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<PostingOverview>(String.Format(VBWebServiceConst.UriFacilityLocation_Bookings_F, facilityID, dateFrom, dateTo));
        }

        #endregion

        #region Booking
        public async Task<WSResponse<MsgWithDetails>> BookFacilityAsync(ACMethodBooking bpParam)
        {
            if (bpParam == null)
                return await Task.FromResult(new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "bpParam is null")));
            return await Post<MsgWithDetails, ACMethodBooking>(bpParam, VBWebServiceConst.UriBookFacility);
        }
        #endregion

        #region Inventory

        #region Inventory-> MD
        //public const string UrlInventory_MDFacilityInventoryState = "FacilityInventory/Get/MDFacilityInventoryState";
        public async Task<WSResponse<List<MDFacilityInventoryState>>> GetMDFacilityInventoryStatesAsync()
        {
            return await Get<List<MDFacilityInventoryState>>(VBWebServiceConst.UrlInventory_MDFacilityInventoryState);
        }

        //public const string UrlInventory_MDFacilityInventoryPosState = "FacilityInventory/Get/MDFacilityInventoryPosState";
        public async Task<WSResponse<List<MDFacilityInventoryPosState>>> GetMDFacilityInventoryPosStatesAsync()
        {
            return await Get<List<MDFacilityInventoryPosState>>(VBWebServiceConst.UrlInventory_MDFacilityInventoryPosState);
        }
        #endregion

        #region Inventory -> Get

        // WSResponse<List<FacilityInventory>> GetFacilityInventories (short? inventoryState, DateTime dateFrom, DateTime dateTo)
        //public const string UrlInventory_Inventories = "FacilityInventory/FacilityInventories/InventoryState/{inventoryState}/from/{dateFrom}/to/{dateTo}";
        public async Task<WSResponse<List<FacilityInventory>>> GetFacilityInventoriesAsync(string inventoryState, string dateFrom, string dateTo)
        {
            inventoryState = inventoryState.CorrectEmptyUrlString();
            return await Get<List<FacilityInventory>>(String.Format(VBWebServiceConst.UrlInventory_Inventories_F, inventoryState, dateFrom, dateTo));
        }

        #endregion

        #region Inventory -> New
        // WSResponse<string> GetFacilityInventoryNo ()
        //public const string UrlInventory_GetInventoryNo = "FacilityInventory/Get/FacilityInventoryNo";
        public async Task<WSResponse<string>> GetFacilityInventoryNoAsync()
        {
            return await Get<string>(VBWebServiceConst.UrlInventory_GetInventoryNo_F);
        }

        // WSResponse NewFacilityInventory (string facilityInventoryNo, string facilityInventoryName)
        // public const string UrlInventory_New = "FacilityInventory/New/FacilityInventoryNo/{facilityInventoryNo}/FacilityInventoryName/{facilityInventoryName}";
        public async Task<WSResponse<bool>> NewFacilityInventoryAsync(string facilityInventoryNo, string facilityInventoryName)
        {
            if (string.IsNullOrEmpty(facilityInventoryNo) || string.IsNullOrEmpty(facilityInventoryName))
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<bool>(String.Format(VBWebServiceConst.UrlInventory_New_F, facilityInventoryNo, facilityInventoryName));
        }

        #endregion

        #region Inventory -> Lifecycle
        // WSResponse<bool> StartFacilityInventory (string facilityInventoryNo)
        //public const string UrlInventory_Start = "FacilityInventory/Start/FacilityInventoryNo/{facilityInventoryNo}";
        public async Task<WSResponse<bool>> StartFacilityInventoryAsync(string facilityInventoryNo)
        {
            if (string.IsNullOrEmpty(facilityInventoryNo))
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<bool>(String.Format(VBWebServiceConst.UrlInventory_Start_F, facilityInventoryNo));
        }

        // WSResponse<bool> CloseFacilityInventory (string facilityInventoryNo)
        //public const string UrlInventory_Close = "FacilityInventory/Close/FacilityInventoryNo/{facilityInventoryNo}";
        public async Task<WSResponse<bool>> CloseFacilityInventoryAsync(string facilityInventoryNo)
        {
            if (string.IsNullOrEmpty(facilityInventoryNo))
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<bool>(String.Format(VBWebServiceConst.UrlInventory_Close_F, facilityInventoryNo));
        }
        #endregion

        #region Inventory -> Pos
        #region Inventory -> Pos - Get
        // WSResponse<List<FacilityInventoryPos>> GetFacilityInventoryPoses (string facilityInventoryNo, Guid? inputCode, string facilityNo, string lotNo, string materialNo, short? inventoryPosState, bool? notAvailable, bool?zeroStock)
        //public const string UrlInventory_InventoryPoses = "FacilityInventory/FacilityInventoryNo/{facilityInventoryNo}/InputCode/{inputCode}/FacilityNo/{facilityNo}/LotNo/{lotNo}" +
        //    "/MaterialNo/{materialNo}/InventoryPosState/{inventoryPosState}/NotAvailable/{notAvailable}/ZeroStock/{zeroStock}";
        public async Task<WSResponse<List<FacilityInventoryPos>>> GetFacilityInventoryPosesAsync(string facilityInventoryNo, string inputCode, string facilityNo, string lotNo, string materialNo, string inventoryPosState, string notAvailable, string zeroStock)
        {
            inputCode = inputCode.CorrectEmptyUrlString();
            facilityNo = facilityNo.CorrectEmptyUrlString();
            lotNo = lotNo.CorrectEmptyUrlString();
            materialNo = materialNo.CorrectEmptyUrlString();
            inventoryPosState = inventoryPosState.CorrectEmptyUrlString();
            notAvailable = notAvailable.CorrectEmptyUrlString();
            zeroStock = zeroStock.CorrectEmptyUrlString();

            return await Get<List<FacilityInventoryPos>>(String.Format(VBWebServiceConst.UrlInventory_InventoryPoses_F, facilityInventoryNo, inputCode, facilityNo, lotNo, materialNo, inventoryPosState, notAvailable, zeroStock));
        }

        #endregion

        #region Inventory -> Pos -> Lifecycle
        // WSResponse<bool> UpdateFacilityInventoryPos (FacilityInventoryPos facilityInventoryPos)
        //public const string UrlInventory_InventoryPos_Update = "FacilityInventoryPos/Update";
        public async Task<WSResponse<bool>> UpdateFacilityInventoryPosAsync(FacilityInventoryPos facilityInventoryPos)
        {
            if (facilityInventoryPos == null)
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Post<bool, FacilityInventoryPos>(facilityInventoryPos, VBWebServiceConst.UrlInventory_InventoryPos_Update);
        }
        // WSResponse<bool> StartFacilityInventoryPos (string facilityInventoryNo, Guid facilityChargeID)
        //public const string UrlInventory_InventoryPos_Start = "FacilityInventoryPos/Start/FacilityInventoryNo/{facilityInventoryNo}/FacilityChargeID/{facilityChargeID}";
        public async Task<WSResponse<bool>> StartFacilityInventoryPosAsync(string facilityInventoryNo, string facilityChargeID)
        {
            if (string.IsNullOrEmpty(facilityInventoryNo) || string.IsNullOrEmpty(facilityChargeID))
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<bool>(String.Format(VBWebServiceConst.UrlInventory_InventoryPos_Start_F, facilityInventoryNo, facilityChargeID));
        }

        // WSResponse<bool> CloseFacilityInventoryPos (string facilityInventoryNo, Guid facilityChargeID)
        //public const string UrlInventory_InventoryPos_Close = "FacilityInventoryPos/Close/FacilityInventoryNo/{facilityInventoryNo}/FacilityChargeID/{facilityChargeID}";
        public async Task<WSResponse<bool>> CloseFacilityInventoryPosAsync(string facilityInventoryNo, string facilityChargeID)
        {
            if (string.IsNullOrEmpty(facilityInventoryNo) || string.IsNullOrEmpty(facilityChargeID))
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<bool>(String.Format(VBWebServiceConst.UrlInventory_InventoryPos_Close_F, facilityInventoryNo, facilityChargeID));
        }
        #endregion

        #region Inventory -> Pos -> Booings
        // WSResponse<PostingOverview> GetFacilityInventoryPosBookings(string facilityInventoryNo, Guid facilityChargeID)
        //public const string UrlInventory_InventoryPos_Bookings = "FacilityInventoryPos/FacilityInventoryNo/{facilityInventoryNo}/FacilityChargeID/{facilityChargeID}/Bookings";
        public async Task<WSResponse<PostingOverview>> GetFacilityInventoryPosBookingsAsync(string facilityInventoryNo, string facilityChargeID)
        {
            if (string.IsNullOrEmpty(facilityInventoryNo) || string.IsNullOrEmpty(facilityChargeID))
                return await Task.FromResult(new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<PostingOverview>(String.Format(VBWebServiceConst.UrlInventory_InventoryPos_Bookings_F, facilityInventoryNo, facilityChargeID));
        }
        #endregion

        #endregion

        #endregion


    }
}