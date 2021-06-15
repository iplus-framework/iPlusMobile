﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Fetch list of facilities
        /// </summary>
        public Command GetFacilitiesCommand { get; private set; }

        #endregion

        #region Overrides

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }

        #endregion

        #region Properties 


        #region Properties -> Params

        private string _FacilityInventoryNo;
        /// <summary>
        /// FacilityInventoryNo working on
        /// </summary>
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
        /// <summary>
        /// Validate and complete mode
        /// </summary>
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

        public bool IsEnabledModeMoveForward
        {
            get
            {
                return SelectedStorageLocation != null;
            }
        }

        #endregion

        #region Properties -> Facilities

        public List<Facility> AllFacilities { get; set; }

        #region Properties -> Facilities -> StorageLocations

        private List<Facility> _StorageLocations;
        /// <summary>
        /// Storage location list - dropdown select
        /// </summary>
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
        /// <summary>
        /// Selected storage location
        /// </summary>
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
                    OnPropertyChanged("IsEnabledModeMoveForward");
                }
            }
        }

        #endregion

        #region Properties -> Facilities -> Facilities


        private List<Facility> _Facilities;
        /// <summary>
        /// Facilities (end storage points)
        /// </summary>
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
        /// <summary>
        /// Selected facility
        /// </summary>
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

        #endregion

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