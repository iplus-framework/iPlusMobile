using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class FacilityChargeMakerViewModel : BaseViewModel
    {
        public FacilityChargeMakerViewModel(Material material) : base()
        {
            SearchMaterialCommand = new Command(async () => await ExecuteSearchMaterial());

            Item = new FacilityCharge();
            Item.Material = material;
        }



        public Command CreateNewFacilityChargeCommand { get; set; }
        public async Task ExecuteCreateNewFacilityCharge()
        {
            if (IsBusy || Item == null)
                return;

            try
            {
                IsBusy = true;



            }
            catch (Exception e)
            {

            }
            finally
            {
                IsBusy = false;
            }
        }

        public Command SearchMaterialCommand { get; set; }
        public async Task ExecuteSearchMaterial()
        {
            if (IsBusy || string.IsNullOrEmpty(MaterialSearchText))
                return;

            try
            {
                IsBusy = true;

                var response = await _WebService.SearchMaterialAsync(MaterialSearchText);
                if (response.Suceeded)
                {
                    MaterialList = response.Data;
                }
                else if (response.Message != null)
                {
                    Message = response.Message;
                }

                IsSelectMaterialVisible = true;
            }
            catch (Exception e)
            {

            }
            finally
            {
                IsBusy = false;
            }
        }

        public FacilityCharge _Item;
        public FacilityCharge Item
        {
            get => _Item;
            set 
            {
                SetProperty(ref _Item, value);
            }
        }

        private string _MaterialSearchText;
        public string MaterialSearchText
        {
            get => _MaterialSearchText;
            set
            {
                SetProperty(ref _MaterialSearchText, value);
            }
        }

        private Material _SelectedMaterial;
        public Material SelectedMaterial
        {
            get => _SelectedMaterial;
            set 
            {
                SetProperty(ref _SelectedMaterial, value);
                IsSelectMaterialVisible = false;
            }
        }

        private List<Material> _MaterialList;
        public List<Material> MaterialList
        {
            get => _MaterialList;
            set
            {
                SetProperty(ref _MaterialList, value);
            }
        }

        private bool _IsSelectMaterialVisible;
        public bool IsSelectMaterialVisible
        {
            get => _IsSelectMaterialVisible;
            set => SetProperty(ref _IsSelectMaterialVisible, value);
        }


        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            
        }
    }
}
