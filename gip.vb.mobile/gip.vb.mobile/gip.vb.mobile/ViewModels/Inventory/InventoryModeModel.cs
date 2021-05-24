using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels.Inventory
{
    public class InventoryModeModel : BaseViewModel
    {

        #region ctor's
        public InventoryModeModel()
        {
            // Commands
            GetFacilitiesCommand = new Command(async () => await ExecuteGetFacilities());
        }
        #endregion

        #region Commands
        public Command GetFacilitiesCommand { get; private set; }
        #endregion

        #region Overrides
        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }

        #endregion
        #region Params


        private string _FacilityInventoryNo;
        public string FacilityInventoryNo
        {
            get
            {
                return _FacilityInventoryNo;
            }
            set
            {
                SetProperty(ref _FacilityInventoryNo, value);
            }
        }

        private bool _IsValidateAndComplete;
        public bool IsValidateAndComplete
        {
            get
            {
                return _IsValidateAndComplete;
            }
            set
            {
                SetProperty(ref _IsValidateAndComplete, value);
            }
        }

        #endregion

        #region Facility Select&List

        #region Facility Select&List -> Facility (Storage place)

        private List<Facility> _StorageLocations;
        public List<Facility> StorageLocations
        {
            get
            {
                return _StorageLocations;
            }
            set
            {
                SetProperty(ref _StorageLocations, value);
            }
        }

        private Facility _SelectedStorageLocation;
        public Facility SelectedStorageLocation
        {
            get
            {
                return _SelectedStorageLocation;
            }
            set
            {
                if (_SelectedStorageLocation != value)
                {
                    SetProperty(ref _SelectedStorageLocation, value);
                    Facilities.Clear();
                    if (SelectedStorageLocation != null)
                    {
                        List<Facility> facilities =
                            AllFacilities
                            .Where(c => c.ParentFacilityID == SelectedStorageLocation.FacilityID)
                            .OrderBy(c => c.FacilityNo)
                            .ToList();
                        Facilities = facilities;
                    }
                    else
                        SelectedFacility = null;
                }
            }
        }

        #endregion

        #region Facility Select&List -> Facility (Faciliy
        public List<Facility> AllFacilities { get; set; }

        private List<Facility> _Facilities;
        public List<Facility> Facilities
        {
            get
            {
                if (_Facilities == null)
                    _Facilities = new List<Facility>();
                return _Facilities;
            }
            set
            {
                SetProperty(ref _Facilities, value);
            }
        }

        private Facility _SelectedFacility;
        public Facility SelectedFacility
        {
            get
            {
                return _SelectedFacility;
            }
            set
            {
                SetProperty(ref _SelectedFacility, value);
                OnPropertyChanged("IsEnabledModeMoveForward");
            }
        }

        public bool IsEnabledModeMoveForward
        {
            get
            {
                return SelectedFacility != null;
            }
        }

        #endregion

        #endregion

        #region Tasks

        /// <summary>
        /// Calling GetFacilityInventoriesAsync
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExecuteGetFacilities()
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    WSResponse<List<Facility>> wSResponse = await _WebService.GetFacilitiesAsync();
                    WSResponse = wSResponse;
                    success = wSResponse.Suceeded;
                    if (success)
                    {
                        AllFacilities = wSResponse.Data;
                        StorageLocations = AllFacilities.Where(c => c.ParentFacilityID == null).OrderBy(c => c.FacilityNo).ToList();
                        SelectedStorageLocation = null;

                        Facilities.Clear();
                        SelectedFacility = null;
                    }
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


    }
}
