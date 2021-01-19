using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class FacilityInventoryLinesViewModel : BaseViewModel
    {

        #region Properties

        private List<MDFacilityInventoryPosState> _MDFacilityInventoryPosStates;
        public List<MDFacilityInventoryPosState> MDFacilityInventoryPosStates
        {
            get
            {
                return _MDFacilityInventoryPosStates;
            }
            set
            {
                SetProperty(ref _MDFacilityInventoryPosStates, value);
            }
        }

        private List<FacilityInventoryPos> _FacilityInventoryPoses;
        public List<FacilityInventoryPos> FacilityInventoryPoses
        {
            get
            {
                return _FacilityInventoryPoses;
            }
            set
            {
                SetProperty(ref _FacilityInventoryPoses, value);
            }
        }

        private FacilityInventoryPos _SelectedFacilityInventoryPos;
        public FacilityInventoryPos SelectedFacilityInventoryPos
        {
            get
            {
                return _SelectedFacilityInventoryPos;
            }
            set
            {
                SetProperty(ref _SelectedFacilityInventoryPos, value);
            }
        }

        #endregion

        #region Filter

        public string FilterFacilityInventoryNo { get; set; }

        private MDFacilityInventoryPosState _FilterFacilityInventoryPosState;
        public MDFacilityInventoryPosState FilterFacilityInventoryPosState
        {
            get
            {
                return _FilterFacilityInventoryPosState;
            }
            set
            {
                SetProperty(ref _FilterFacilityInventoryPosState, value);
            }
        }

        private Facility _FilterInventoryPosFacility;
        public Facility FilterInventoryPosFacility
        {
            get
            {
                return _FilterInventoryPosFacility;
            }
            set
            {
                SetProperty(ref _FilterInventoryPosFacility, value);
            }
        }

        private Material _FilterInventoryPosMaterial;
        public Material FilterInventoryPosMaterial
        {
            get
            {
                return _FilterInventoryPosMaterial;
            }
            set
            {
                SetProperty(ref _FilterInventoryPosMaterial, value);
            }
        }

        private string _FilterInventoryPosLotNo;
        public string FilterInventoryPosLotNo
        {
            get
            {
                return _FilterInventoryPosLotNo;
            }
            set
            {
                SetProperty(ref _FilterInventoryPosLotNo, value);
            }
        }

        private FacilityCharge _FilterInventoryPosFacilityCharge;
        public FacilityCharge FilterInventoryPosFacilityCharge
        {
            get
            {
                return _FilterInventoryPosFacilityCharge;
            }
            set
            {
                SetProperty(ref _FilterInventoryPosFacilityCharge, value);
            }
        }

        private string _InputCode;
        public string InputCode
        {
            get
            {
                return _InputCode;
            }
            set
            {
                SetProperty(ref _InputCode, value);
            }
        }

        #endregion

        #region ctor's

        public FacilityInventoryLinesViewModel(string facilityInventoryNo)
        {
            FilterFacilityInventoryNo = facilityInventoryNo;

            //Commands
            GetListsCommand = new Command(async () => await ExecuteGetListsAsync());
            GetFacilityInventoryPosesCommand = new Command(async () => await ExecuteGetFacilityInventoryPosesAsync());
        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {

        }

        #endregion

        #region Commands
        public Command GetListsCommand { get; set; }

        public Command GetFacilityInventoryPosesCommand { get; private set; }

        #endregion

        #region Tasks
        /// <summary>
        /// Calling ExecuteGetListsAsync
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExecuteGetListsAsync()
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    WSResponse<List<MDFacilityInventoryPosState>> wSResponsemDFacilityInventoryPosState = await _WebService.GetMDFacilityInventoryPosStatesAsync();
                    MDFacilityInventoryPosStates = wSResponsemDFacilityInventoryPosState.Data;
                    success = wSResponsemDFacilityInventoryPosState.Suceeded;
                }
                catch (Exception ex)
                {
                    Message = new Msg(eMsgLevel.Exception, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
            return success;
        }

        /// <summary>
        /// Calling GetFacilityInventoryPosesAsync
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExecuteGetFacilityInventoryPosesAsync()
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    string facilityNo = null;
                    if (FilterInventoryPosFacility != null)
                        facilityNo = FilterInventoryPosFacility.FacilityNo;
                   
                    string materialNo = null;
                    if (FilterInventoryPosMaterial != null)
                        materialNo = FilterInventoryPosMaterial.MaterialNo;
                    
                    short? mdFacilityInventoryPosStateIndex = null;
                    if (FilterFacilityInventoryPosState != null)
                        mdFacilityInventoryPosStateIndex = (short)FilterFacilityInventoryPosState.MDFacilityInventoryPosStateIndex;

                    WSResponse<List<FacilityInventoryPos>> wSResponse = await _WebService.GetFacilityInventoryPosesAsync(FilterFacilityInventoryNo, InputCode, facilityNo,
                        FilterInventoryPosLotNo, materialNo, mdFacilityInventoryPosStateIndex != null ? mdFacilityInventoryPosStateIndex.Value.ToString() : "", null, null);
                    FacilityInventoryPoses = wSResponse.Data;
                    WSResponse = wSResponse;
                    success = wSResponse.Suceeded;
                }
                catch (Exception ex)
                {
                    Message = new Msg(eMsgLevel.Exception, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
            return success;
        }

        #endregion

        #region Methods

        private async Task<bool> ApplyInputCode(string value)
        {
            bool success = false;
            Guid itemID = Guid.Empty;
            if (Guid.TryParse(value, out itemID))
            {
                WSResponse<FacilityCharge> facilityChargeResponse = await _WebService.GetFacilityChargeAsync(value);
                if (facilityChargeResponse.Suceeded && facilityChargeResponse.Data != null)
                {
                    FilterInventoryPosFacilityCharge = facilityChargeResponse.Data;
                    success = true;
                }
                else
                {
                    WSResponse<Facility> facilityResponse = await _WebService.GetFacilityAsync(value);
                    if (facilityResponse.Suceeded && facilityResponse.Data != null)
                    {
                        FilterInventoryPosFacility = facilityResponse.Data;
                        success = true;
                    }
                    else
                    {
                        WSResponse<Material> materialResponse = await _WebService.GetMaterialAsync(value);
                        if (materialResponse.Suceeded && materialResponse.Data != null)
                        {
                            FilterInventoryPosMaterial = materialResponse.Data;
                            success = true;
                        }
                    }
                }
            }
            else
            {
                WSResponse<List<Facility>> wSResponseFacilities = await _WebService.SearchFacilityAsync(value, CoreWebServiceConst.EmptyParam, CoreWebServiceConst.EmptyParam);
                if (wSResponseFacilities.Suceeded)
                {
                    FilterInventoryPosFacility = wSResponseFacilities.Data.FirstOrDefault();
                    if (FilterInventoryPosFacility != null)
                        success = true;
                }
                else
                {
                    WSResponse<List<Material>> wSResponseMaterials = await _WebService.SearchMaterialAsync(value);
                    if (wSResponseMaterials.Suceeded)
                    {
                        FilterInventoryPosMaterial = wSResponseMaterials.Data.FirstOrDefault();
                        if (FilterInventoryPosMaterial != null)
                            success = true;
                    }
                }
            }

            return success;
        }

        #region Methods -> Event Handlers

        #endregion
        #endregion

    }
}
