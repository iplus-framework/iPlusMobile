using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class FacilitySelectorViewModel : BaseViewModel, IBarcodeScanFacilityHost
    {
        public Command LoadStorageLocationsCommand { get; set; }

        public Command AddFacilityToPath { get; set; }

        public Command ClearFacilityPath { get; set; }

        public Command RemoveLastFacilityInPath { get; set; }

        public FacilitySelectorViewModel(string invoker)
        {
            Invoker = invoker;

            LoadStorageLocationsCommand = new Command(async () => await ExecuteLoadStorageLocationsCommand());
            AddFacilityToPath = new Command(async (param) => await ExecuteAddFacilityToPath(param));

            ClearFacilityPath = new Command(async (param) => await ExecuteClearFacilityPath());
            RemoveLastFacilityInPath = new Command(async (param) => await ExecuteRemoveLastFacilityInPath());

            FacilityScanViewModel = new BarcodeScanFacilityModel(this);
            FacilityScanViewModel.BarcodeIssuer = mes.datamodel.BarcodeSequenceBase.BarcodeIssuerEnum.Facility;

            StorageLocationPath = new List<Facility>();
        }

        public string Invoker
        {
            get;
            set;
        }

        private bool _SearchOnlyInList;
        public bool SearchOnlyInList
        {
            get => _SearchOnlyInList;
            set
            {
                SetProperty<bool>(ref _SearchOnlyInList, value);
            }
        }

        public BarcodeScanFacilityModel FacilityScanViewModel;

        private List<Facility> _StorageLocationPath;

        public List<Facility> StorageLocationPath
        {
            get => _StorageLocationPath;
            set
            {
                SetProperty<List<Facility>>(ref _StorageLocationPath, value);
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

        private Facility _SelectedStorageLocation;
        public Facility SelectedStorageLocation
        {
            get
            {
                return _SelectedStorageLocation;
            }
            set
            {
                SetProperty(ref _SelectedStorageLocation, value);
            }
        }


        public async Task ExecuteLoadStorageLocationsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.SearchFacilityAsync("*", "*", ((short)FacilityTypesEnum.StorageLocation).ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    StorageLocations = response.Data.OrderBy(c => c.FacilityNo).ToList();
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

        public async Task ExecuteAddFacilityToPath(object param)
        {
            Facility facility = param as Facility;
            if (facility == null)
                return;

            StorageLocationPath.Add(facility);

            StorageLocationPath = new List<Facility>(StorageLocationPath);

            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.SearchFacilityAsync("*", facility.FacilityID.ToString(), "*");
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    StorageLocations = response.Data.OrderBy(c => c.FacilityNo).ToList();
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

        public async Task ExecuteClearFacilityPath()
        {
            StorageLocationPath.Clear(); 
            StorageLocationPath = new List<Facility>(StorageLocationPath);

            await ExecuteLoadStorageLocationsCommand();
        }

        public async Task ExecuteRemoveLastFacilityInPath()
        {
            Facility last = StorageLocationPath.LastOrDefault();
            if (last != null)
            {
                StorageLocationPath.Remove(last); 
                StorageLocationPath = new List<Facility>(StorageLocationPath);
            }

            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                string parentFacilityID = core.webservices.CoreWebServiceConst.EmptyParam;
                string facilityType = core.webservices.CoreWebServiceConst.EmptyParam;

                Facility lastAfterRemove = StorageLocationPath.LastOrDefault();
                if (lastAfterRemove != null)
                    parentFacilityID = lastAfterRemove?.FacilityID.ToString();
                else
                    facilityType = ((short)FacilityTypesEnum.StorageLocation).ToString();

                var response = await _WebService.SearchFacilityAsync("*", parentFacilityID, facilityType);
                this.WSResponse = response;
                if (response.Suceeded && response.Data != null)
                {
                    StorageLocations = response.Data.OrderBy(c => c.FacilityNo).ToList();
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

        public void OnFacilityScanned(Facility facility)
        {
            List<Facility> tempList = new List<Facility>();
            if (facility != null)
                tempList.Add(facility);
            StorageLocationPath = new List<Facility>();
            StorageLocations = tempList;
        }

        public async void OnSearchFacility(string searchText)
        {
            if (SearchOnlyInList && StorageLocations != null)
            {
                string text = searchText?.ToLower();
                StorageLocations = StorageLocations.Where(c => c.FacilityNo.ToLower().Contains(text) || c.FacilityName.ToLower().Contains(text)).ToList();
            }
            else
            {
                if (IsBusy)
                    return;

                IsBusy = true;

                try
                {
                    StorageLocationPath = new List<Facility>();
                    string parentFacilityID = "*";
                    Facility lastAfterRemove = StorageLocationPath.LastOrDefault();
                    if (lastAfterRemove != null)
                        parentFacilityID = lastAfterRemove?.FacilityID.ToString();

                    var response = await _WebService.SearchFacilityAsync(searchText, "*", "*");
                    this.WSResponse = response;
                    if (response.Suceeded)
                    {
                        StorageLocations = response.Data.OrderBy(c => c.FacilityNo).ToList();
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
        }
    }
}
