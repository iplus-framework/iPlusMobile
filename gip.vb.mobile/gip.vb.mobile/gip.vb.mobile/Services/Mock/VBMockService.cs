using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gip.mes.webservices;
using gip.core.webservices;
using gip.mes.facility;
using gip.core.autocomponent;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using gip.core.datamodel;
using System.Linq;

namespace gip.vb.mobile.Services
{
    public partial class VBMockService : IVBWebService, ILoginService
    {
        VBUser _VBUser;

        public VBMockService()
        {
        }

        private string ReadJsonFile(string fileName)
        {
            Type type = typeof(App);
            Stream stream = type.Assembly.GetManifestResourceStream(type.Namespace + "." + fileName);
            string json = "";
            using (var reader = new System.IO.StreamReader(stream, Encoding.UTF8))
            {
                json = reader.ReadToEnd();
            }
            return json;
        }

        public void ReloadSettings()
        {
        }


        public async Task<VBUserRights> Login(VBUser user)
        {
            _VBUser = user;
            WSResponse<VBUserRights> result = await Login(user.Username);
            return result.Data;
        }


        public async Task<WSResponse<VBUserRights>> Login(string userName)
        {
            var vbUserRights = JsonConvert.DeserializeObject<WSResponse<VBUserRights>>(ReadJsonFile("Models.JsonMock.VBUserRights.json"));
            vbUserRights.Data.UserName = userName;
            return await Task.FromResult(vbUserRights);
        }

        public async Task<WSResponse<bool>> Logout(string sessionID)
        {
            return await Task.FromResult(new WSResponse<bool>(true));
        }


        public async Task<WSResponse<ACClass>> GetACClassByBarcodeAsync(string barcodeID)
        {
            Guid guid = Guid.Parse(barcodeID);
            return await Task.FromResult(new WSResponse<ACClass>(new ACClass() { ACClassID = guid, ACIdentifier = "MockClass", ACUrlComponent = @"\iPlusMES\MockClass", ACCaptionTranslation = "en{'Mock class'}de{'Mock class'}" }));
        }

        public async Task<WSResponse<bool>> Print(PrintEntity printEntity)
        {
            if (printEntity.Sequence == null || !printEntity.Sequence.Any())
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "sequence is null")));
            return await Task.FromResult(new WSResponse<bool>(true));
        }


        public async Task<WSResponse<BarcodeEntity>> GetBarcodeEntityAsync(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Error, "barcodeID is empty"));

            var result01 = await GetFacilityChargeByBarcodeAsync(barcodeID);
            if (result01.Suceeded && result01.Data != null)
                return new WSResponse<BarcodeEntity>(new BarcodeEntity() { FacilityCharge = result01.Data });

            var result02 = await GetFacilityLotByBarcodeAsync(barcodeID);
            if (result02.Suceeded && result02.Data != null)
                return new WSResponse<BarcodeEntity>(new BarcodeEntity() { FacilityLot = result02.Data });

            var result03 = await GetMaterialByBarcodeAsync(barcodeID);
            if (result03.Suceeded && result03.Data != null)
                return new WSResponse<BarcodeEntity>(new BarcodeEntity() { Material = result03.Data });

            var result04 = await GetFacilityByBarcodeAsync(barcodeID);
            if (result04.Suceeded && result04.Data != null)
                return new WSResponse<BarcodeEntity>(new BarcodeEntity() { Facility = result04.Data });

            var result05 = await GetACClassByBarcodeAsync(barcodeID);
            if (result05.Suceeded && result05.Data != null)
                return new WSResponse<BarcodeEntity>(new BarcodeEntity() { ACClass = result05.Data });

            return new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Error, "Unknown barcode"));
        }


        public async Task<WSResponse<BarcodeSequence>> InvokeBarcodeSequenceAsync(BarcodeSequence sequence)
        {
            if (String.IsNullOrEmpty(sequence.CurrentBarcode))
                return new WSResponse<BarcodeSequence>(sequence, new Msg(eMsgLevel.Error, "barcodeID is empty"));

            WSResponse<BarcodeEntity> nextEntity = await GetBarcodeEntityAsync(sequence.CurrentBarcode);
            if (!nextEntity.Suceeded)
            {
                sequence.State = BarcodeSequence.ActionState.Cancelled;
                sequence.Message = nextEntity.Message;
                return new WSResponse<BarcodeSequence>(sequence, nextEntity.Message);
            }

            if (sequence.Sequence == null)
                sequence.Sequence = new List<BarcodeEntity>();
            else if (sequence.State == BarcodeSequence.ActionState.Completed 
                || sequence.State == BarcodeSequence.ActionState.Cancelled
                || sequence.Sequence.Count >= 3)
                sequence.Sequence = new List<BarcodeEntity>();
            sequence.Sequence.Add(nextEntity.Data);

            sequence.Message = new Msg(eMsgLevel.Info, String.Format("TODO Step {0}", sequence.Sequence.Count));
            sequence.State = sequence.Sequence.Count >= 3 ? BarcodeSequence.ActionState.Completed : BarcodeSequence.ActionState.ScanAgain;
            return new WSResponse<BarcodeSequence>(sequence);
        }

        public Task<WSResponse<MsgWithDetails>> BookFacilitiesAsync(ACMethodBookingList bpParams)
        {
            return new Task<WSResponse<MsgWithDetails>>(null);
        }

        public Task<WSResponse<MsgWithDetails>> FinishPickingOrderAsync(Guid pickingID)
        {
            throw new NotImplementedException();
        }

        public Task<WSResponse<MsgWithDetails>> FinishPickingOrderWithoutCheckAsync(Guid pickingID)
        {
            throw new NotImplementedException();
        }

        public Task<WSResponse<string>> GetAssignedPrinterAsync()
        {
            throw new NotImplementedException();
        }

        public Task<WSResponse<string>> GetScannedPrinterAsync(string printerID)
        {
            throw new NotImplementedException();
        }

        public Task<WSResponse<string>> AssignPrinterAsync(string printerID)
        {
            throw new NotImplementedException();
        }

        public Task<WSResponse<FacilityCharge>> GetFacilityChargeFromFacilityMaterialLotAsync(string facilityID, string materialID, string facilityLotID, string splitNo)
        {
            throw new NotImplementedException();
        }
    }
}