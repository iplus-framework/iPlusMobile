using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using gip.mes.webservices;
using gip.core.autocomponent;
using gip.core.datamodel;

namespace gip.vbm.mobile.Services
{
    public partial class VBWebService
    {
        #region Material

        public async Task<WSResponse<List<Material>>> GetMaterialsAsync()
        {
            return await Get<List<Material>>(VBWebServiceConst.UriMaterial);
        }

        public async Task<WSResponse<Material>> GetMaterialAsync(string materialID)
        {
            if (string.IsNullOrEmpty(materialID))
                return await Task.FromResult(new WSResponse<Material>(null, new Msg(eMsgLevel.Error, "materialID is empty")));
            return await Get<Material>(String.Format(VBWebServiceConst.UriMaterialID_F, materialID));
        }

        public async Task<WSResponse<List<Material>>> GetSuggestedMaterialsAsync(string materialID)
        {
            if (string.IsNullOrEmpty(materialID))
                return await Task.FromResult(new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Error, "materialID is empty")));
            return await Get<List<Material>>(String.Format(VBWebServiceConst.UriSuggestedMaterialsID_F, materialID));
        }

        public async Task<WSResponse<List<Material>>> SearchMaterialAsync(string term)
        {
            if (string.IsNullOrEmpty(term))
                return await Task.FromResult(new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Error, "term is empty")));
            return await Get<List<Material>>(String.Format(VBWebServiceConst.UriMaterial_Search_F, term));
        }

        public async Task<WSResponse<Material>> GetMaterialByBarcodeAsync(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return await Task.FromResult(new WSResponse<Material>(null, new Msg(eMsgLevel.Error, "barcodeID is empty")));
            return await Get<Material>(String.Format(VBWebServiceConst.UriMaterial_BarcodeID_F, barcodeID));
        }

        public async Task<WSResponse<List<MDUnit>>> GetMaterialUnitsAsync(string materialID)
        {
            if (string.IsNullOrEmpty(materialID))
                return await Task.FromResult(new WSResponse<List<MDUnit>>(null, new Msg(eMsgLevel.Error, "materialID is empty")));
            return await Get<List<MDUnit>>(String.Format(VBWebServiceConst.UriMaterialUnitsID_F, materialID));
        }

        public async Task<WSResponse<MDUnitCalc>> MaterialConvertUnitAsync(MDUnitCalc calcParam)
        {
            if (calcParam == null)
                return await Task.FromResult(new WSResponse<MDUnitCalc>(null, new Msg(eMsgLevel.Error, "calcParam is null")));
            return await Post<MDUnitCalc, MDUnitCalc>(calcParam, VBWebServiceConst.UriMaterialConvertUnit);
        }

        public async Task<WSResponse<List<MDUnitCalc>>> MaterialConvertAllUnitsAsync(MDUnitCalc calcParam)
        {
            if (calcParam == null)
                return await Task.FromResult(new WSResponse<List<MDUnitCalc>>(null, new Msg(eMsgLevel.Error, "calcParam is null")));
            return await Post<List<MDUnitCalc>, MDUnitCalc>(calcParam, VBWebServiceConst.UriMaterialConvertAllUnits);
        }

        #endregion


        #region Facility

        public async Task<WSResponse<List<Facility>>> GetFacilitiesAsync()
        {
            return await Get<List<Facility>>(VBWebServiceConst.UriFacility);
        }

        public async Task<WSResponse<Facility>> GetFacilityAsync(string facilityID)
        {
            if (string.IsNullOrEmpty(facilityID))
                return await Task.FromResult(new WSResponse<Facility>(null, new Msg(eMsgLevel.Error, "facilityID is empty")));
            return await Get<Facility>(String.Format(VBWebServiceConst.UriFacilityID_F, facilityID));
        }

        public async Task<WSResponse<List<Facility>>> SearchFacilityAsync(string term, string parentFacilityID, string type)
        {
            if (string.IsNullOrEmpty(term) && string.IsNullOrEmpty(parentFacilityID) && string.IsNullOrEmpty(type))
                return await Task.FromResult(new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "parameters are empty")));
            return await Get<List<Facility>>(String.Format(VBWebServiceConst.UriFacility_Search_F, term, parentFacilityID, type));
        }

        public async Task<WSResponse<Facility>> GetFacilityByBarcodeAsync(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return await Task.FromResult(new WSResponse<Facility>(null, new Msg(eMsgLevel.Error, "barcodeID is empty")));
            return await Get<Facility>(String.Format(VBWebServiceConst.UriFacility_BarcodeID_F, barcodeID));
        }

        #endregion
    }
}