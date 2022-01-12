using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.datamodel;
using gip.mes.webservices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class FacilitySortInfoOverLocationModel : BaseViewModel
    {
        public FacilitySortInfoOverLocationModel()
        {
            LoadStorageLocations = new Command(async () => await ExecuteLoadStorageLocations());
            SelectedStorageLocations = new ObservableCollection<object>();
        }

        private List<Facility> _AvailableStorageLocations;
        public List<Facility> AvailableStorageLocations
        {
            get => _AvailableStorageLocations;
            set
            {
                SetProperty(ref _AvailableStorageLocations, value);
            }
        }


        private ObservableCollection<object> _SelectedStorageLocations;
        public ObservableCollection<object> SelectedStorageLocations
        {
            get => _SelectedStorageLocations;
            set
            {
                SetProperty(ref _SelectedStorageLocations, value);
            }
        }

        public Command LoadStorageLocations { get; set; }
        public async Task ExecuteLoadStorageLocations()
        {
            if (IsBusy)
                return;

            try
            {
                var response = await _WebService.SearchFacilityAsync(CoreWebServiceConst.EmptyParam, CoreWebServiceConst.EmptyParam, ((short)FacilityTypesEnum.StorageLocation).ToString());
                if (response.Suceeded)
                {
                    AvailableStorageLocations = response.Data;
                    Facility[] selectedLocations = LoadSortStorageLocations();

                    if (selectedLocations != null)
                    {
                        SelectedStorageLocations = new ObservableCollection<object>(AvailableStorageLocations.Where(c => selectedLocations.Any(x => x.FacilityNo == c.FacilityNo)));
                    }

                }
                else if (response.Message != null)
                {
                    Message = response.Message;
                }
            }
            catch (Exception e)
            {
                Message = new Msg(eMsgLevel.Exception, e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void SaveSortStorageLocations()
        {
            try
            {
                Facility[] selectedItems = SelectedStorageLocations.OfType<Facility>().ToArray();
                string json = JsonConvert.SerializeObject(selectedItems);
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SortStorageLocations.json");
                File.WriteAllText(fileName, json);
            }
            catch (Exception e)
            {
                Message = new Msg(eMsgLevel.Exception, e.Message);
            }
        }

        public static Facility[] LoadSortStorageLocations()
        {
            try
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SortStorageLocations.json");
                if (!File.Exists(fileName))
                    return null;

                string json = File.ReadAllText(fileName);

                if (string.IsNullOrEmpty(json))
                    return null;

                return JsonConvert.DeserializeObject<Facility[]>(json);
            }
            catch
            {
                return null;
            }
        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }
    }
}
