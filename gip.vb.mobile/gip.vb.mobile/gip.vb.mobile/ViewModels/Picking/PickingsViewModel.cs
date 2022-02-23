 using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using gip.mes.webservices;
using System.Collections.Generic;
using gip.mes.facility;
using gip.core.datamodel;
using System.Linq;

namespace gip.vb.mobile.ViewModels
{
    public class PickingsViewModel : BaseViewModel
    {
        #region c'tors

        public PickingsViewModel()
        {
            Pickings = new ObservableCollection<Picking>();
            PickingsByMaterial = new ObservableCollection<PickingMaterial>();
            LoadPickingsCommand = new Command(async () => await ExecuteLoadPickingsCommand());
            FinishOrdersCommand = new Command(async () => await ExecuteFinishOrders());
        }

        #endregion

        #region Properties

        public ObservableCollection<Picking> Pickings { get; set; }
        public Command LoadPickingsCommand { get; set; }
        public PostingOverview Bookings {get;set;}
        public ObservableCollection<PickingMaterial> PickingsByMaterial { get; set; }

        private PickingViewModel _PickingFilter;
        public PickingViewModel PickingFilter 
        {
            get => _PickingFilter;
            set
            {
                SetProperty(ref _PickingFilter, value);
                if (_PickingFilter != null && _PickingFilter.IsGroupedByMaterial)
                {
                    if (_PickingFilter.SelectedPickingType == null)
                    {
                        ShowDialog(new Msg(eMsgLevel.Warning, Strings.AppStrings.PickingByMaterialWarning_Text));
                    }
                }
            }
        }

        #endregion

        #region Methods

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

                await Task.Delay(100); // hack for ListView - because IsRefreshing won't stop

                var response = await _WebService.SearchPickingsAsync(pickingType, fromFacility, toFacility, fromDate, toDate);
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    foreach (var item in response.Data)
                    {
                        if (PickingFilter != null && PickingFilter.IsGroupedByMaterial)
                        {
                            foreach (PickingPos pickingPos in item.PickingPos_Picking)
                            {
                                pickingPos.Picking = item;
                                pickingPos.CalculateDefaultPostingQuantity();
                            }
                        }

                        Pickings.Add(item);
                    }

                    if (PickingFilter != null && PickingFilter.IsGroupedByMaterial)
                    {
                        GroupByMaterial();
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
                OnExecutedLoadPickings();
            }
        }

        public virtual void OnExecutedLoadPickings()
        {

        }

        private PickingMaterial _SelectedPickingMaterial;
        public PickingMaterial SelectedPickingMaterial
        {
            get => _SelectedPickingMaterial;
            set
            {
                SetProperty<PickingMaterial>(ref _SelectedPickingMaterial, value);
            }
        }

        public void GroupByMaterial()
        {
            if (PickingsByMaterial != null)
                PickingsByMaterial.Clear();

            if (Pickings != null)
            {
                var groupedItems = Pickings.SelectMany(c => c.PickingPos_Picking).GroupBy(c => c.Material.MaterialID)
                                                                                 .Select(p => new PickingMaterial() { Material = p.FirstOrDefault()?.Material, 
                                                                                                                        PickingItems = p, 
                                                                                                                        TotalQuantity = p.Sum(c => c.TargetQuantity),
                                                                                                                        ActualQuantity = p.Sum(c => c.ActualQuantity),
                                                                                                                        MDUnit = p.FirstOrDefault()?.MDUnit });

                foreach (PickingMaterial pm in groupedItems)
                {
                    PickingsByMaterial.Add(pm);
                }
            }
            else
            {
                 PickingsByMaterial = new ObservableCollection<PickingMaterial>();
            }
        }

        public Command FinishOrdersCommand { get; set; }
        public async Task<bool> ExecuteFinishOrders(bool skipCheck = false)
        {
            MsgWithDetails result = null;
            if (IsBusy)
                return false;

            if (Pickings == null || !Pickings.Any())
                return false;

            IsBusy = true;

            try
            {
                BarcodeSequence sequence = new BarcodeSequence();

                foreach (Picking picking in Pickings)
                {
                    sequence.AddSequence(new BarcodeEntity() { Picking = new Picking() { PickingID = picking.PickingID } });
                }

                if (!sequence.Sequence.Any())
                    return true;

                var response = await _WebService.FinishPickingOrdersByMaterialAsync(sequence);
                if (response.Suceeded)
                {
                    result = response.Data;
                    if (result != null)
                    {
                        ShowDialog(new Msg(eMsgLevel.Error, result.DetailsAsText));
                        return false;
                    }
                    else
                    {
                        Message = new Msg(eMsgLevel.Info, Strings.AppStrings.OrderFinished_Text);
                    }
                }
                else if (response.Message != null)
                {
                    Message = response.Message;
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
            return true;
        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }

        #endregion
    }
}