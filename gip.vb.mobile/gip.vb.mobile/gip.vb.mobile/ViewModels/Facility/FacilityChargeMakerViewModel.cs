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
            Item = new FacilityCharge();
            Item.Material = material;
        }

        public Command GetFacilityLotsCommand { get; set; }
        public async Task ExecuteGetFacilityLots()
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

        public Command CreateNewFacilityLotCommand { get; set; }
        public async Task ExecuteCreateNewFacilityLot()
        {
            if (IsBusy || Item == null)
                return;

            try
            {
                IsBusy = true;


                WSResponse<FacilityLot> response = await _WebService.CreateFacilityLotAsync();
                WSResponse = response;

                if (response.Suceeded)
                {
                    if (response.Message != null)
                        Message = response.Message;

                    Item.FacilityLot = response.Data;
                }
                else 
                {
                    if (response.Message != null)
                        Message = response.Message;
                }

            }
            catch (Exception e)
            {

            }
            finally
            {
                IsBusy = false;
            }
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

        public FacilityCharge _Item;
        public FacilityCharge Item
        {
            get => _Item;
            set 
            {
                SetProperty(ref _Item, value);
            }
        }

        public void GenerateNewLot()
        {

        }


        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            
        }
    }
}
