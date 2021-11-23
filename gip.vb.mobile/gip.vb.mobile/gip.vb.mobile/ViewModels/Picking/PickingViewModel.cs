﻿using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class PickingViewModel : BaseViewModel
    {
        public Command LoadPickingTypesCommand { get; set; }

        public Command LoadStorageLocationsCommand { get; set; }

        #region c'tors

        public PickingViewModel()
        {
            LoadPickingTypesCommand = new Command(async () => await ExecuteLoadPickingTypesCommand());
            LoadStorageLocationsCommand = new Command(async () => await ExecuteLoadStorageLocationsCommand());
        }

        #endregion

        #region Properties

        private List<MDPickingType> _PickingTypes;
        public List<MDPickingType> PickingTypes
        {
            get
            {
                return _PickingTypes;
            }
            set
            {
                SetProperty(ref _PickingTypes, value);
            }
        }

        private MDPickingType _SelectedPickingType;
        public MDPickingType SelectedPickingType
        {
            get
            {
                return _SelectedPickingType;
            }
            set
            {
                SetProperty(ref _SelectedPickingType, value);
            }
        }

        private DateTime _DateFrom;
        public DateTime DateFrom
        {
            get
            {
                return _DateFrom;
            }
            set
            {
                SetProperty(ref _DateFrom, value);
            }
        }

        private DateTime _DateTo;
        public DateTime DateTo
        {
            get
            {
                return _DateTo;
            }
            set
            {
                SetProperty(ref _DateTo, value);
            }
        }

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

        private Facility _SelectedStorageLocationFrom;
        public Facility SelectedStorageLocationFrom
        {
            get
            {
                return _SelectedStorageLocationFrom;
            }
            set
            {
                SetProperty(ref _SelectedStorageLocationFrom, value);
            }
        }

        private Facility _SelectedStorageLocationTo;
        public Facility SelectedStorageLocationTo
        {
            get
            {
                return _SelectedStorageLocationTo;
            }
            set
            {
                SetProperty(ref _SelectedStorageLocationTo, value);
            }
        }

        #endregion

        public async Task ExecuteLoadPickingTypesCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetPickingTypesAsync();
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    PickingTypes = response.Data;
                }
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }

            await ExecuteLoadStorageLocationsCommand();
        }

        public async Task ExecuteLoadStorageLocationsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.SearchFacilityAsync("*", "*", ((short)MDFacilityType.FacilityTypes.StorageLocation).ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    StorageLocations = response.Data;
                }
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            
        }
    }
}