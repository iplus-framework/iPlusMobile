using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.webservices;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class FacilityBookingHistoryModel : BaseViewModel
    {
        public FacilityBookingHistoryModel(object item = null)
        {
            Item = item;
            ReadPostingsCommand = new Command(async () => await ExecuteReadPostingsCommand());
        }

        #region Properties
        private object _Item;
        public object Item
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

        public Facility ItemAsFacility
        {
            get
            {
                return Item as Facility;
            }
        }

        public Material ItemAsMaterial
        {
            get
            {
                return Item as Material;
            }
        }

        public FacilityLot ItemAsFacilityLot
        {
            get
            {
                return Item as FacilityLot;
            }
        }

        public FacilityCharge ItemAsFacilityCharge
        {
            get
            {
                return Item as FacilityCharge;
            }
        }

        private PostingOverview _Overview;
        public PostingOverview Overview
        {
            get
            {
                return _Overview;
            }
            set
            {
                SetProperty(ref _Overview, value);
            }
        }

        private DateTime _FilterFrom = DateTime.Now.AddDays(-1);
        public DateTime FilterFrom
        {
            get
            {
                return _FilterFrom;
            }
            set
            {
                SetProperty(ref _FilterFrom, value);
                OnPropertyChanged("FilterDateFrom");
                OnPropertyChanged("FilteTimeFrom");
            }
        }

        public DateTime FilterDateFrom
        {
            get
            {
                DateTime viewDateTime;
                try
                {
                    viewDateTime = FilterFrom;
                }
                catch (Exception)
                {
                    viewDateTime = DateTime.MinValue;
                }
                return viewDateTime;
            }
            set
            {
                DateTime newDateTime = new DateTime(value.Year, value.Month, value.Day, FilterFrom.Hour, FilterFrom.Minute, FilterFrom.Second);
                FilterFrom = newDateTime;
            }
        }

        public TimeSpan FilteTimeFrom
        {
            get
            {
                return FilterFrom.TimeOfDay;
            }
            set
            {
                DateTime newDateTime = new DateTime(FilterFrom.Year, FilterFrom.Month, FilterFrom.Day, value.Hours, value.Minutes, value.Seconds);
                FilterFrom = newDateTime;
            }
        }


        private DateTime _FilterTo = DateTime.Now;
        public DateTime FilterTo
        {
            get
            {
                return _FilterTo;
            }
            set
            {
                SetProperty(ref _FilterTo, value);
                OnPropertyChanged("FilterDateTo");
                OnPropertyChanged("FilteTimeTo");
            }
        }

        public DateTime FilterDateTo
        {
            get
            {
                DateTime viewDateTime;
                try
                {
                    viewDateTime = FilterTo;
                }
                catch (Exception)
                {
                    viewDateTime = DateTime.MinValue;
                }
                return viewDateTime;
            }
            set
            {
                DateTime newDateTime = new DateTime(value.Year, value.Month, value.Day, FilterTo.Hour, FilterTo.Minute, FilterTo.Second);
                FilterTo = newDateTime;
            }
        }

        public TimeSpan FilteTimeTo
        {
            get
            {
                return FilterTo.TimeOfDay;
            }
            set
            {
                DateTime newDateTime = new DateTime(FilterTo.Year, FilterTo.Month, FilterTo.Day, value.Hours, value.Minutes, value.Seconds);
                FilterTo = newDateTime;
            }
        }



        #endregion

        #region Methods
        void RebuildTitle()
        {
            if (ItemAsMaterial != null)
                Title = ItemAsMaterial.MaterialName1;
            else if (ItemAsFacility != null)
                Title = ItemAsFacility.FacilityName;
            else if (ItemAsFacilityLot != null)
                Title = ItemAsFacilityLot.LotNo;
            else if (ItemAsFacilityCharge != null)
                Title = ItemAsFacilityCharge.Material.MaterialName1 + "/" + ItemAsFacilityCharge.FacilityLot != null ? ItemAsFacilityCharge.FacilityLot.LotNo : "";
            else
                Title = "FacilityBookingHistoryModel";
        }

        public Command ReadPostingsCommand { get; set; }
        public async Task ExecuteReadPostingsCommand()
        {
            if (IsBusy || Item == null)
                return;

            IsBusy = true;

            try
            {
                WSResponse<PostingOverview> response = null;
                if (ItemAsMaterial != null)
                    response = await _WebService.GetMaterialBookingsAsync(ItemAsMaterial.MaterialID.ToString(),
                        FilterFrom.ToString("o", CultureInfo.InvariantCulture),
                        FilterTo.ToString("o", CultureInfo.InvariantCulture));
                else if (ItemAsFacilityLot != null)
                    response = await _WebService.GetFacilityLotBookingsAsync(ItemAsFacilityLot.FacilityLotID.ToString(),
                        FilterFrom.ToString("o", CultureInfo.InvariantCulture),
                        FilterTo.ToString("o", CultureInfo.InvariantCulture));
                else if (ItemAsFacilityCharge != null)
                    response = await _WebService.GetFacilityChargeBookingsAsync(ItemAsFacilityCharge.FacilityChargeID.ToString(),
                        FilterFrom.ToString("o", CultureInfo.InvariantCulture),
                        FilterTo.ToString("o", CultureInfo.InvariantCulture));
                else if (ItemAsFacility != null)
                {
                    if (ItemAsFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageLocation)
                    {
                        response = await _WebService.GetFacilityLocationBookingsAsync(ItemAsFacility.FacilityID.ToString(),
                            FilterFrom.ToString("o", CultureInfo.InvariantCulture),
                            FilterTo.ToString("o", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        response = await _WebService.GetFacilityBookingsAsync(ItemAsFacility.FacilityID.ToString(),
                            FilterFrom.ToString("o", CultureInfo.InvariantCulture),
                            FilterTo.ToString("o", CultureInfo.InvariantCulture));
                    }
                }

                this.WSResponse = response;
                if (response != null)
                {
                    if (response.Suceeded)
                        Overview = response.Data;
                    else
                        Overview = new PostingOverview();
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

        #endregion
    }
}
