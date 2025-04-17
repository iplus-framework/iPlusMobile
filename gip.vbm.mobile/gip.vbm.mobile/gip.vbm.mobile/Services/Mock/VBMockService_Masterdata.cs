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
using System.Linq;
using gip.core.webservices;

namespace gip.vbm.mobile.Services
{
    public partial class VBMockService
    {
        #region Material

        private WSResponse<List<Material>> _GetMaterials;
        public async Task<WSResponse<List<Material>>> GetMaterialsAsync()
        {
            if (_GetMaterials == null)
                _GetMaterials = JsonConvert.DeserializeObject<WSResponse<List<Material>>>(ReadJsonFile("Models.JsonMock.Materials.json"));
            return await Task.FromResult(_GetMaterials);
        }

        public async Task<WSResponse<Material>> GetMaterialAsync(string materialID)
        {
            var materials = await GetMaterialsAsync();
            Guid guid = Guid.Parse(materialID);
            Material material = materials.Data.Where(c => c.MaterialID == guid).FirstOrDefault();
            //if (material == null)
            //    material = materials.Data.FirstOrDefault();
            return await Task.FromResult(new WSResponse<Material>(material));
        }

        public async Task<WSResponse<List<Material>>> SearchMaterialAsync(string term)
        {
            if (string.IsNullOrEmpty(term) || term == CoreWebServiceConst.EmptyParam)
                term = null;
            var materials = await GetMaterialsAsync();
            List <Material> searchResult = materials.Data.Where(c => (term == null || c.MaterialNo.Contains(term) || c.MaterialName1.Contains(term))).ToList();
            return await Task.FromResult(new WSResponse<List<Material>>(searchResult));
        }

        public async Task<WSResponse<Material>> GetMaterialByBarcodeAsync(string barcodeID)
        {
            return await GetMaterialAsync(barcodeID);
        }

        public async Task<WSResponse<List<MDUnit>>> GetMaterialUnitsAsync(string materialID)
        {
            return await Task.FromResult(new WSResponse<List<MDUnit>>(null, new Msg(eMsgLevel.Error, "materialID is empty")));
        }

        public async Task<WSResponse<MDUnitCalc>> MaterialConvertUnitAsync(MDUnitCalc calcParam)
        {
            return await Task.FromResult(new WSResponse<MDUnitCalc>(null, new Msg(eMsgLevel.Error, "calcParam is null")));
        }

        public async Task<WSResponse<List<MDUnitCalc>>> MaterialConvertAllUnitsAsync(MDUnitCalc calcParam)
        {
            return await Task.FromResult(new WSResponse<List<MDUnitCalc>>(null, new Msg(eMsgLevel.Error, "calcParam is null")));
        }

        #endregion


        #region Facility

        private WSResponse<List<Facility>> _GetFacilities;
        public async Task<WSResponse<List<Facility>>> GetFacilitiesAsync()
        {
            if (_GetFacilities == null)
                _GetFacilities = JsonConvert.DeserializeObject<WSResponse<List<Facility>>>(ReadJsonFile("Models.JsonMock.Facilities.json"));
            return await Task.FromResult(_GetFacilities);
        }

        public async Task<WSResponse<Facility>> GetFacilityAsync(string facilityID)
        {
            var facilities = await GetFacilitiesAsync();
            Guid guid = Guid.Parse(facilityID);
            Facility facility = facilities.Data.Where(c => c.FacilityID == guid).FirstOrDefault();
            //if (facility == null)
            //    facility = facilities.Data.FirstOrDefault();
            return await Task.FromResult(new WSResponse<Facility>(facility));
        }

        public async Task<WSResponse<List<Facility>>> SearchFacilityAsync(string term, string parentFacilityID, string type)
        {
            if (string.IsNullOrEmpty(term) && string.IsNullOrEmpty(parentFacilityID) && string.IsNullOrEmpty(type))
                return await Task.FromResult(new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "parameters are empty")));
            var facilities = await GetFacilitiesAsync();

            if (string.IsNullOrEmpty(term) || term == CoreWebServiceConst.EmptyParam)
                term = null;

            Guid? guid = null;
            if (!string.IsNullOrEmpty(parentFacilityID) && parentFacilityID != CoreWebServiceConst.EmptyParam)
            {
                Guid tmpGuid;
                if (!Guid.TryParse(parentFacilityID, out tmpGuid))
                    return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "parentFacilityID is invalid"));
                guid = tmpGuid;
            }

            short? facilityTypeIndex = null;
            if (!string.IsNullOrEmpty(type) && type != CoreWebServiceConst.EmptyParam)
            {
                short tmpIndex = 0;
                if (!short.TryParse(type, out tmpIndex))
                    return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "type is invalid"));
                //Type typeofFacType = typeof(MDFacilityType.FacilityTypes);
                //if (!Enum.IsDefined(typeofFacType, tmpIndex))
                //    return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "type is invalid"));
                facilityTypeIndex = tmpIndex;
            }

            List<Facility> searchResult = facilities.Data
                   .Where(c => (term == null || c.FacilityNo.Contains(term) || c.FacilityName.Contains(term))
                            && (!guid.HasValue || c.ParentFacilityID == guid)
                            && (!facilityTypeIndex.HasValue || c.MDFacilityType.MDFacilityTypeIndex == facilityTypeIndex))
                            .ToList();
            return await Task.FromResult(new WSResponse<List<Facility>>(searchResult));
        }

        public async Task<WSResponse<Facility>> GetFacilityByBarcodeAsync(string barcodeID)
        {
            return await GetFacilityAsync(barcodeID);
        }

        #endregion
    }
}