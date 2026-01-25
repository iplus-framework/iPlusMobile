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
using gip.mes.facility;
using gip.core.datamodel;

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

        public async Task<WSResponse<List<FacilityCharge>>> GetRegisteredFacilityChargesAsync(string workplaceID)
        {
            if (string.IsNullOrEmpty(workplaceID))
                return await Task.FromResult(new WSResponse<List<FacilityCharge>> (null, new Msg(eMsgLevel.Error, "workplaceID is empty")));
            return await Get<List<FacilityCharge>>(string.Format(VBWebServiceConst.UriRegisteredFacilityChargeID_F, workplaceID));
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

        public async Task<WSResponse<FacilityCharge>> GetFacilityChargeFromFacilityMaterialLotAsync(string facilityID, string materialID, string facilityLotID, string splitNo)
        {
            if (string.IsNullOrEmpty(facilityID) || string.IsNullOrEmpty(materialID) || string.IsNullOrEmpty(facilityLotID) || string.IsNullOrEmpty(splitNo))
                return await Task.FromResult(new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "parameters are empty")));

            return await Get<FacilityCharge>(String.Format(VBWebServiceConst.UriFacilityChargeFacilityMaterialLot_F, facilityID, materialID, facilityLotID, splitNo));
        }

        public async Task<WSResponse<FacilityCharge>> CreateFacilityChargeAsync(FacilityChargeParamItem facilityCharge)
        {
            if (facilityCharge == null)
                return await Task.FromResult(new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "parameter facility charge is null")));

            return await Post<FacilityCharge, FacilityChargeParamItem>(facilityCharge, VBWebServiceConst.UriFacilityChargeNew);
        }

        public async Task<WSResponse<bool>> ActivateFacilityChargeAsync(FacilityChargeParamItem activationItem)
        {
            if (activationItem == null)
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "parameter activationItem is null")));

            return await Post<bool, FacilityChargeParamItem>(activationItem, VBWebServiceConst.UriActivateFacilityCharge);
        }

        public async Task<WSResponse<bool>> DeactivateFacilityChargeAsync(FacilityChargeParamItem deactivationItem)
        {
            if (deactivationItem == null)
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "parameter deactivationItem is null")));

            return await Post<bool, FacilityChargeParamItem>(deactivationItem, VBWebServiceConst.UriDeactivateFacilityCharge);
        }

        public async Task<WSResponse<List<FacilityCharge>>> GetOperationLogFacilityChargesAsync(string machineID)
        {
            if (string.IsNullOrEmpty(machineID))
                return await Task.FromResult(new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Error, "machineID is empty")));

            return await Get<List<FacilityCharge>>(string.Format(VBWebServiceConst.UriOperationFacilityChargeID_F, machineID));
        }

        public async Task<WSResponse<BarcodeEntity>> GetLastPostingOrderAsync(string facilityChargeID)
        {
            if (string.IsNullOrEmpty(facilityChargeID))
                return await Task.FromResult(new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Error, "facilityChargeID is empty")));

            return await Get<BarcodeEntity>(string.Format(VBWebServiceConst.UriGetLastPostingOrder_F, facilityChargeID));
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

        public async Task<WSResponse<List<FacilityLot>>> SearchFacilityLotByMaterialAsync(string materialNo)
        {
            if (string.IsNullOrEmpty(materialNo))
                return await Task.FromResult(new WSResponse<List<FacilityLot>>(null, new Msg(eMsgLevel.Error, "materialNo is empty")));
            return await Get<List<FacilityLot>>(String.Format(VBWebServiceConst.UriFacilityLot_SearchByMaterial_F, materialNo));
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

        public async Task<WSResponse<MsgWithDetails>> BookFacilitiesAsync(ACMethodBookingList bpParams)
        {
            if (bpParams == null)
                return await Task.FromResult(new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "bpParams is null")));
            return await Post<MsgWithDetails, ACMethodBookingList>(bpParams, VBWebServiceConst.UriBookFacilities);
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

        #region Inventory -> Lifecycle
        #endregion

        #region Inventory -> Pos
        #region Inventory -> Pos - Get
        public async Task<WSResponse<List<FacilityInventoryPos>>> GetFacilityInventoryLinesAsync(string facilityInventoryNo, string inputCode, string storageLocationNo, string facilityNo, string lotNo, string materialNo, string inventoryPosState, string notAvailable, string zeroStock, string notProcessed)
        {
            inputCode = inputCode.CorrectEmptyUrlString();
            storageLocationNo = storageLocationNo.CorrectEmptyUrlString();
            facilityNo = facilityNo.CorrectEmptyUrlString();
            lotNo = lotNo.CorrectEmptyUrlString();
            materialNo = materialNo.CorrectEmptyUrlString();
            inventoryPosState = inventoryPosState.CorrectEmptyUrlString();
            notAvailable = notAvailable.CorrectEmptyUrlString();
            zeroStock = zeroStock.CorrectEmptyUrlString();
            notProcessed = notProcessed.CorrectEmptyUrlString();

            return await Get<List<FacilityInventoryPos>>(String.Format(VBWebServiceConst.UrlInventory_InventoryLines_F, facilityInventoryNo, inputCode, storageLocationNo, facilityNo, lotNo, materialNo, inventoryPosState, notAvailable, zeroStock, notProcessed));
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

        // WSResponse<SearchFacilityCharge> GetFacilityInventorySearchCharge(string facilityInventoryNo,string storageLocationNo, string facilityNo, string facilityChargeID)
        // public const string UrlInventory_SearchCharge = "FacilityInventoryPos/FacilityInventoryNo/{facilityInventoryNo}/StorageLocationNo/{storageLocationNo}/FacilityNo/{facilityNo}/FacilityChargeID/{facilityChargeID}";
        public async Task<WSResponse<SearchFacilityCharge>> GetFacilityInventorySearchCharge(string facilityInventoryNo, string storageLocationNo, string facilityNo, string facilityChargeID)
        {
            if (
                string.IsNullOrEmpty(facilityInventoryNo) 
                || string.IsNullOrEmpty(storageLocationNo) 
                || string.IsNullOrEmpty(facilityChargeID))
                return await Task.FromResult(new WSResponse<SearchFacilityCharge>(null, new Msg(eMsgLevel.Error, "parameters are empty")));
            facilityNo = facilityNo.CorrectEmptyUrlString();
            return await Get<SearchFacilityCharge>(String.Format(VBWebServiceConst.UrlInventory_SearchCharge_F, facilityInventoryNo, storageLocationNo, facilityNo, facilityChargeID));
        }

        // WSResponse<FacilityInventoryPos> SetFacilityInventoryChargeAvailable (string facilityInventoryNo, string facilityChargeID)
        // public const string UrlInventory_SetChargeAvailable = "FacilityInventoryChargeAvailable/FacilityInventoryNo/{facilityInventoryNo}/FacilityChargeID/{facilityChargeID}";
        public async Task<WSResponse<FacilityInventoryPos>> SetFacilityInventoryChargeAvailable(string facilityInventoryNo, string facilityChargeID)
        {
            if (
                string.IsNullOrEmpty(facilityInventoryNo) 
                || string.IsNullOrEmpty(facilityChargeID))
                return await Task.FromResult(new WSResponse<FacilityInventoryPos>(null, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<FacilityInventoryPos>(String.Format(VBWebServiceConst.UrlInventory_SetChargeAvailable_F, facilityInventoryNo, facilityChargeID));
        }

        #endregion


        #endregion

        #endregion

        #region MovementReason

        public async Task<WSResponse<List<MDMovementReason>>> GetMovementReasonsAsync()
        {
            return await Get<List<MDMovementReason>>(VBWebServiceConst.UriGetMovementReasons);
        }

        #endregion

        #region OEEReason

        public async Task<WSResponse<List<core.webservices.ACClassMessage>>> GetOEEReasonsAsync(string acClassID)
        {
            if (string.IsNullOrEmpty(acClassID))
                return await Task.FromResult(new WSResponse<List<core.webservices.ACClassMessage>>(null, new Msg(eMsgLevel.Error, "acClassID is empty")));
            return await Get<List<core.webservices.ACClassMessage>>(String.Format(VBWebServiceConst.UriOEEReasons_F, acClassID));
        }

        #endregion
    }
}