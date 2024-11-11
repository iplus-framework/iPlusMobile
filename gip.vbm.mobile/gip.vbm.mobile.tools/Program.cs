// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.webservices;
using gip.mes.webservices;
using gip.vbm.mobile.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace gip.vbm.mobile.tools
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Writing files");
            try
            {
                Task.Run(() => DownloadMockJson());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private const string IPlusBackendUrl = "http://192.168.0.13:8730";
        private const string JsonMockFolder = @"C:\Aleksandar\gipSoft\Source\iPlusV4\trunk\Mobile\gip.vbm.mobile\gip.vbm.mobile\gip.vbm.mobile\Models\JsonMock\";
        //private const string JsonMockFolder = @"C:\Devel\gipSoft\iPlusMES\V4\trunk\Mobile\gip.vbm.mobile\gip.vbm.mobile\gip.vbm.mobile\Models\JsonMock\";

        static async Task DownloadMockJson()
        {
            VBWebService vbWebService = new VBWebService(new Uri(IPlusBackendUrl));
            VBUser vbUser = new VBUser("Mobile", "Mobile");
            await vbWebService.Login(vbUser);

            var json = await vbWebService.Client.GetStringAsync(String.Format(CoreWebServiceConst.UriLogin_F, vbUser.Username));
            File.WriteAllText(JsonMockFolder + "VBUserRights.json", json);
            //WSResponse<VBUserRights> wSResponse = await Task.Run(() => JsonConvert.DeserializeObject<WSResponse<VBUserRights>>(json));
            //if (wSResponse.Suceeded)
            //{
            //    if (wSResponse.Data.Translation != null)
            //        wSResponse.Data.Translation.RebuildDictionary();
            //}

            //json = await vbWebService.Client.GetStringAsync(new Uri(VBWebServiceConst.UriMaterial, UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "Materials.json", json);

            //783e03b8-1655-4823-bac0-27eb47bbab5a
            //2e09e3b8-2495-426e-a211-6753002105fa
            json = await vbWebService.Client.GetStringAsync(new Uri(String.Format(VBWebServiceConst.UriMaterial_SumID_F, "2e09e3b8-2495-426e-a211-6753002105fa"), UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "MaterialsSum.json", json);

            json = await vbWebService.Client.GetStringAsync(new Uri(String.Format(VBWebServiceConst.UriMaterial_Bookings_F, "2e09e3b8-2495-426e-a211-6753002105fa", "2019-09-30T23:00:00.0000000Z", "2020-02-13T12:57:13.4289259Z"), UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "MaterialsBookings.json", json);

            json = await vbWebService.Client.GetStringAsync(new Uri(VBWebServiceConst.UriFacility, UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "Facilities.json", json);

            json = await vbWebService.Client.GetStringAsync(new Uri(String.Format(VBWebServiceConst.UriFacility_SumID_F, "c51b52b8-dcec-4354-8239-9247677b5a46"), UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "FacilitiesSum.json", json);

            json = await vbWebService.Client.GetStringAsync(new Uri(String.Format(VBWebServiceConst.UriFacility_Bookings_F, "c51b52b8-dcec-4354-8239-9247677b5a46", "1969-12-31T23:00:00.0000000Z", "2020-02-13T12:57:13.4289259Z"), UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "FacilitiesBookings.json", json);

            json = await vbWebService.Client.GetStringAsync(new Uri(VBWebServiceConst.UriFacilityLot, UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "FacilityLots.json", json);

            json = await vbWebService.Client.GetStringAsync(new Uri(String.Format(VBWebServiceConst.UriFacilityLot_SumID_F, "f5f896da-1ab3-401f-bb53-02200867fb00"), UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "FacilityLotsSum.json", json);

            json = await vbWebService.Client.GetStringAsync(new Uri(String.Format(VBWebServiceConst.UriFacilityLot_Bookings_F, "f5f896da-1ab3-401f-bb53-02200867fb00", "1969-12-31T23:00:00.0000000Z", "2020-02-13T12:57:13.4289259Z"), UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "FacilityLotsBookings.json", json);

            json = await vbWebService.Client.GetStringAsync(new Uri(String.Format(VBWebServiceConst.UriFacilityLocation_SumID_F, "6417E956-8941-4373-92E4-7BC7FA8A3D9D"), UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "FacilityLocationSum.json", json);

            json = await vbWebService.Client.GetStringAsync(new Uri(String.Format(VBWebServiceConst.UriFacilityLocation_Bookings_F, "6417E956-8941-4373-92E4-7BC7FA8A3D9D", "2020-01-30T23:00:00.0000000Z", "2020-02-13T12:57:13.4289259Z"), UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "FacilityLocationBookings.json", json);

            json = await vbWebService.Client.GetStringAsync(new Uri(VBWebServiceConst.UriFacilityCharge, UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "FacilityCharges.json", json);

            json = await vbWebService.Client.GetStringAsync(new Uri(String.Format(VBWebServiceConst.UriFacilityCharge_Bookings_F, "7e0eba7d-2f8e-4b78-9c6b-0002f8bacc37", "1969-12-31T23:00:00.0000000Z", "2020-02-13T12:57:13.4289259Z"), UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "FacilityChargesBookings.json", json);

            json = await vbWebService.Client.GetStringAsync(new Uri(VBWebServiceConst.UriPicking, UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "Pickings.json", json);

            #region ProdOrder

            //json = await vbWebService.Client.GetStringAsync(new Uri(VBWebServiceConst.UriProdOrderPartslists, UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "ProdOrderPartslists.json", json);

            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderPLIntermediates_F, "7d4b1478-9efe-48fe-8c8a-c89d706ae09f"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "ProdOrderPLIntermediates.json", json);


            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderIntermBatches_F, "296e02fe-8767-4b85-a4f1-f7d7810ab4c8"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "POPLIm-296e02fe-8767-4b85-a4f1-f7d7810ab4c8.json", json);

            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderIntermBatches_F, "d2f415cf-c01f-4a96-9b35-7d0af8f1c192"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "POPLIm-d2f415cf-c01f-4a96-9b35-7d0af8f1c192.json", json);

            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderIntermBatches_F, "bb5f464b-0208-4588-94b1-36037d7e8d7d"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "POPLIm-bb5f464b-0208-4588-94b1-36037d7e8d7d.json", json);

            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderIntermBatches_F, "0ffd2763-ed12-4c03-bd7e-3abd82c4cfef"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "POPLIm-0ffd2763-ed12-4c03-bd7e-3abd82c4cfef.json", json);


            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderInputMaterials_F, "a55a41f1-1113-4032-a9fc-23ae4c6a4636"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "POInM-a55a41f1-1113-4032-a9fc-23ae4c6a4636.json", json);

            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderInputMaterials_F, "9dc63e9a-6646-413d-bd63-4482a45901bd"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "POInM-9dc63e9a-6646-413d-bd63-4482a45901bd.json", json);

            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderInputMaterials_F, "4b3e016e-4652-4203-94db-8b5788cbd9f2"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "POInM-4b3e016e-4652-4203-94db-8b5788cbd9f2.json", json);


            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderInputMaterials_F, "6ffd630a-97d9-4ce7-b362-eb59a365a4a1"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "POInM-6ffd630a-97d9-4ce7-b362-eb59a365a4a1.json", json);



            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderPosRelFacilityBooking_F, "3461fa88-4a7d-406a-850c-bd4a0a911772"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "PORelFB-3461fa88-4a7d-406a-850c-bd4a0a911772.json", json);

            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderPosRelFacilityBooking_F, "794e9d34-c97c-4b1e-a852-3fc9127aaa92"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "PORelFB-794e9d34-c97c-4b1e-a852-3fc9127aaa92.json", json);

            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderPosRelFacilityBooking_F, "35e46d81-4282-4e38-bcbc-5c91916d054d"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "PORelFB-35e46d81-4282-4e38-bcbc-5c91916d054d.json", json);


            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderPosFacilityBooking_F, "a55a41f1-1113-4032-a9fc-23ae4c6a4636"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "POPosFB-a55a41f1-1113-4032-a9fc-23ae4c6a4636.json", json);

            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderPosFacilityBooking_F, "9dc63e9a-6646-413d-bd63-4482a45901bd"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "POPosFB-9dc63e9a-6646-413d-bd63-4482a45901bd.json", json);

            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderPosFacilityBooking_F, "4b3e016e-4652-4203-94db-8b5788cbd9f2"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "POPosFB-4b3e016e-4652-4203-94db-8b5788cbd9f2.json", json);

            //json = await vbWebService.Client.GetStringAsync(new Uri(string.Format(VBWebServiceConst.UriProdOrderPosFacilityBooking_F, "6ffd630a-97d9-4ce7-b362-eb59a365a4a1"), UriKind.Relative));
            //File.WriteAllText(JsonMockFolder + "POPosFB-6ffd630a-97d9-4ce7-b362-eb59a365a4a1.json", json);

            #endregion

            #region Faciliy Inventory 

            MDFacilityInventoryState.FacilityInventoryStates inventoryState = MDFacilityInventoryState.FacilityInventoryStates.InProgress;
            DateTime inventoryStartDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime inventoryEndDate = inventoryStartDate.AddYears(1);
            string inventoryUrl = string.Format(VBWebServiceConst.UrlInventory_Inventories_F, (short)inventoryState, inventoryStartDate.ToString("o"), inventoryEndDate.ToString("o"));
            json = await vbWebService.Client.GetStringAsync(new Uri(inventoryUrl, UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "FacilityInventories.json", json);

            string facilityInventoryNo = "00000003";
            string inventoryPosUrl = string.Format(VBWebServiceConst.UrlInventory_InventoryLines_F, facilityInventoryNo,
                CoreWebServiceConst.EmptyParam, CoreWebServiceConst.EmptyParam, CoreWebServiceConst.EmptyParam, CoreWebServiceConst.EmptyParam, CoreWebServiceConst.EmptyParam, CoreWebServiceConst.EmptyParam, CoreWebServiceConst.EmptyParam);
            json = await vbWebService.Client.GetStringAsync(new Uri(inventoryPosUrl, UriKind.Relative));
            File.WriteAllText(JsonMockFolder + "FacilityInventoryLines.json", json);

            #endregion

            Console.WriteLine("Completed. Press Enter");
        }
    }
}
