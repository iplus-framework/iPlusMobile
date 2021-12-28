using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using gip.mes.webservices;
using gip.core.autocomponent;
using gip.vb.mobile.Views;
using gip.core.datamodel;
using System.Linq;

namespace gip.vb.mobile.ViewModels
{
    public class MaterialOverviewModel : BaseViewModel
    {
        public MaterialOverviewModel(Material item = null)
        {
            Item = item;
            LoadMaterialSumOverviewCommand = new Command(async () => await ExecuteLoadMaterialSumOverviewCommand());
            FilterByStorageLocationAndSortCommand = new Command((object param) => ExecuteFilterByStorageLocationAndSortCommand(param));
        }

        #region Properties
        private Material _Item;
        public Material Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetProperty(ref _Item, value);
                RebuildTitle();
            }
        }

        public MaterialSumOverview _Overview;
        public MaterialSumOverview Overview
        {
            get
            {
                return _Overview;
            }
            set
            {
                SelectedFacilityCharge = null;
                SetProperty(ref _Overview, value);
                RebuildTitle();
            }
        }


        private FacilityCharge _SelectedFacilityCharge;
        public FacilityCharge SelectedFacilityCharge
        {
            get
            {
                return _SelectedFacilityCharge;
            }
            set
            {
                SetProperty(ref _SelectedFacilityCharge, value);
            }
        }
        #endregion

        #region Methods
        void RebuildTitle()
        {
            if (Item != null)
            {
                if (Overview != null && Overview.MaterialStock != null)
                    Title = String.Format("{0}: {1:0.0}", Item.MaterialName1, Overview.MaterialStock.StockQuantity);
                else
                    Title = Item.MaterialName1;
            }
            else
                Title = "MaterialOverview";
        }

        public Command LoadMaterialSumOverviewCommand { get; set; }
        public async Task ExecuteLoadMaterialSumOverviewCommand()
        {
            if (IsBusy || Item == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetMaterialSumAsync(Item.MaterialID.ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                    Overview = response.Data;
                else
                    Overview = null;
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

        public Command FilterByStorageLocationAndSortCommand { get; set; }
        public void ExecuteFilterByStorageLocationAndSortCommand(object param)
        {
            FacilityCharge fc = param as FacilityCharge;
            if (fc == null || fc.Facility == null)
            {
                //TODO: error
                return;
            }

            var temp = Overview.FacilityCharges.Where(c => c.Facility != null && c.Facility.ParentFacilityID == fc.Facility.ParentFacilityID).OrderBy(x => x.ExpirationDate).ToArray();
            Overview.FacilityCharges = temp;
            Overview.OnFacilityChargesChanged();
        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }


        #endregion

    }
}