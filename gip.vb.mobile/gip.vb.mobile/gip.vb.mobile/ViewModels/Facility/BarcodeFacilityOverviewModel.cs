using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using gip.mes.webservices;
using System.Collections.Generic;
using gip.core.webservices;
using gip.core.datamodel;
using System.Linq;

namespace gip.vb.mobile.ViewModels
{
    public class BarcodeFacilityOverviewModel : BaseViewModel
    {
        public BarcodeFacilityOverviewModel()
        {
            LoadFilteredMaterialsCommand = new Command(async () => await ExecuteLoadFilteredMaterialsCommand());
            LoadLocationsCommand = new Command(async () => await ExecuteLoadLocationsCommand());
            LoadFilteredFacilitiesCommand = new Command(async () => await ExecuteLoadFilteredFacilitiesCommand());
            LoadFilteredLotsCommand = new Command(async () => await ExecuteLoadFilteredLotsCommand());
            LoadBarcodeEntityCommand = new Command(async () => await ExecuteLoadBarcodeEntityCommand());

            ResetScanSequence();

            //MessagingCenter.Subscribe<NewItemPage, FacilityCharge>(this, "AddItem", async (obj, item) =>
            //{
            //    var newItem = item as FacilityCharge;
            //    Items.Add(newItem);
            //    await _WebService.AddItemAsync(newItem);
            //});
        }

        #region Properties
        private bool _ZXingIsScanning;
        public bool ZXingIsScanning
        {
            get
            {
                return _ZXingIsScanning;
            }
            set
            {
                SetProperty(ref _ZXingIsScanning, value);
                OnPropertyChanged();
            }
        }

        private List<Material> _FilteredMaterials = new List<Material>();
        public List<Material> FilteredMaterials
        {
            get
            {
                return _FilteredMaterials;
            }
            set
            {
                SetProperty(ref _FilteredMaterials, value);
            }
        }

        public string _MaterialSearch;
        public string MaterialSearch
        {
            get
            {
                return _MaterialSearch;
            }
            set
            {
                SetProperty(ref _MaterialSearch, value);
            }
        }


        private List<Facility> _Locations = new List<Facility>();
        public List<Facility> Locations
        {
            get
            {
                return _Locations;
            }

            set
            {
                SetProperty(ref _Locations, value);
            }
        }

        public Facility _SelectedLocation;
        public Facility SelectedLocation
        {
            get
            {
                return _SelectedLocation;
            }
            set
            {
                SetProperty(ref _SelectedLocation, value);
            }
        }

        
        private List<Facility> _FilteredFacilites = new List<Facility>();
        public List<Facility> FilteredFacilites
        {
            get
            {
                return _FilteredFacilites;
            }

            set
            {
                SetProperty(ref _FilteredFacilites, value);
            }
        }

        public string _FacilitySearch;
        public string FacilitySearch
        {
            get
            {
                return _FacilitySearch;
            }
            set
            {
                SetProperty(ref _FacilitySearch, value);
            }
        }


        private List<FacilityLot> _FilteredLots = new List<FacilityLot>();
        public List<FacilityLot> FilteredLots
        {
            get
            {
                return _FilteredLots;
            }

            set
            {
                SetProperty(ref _FilteredLots, value);
            }
        }

        public string _LotSearch;
        public string LotSearch
        {
            get
            {
                return _LotSearch;
            }
            set
            {
                SetProperty(ref _LotSearch, value);
            }
        }

        public string _CurrentBarcode;
        public string CurrentBarcode
        {
            get
            {
                return _CurrentBarcode;
            }
            set
            {
                SetProperty(ref _CurrentBarcode, value);
            }
        }

        public BarcodeEntity _WSBarcodeEntityResult;
        public BarcodeEntity WSBarcodeEntityResult
        {
            get
            {
                return _WSBarcodeEntityResult;
            }
            set
            {
                SetProperty(ref _WSBarcodeEntityResult, value);
            }
        }

        private BarcodeSequence _BarcodeSequence;
        public BarcodeSequence BarcodeSequence
        {
            get => _BarcodeSequence;
            set
            {
                _BarcodeSequence = value;
                OnPropertyChanged();
            }
        }

        public List<object> _CurrentBarcodeEntity;
        public List<object> CurrentBarcodeEntity
        {
            get
            {
                return _CurrentBarcodeEntity;
            }
            set
            {
                SetProperty(ref _CurrentBarcodeEntity, value);
            }
        }

        public FacilitySelectorViewModel FacilitySelector
        {
            get;
            set;
        }

        #endregion

        #region Methods
        public void Clear()
        {
            CurrentBarcode = "";
        }

        public Command LoadFilteredMaterialsCommand { get; set; }
        public async Task ExecuteLoadFilteredMaterialsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.SearchMaterialAsync(MaterialSearch);
                this.WSResponse = response;
                if (response.Suceeded)
                    FilteredMaterials = response.Data;
                else
                    FilteredMaterials = new List<Material>();
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


        public Command LoadLocationsCommand { get; set; }
        public async Task ExecuteLoadLocationsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.SearchFacilityAsync(CoreWebServiceConst.EmptyParam, CoreWebServiceConst.EmptyParam, "1000");
                this.WSResponse = response;
                if (response.Suceeded)
                    Locations = response.Data;
                else
                    Locations = new List<Facility>();
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


        public Command LoadFilteredFacilitiesCommand { get; set; }
        public async Task ExecuteLoadFilteredFacilitiesCommand()
        {
            if (IsBusy || SelectedLocation == null)
                return;

            IsBusy = true;

            try
            {
                string searchTerm = FacilitySearch;
                if (string.IsNullOrEmpty(searchTerm))
                    searchTerm = CoreWebServiceConst.EmptyParam;

                var response = await _WebService.SearchFacilityAsync(searchTerm, SelectedLocation.FacilityID.ToString(), CoreWebServiceConst.EmptyParam);
                this.WSResponse = response;
                if (response.Suceeded)
                    FilteredFacilites = response.Data;
                else
                    FilteredFacilites = new List<Facility>();
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

        public Command LoadFilteredLotsCommand { get; set; }
        public async Task ExecuteLoadFilteredLotsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.SearchFacilityLotAsync(LotSearch);
                this.WSResponse = response;
                if (response.Suceeded)
                    FilteredLots = response.Data;
                else
                    FilteredLots = new List<FacilityLot>();
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

        public Command LoadBarcodeEntityCommand { get; set; }
        public async Task ExecuteLoadBarcodeEntityCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                BarcodeSequence.CurrentBarcode = CurrentBarcode;

                var response = await _WebService.InvokeBarcodeSequenceAsync(BarcodeSequence);
                BarcodeSequence = response.Data;
                this.WSResponse = response;

                BarcodeEntity facilityChargeEntity = BarcodeSequence.Sequence.Where(c => c.FacilityCharge != null).FirstOrDefault();
                if (facilityChargeEntity == null)
                {
                    this.Message = response.Data.Message;
                    CurrentBarcodeEntity = BarcodeSequence.Sequence.Where(c => c.ValidEntity != null).Select(c => c.ValidEntity).ToList();
                }
                else
                {
                    this.WSBarcodeEntityResult = facilityChargeEntity;
                    List<object> entries = new List<object>();
                    entries.Add(facilityChargeEntity.FacilityCharge);
                    CurrentBarcodeEntity = entries;

                    this.Message = null;
                    ResetScanSequence();
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

        public void OnAppear()
        {
            if (FacilitySelector != null)
            {
                SelectedLocation = FacilitySelector.SelectedStorageLocation;
                FacilitySelector = null;
                //LoadFilteredFacilitiesCommand.Execute(null);
            }
            else if (Locations.Count == 0)
            {
                LoadLocationsCommand.Execute(null);
            }
        }

        public void NavigateToOverview()
        {
            if (SelectedLocation != null)
            {
                if (SelectedLocation.MDFacilityType.MDFacilityTypeIndex == (short)mes.datamodel.FacilityTypesEnum.StorageBin)
                {

                }
                else
                {
                    
                }
            }
        }

        public void ResetScanSequence()
        {
            BarcodeSequence = new BarcodeSequence();
            CurrentBarcode = null;
        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }

        #endregion
    }
}