using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using gip.mes.webservices;
using System.Collections.Generic;
using gip.mes.facility;
using gip.core.datamodel;

namespace gip.vb.mobile.ViewModels
{
    public class PickingsViewModel : BaseViewModel
    {
        public ObservableCollection<Picking> Pickings { get; set; }
        public Command LoadPickingsCommand { get; set; }
        public PostingOverview Bookings {get;set;}

        public PickingViewModel PickingFilter { get; set; }

        public PickingsViewModel()
        {
            Pickings = new ObservableCollection<Picking>();
            LoadPickingsCommand = new Command(async () => await ExecuteLoadPickingsCommand());

            //MessagingCenter.Subscribe<NewItemPage, Picking>(this, "AddItem", async (obj, item) =>
            //{
            //    var newItem = item as Picking;
            //    Items.Add(newItem);
            //    await _WebService.AddItemAsync(newItem);
            //});
        }

        public async Task ExecuteLoadPickingsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Pickings.Clear();

                string pickingType = "", fromFacility = "", toFacility = "", fromDate = "", toDate = "";
                if (PickingFilter != null)
                {
                    pickingType = PickingFilter.SelectedPickingType != null ? PickingFilter.SelectedPickingType.MDPickingTypeID.ToString() : "";

                    fromFacility = PickingFilter.SelectedStorageLocationFrom != null ? PickingFilter.SelectedStorageLocationFrom.FacilityID.ToString() : "";

                    toFacility = PickingFilter.SelectedStorageLocationTo != null ? PickingFilter.SelectedStorageLocationTo.FacilityID.ToString() : "";

                    fromDate = PickingFilter.DateFrom.ToString("o");

                    toDate = PickingFilter.DateTo.ToString("o");
                }

                var response = await _WebService.SearchPickingsAsync(pickingType, fromFacility, toFacility, fromDate, toDate);
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    foreach (var item in response.Data)
                    {
                        Pickings.Add(item);
                    }
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