using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class PickingsWorkplaceViewModel : PickingsViewModel
    {
        public PickingsWorkplaceViewModel() : base ()
        {
            FinishAndBookOrdersCommand = new Command(async () => await ExecuteFinishAndBookOrders());
        }

        private List<Picking> _PickingItems;
        public List<Picking> PickingItems
        {
            get => _PickingItems;
            set
            {
                SetProperty(ref _PickingItems, value);
            }
        }

        private List<string> _GroupItems;
        public List<string> GroupItems
        {
            get => _GroupItems;
            set
            {
                SetProperty(ref _GroupItems, value);
            }
        }

        public ACClass RegisteredWorkplace
        {
            get;
            set;
        }

        private string _SelectedGroupItem;
        public string SelectedGroupItem
        {
            get => _SelectedGroupItem;
            set
            {
                SetProperty(ref _SelectedGroupItem, value);
                if (_SelectedGroupItem != null)
                {
                    PickingItems = Pickings.Where(c => c.GroupItem == _SelectedGroupItem && c.PickingType.PickingType == mes.datamodel.GlobalApp.PickingType.Issue).ToList();
                }
                else
                {
                    PickingItems = Pickings.Where(c => c.PickingType.PickingType == mes.datamodel.GlobalApp.PickingType.Issue).OrderBy(x => x.GroupItem).ToList();
                }
            }
        }

        public override void OnExecutedLoadPickings()
        {
            IEnumerable<string> groups = Pickings?.Where(x => x.GroupItem != null).Select(c => c.GroupItem).Distinct();
            if (groups != null && groups.Any())
            {
                GroupItems = groups.ToList();
            }

            PickingItems = Pickings.Where(c => c.PickingType.PickingType == mes.datamodel.GlobalApp.PickingType.Issue).OrderBy(x => x.GroupItem).ToList();
        }

        public Command FinishAndBookOrdersCommand
        {
            get;
            set;
        }

        public async Task ExecuteFinishAndBookOrders()
        {
            if (IsBusy)
                return;

            try
            {
                var requiredQuants = PickingItems.SelectMany(c => c.PickingPos_Picking)
                                                 .GroupBy(c => c.Material.MaterialID)
                                                 .Select(p => new FacilityCharge()
                                                 {
                                                    Material = p.FirstOrDefault()?.Material,
                                                    MDUnit = p.FirstOrDefault()?.MDUnit,
                                                    StockQuantity = p.Sum(x => x.TargetQuantity)
                                                 }).ToList();

                IsBusy = true;

                var activatedQuants = await _WebService.GetRegisteredFacilityChargesAsync(RegisteredWorkplace.ACClassID.ToString());

                if (activatedQuants.Suceeded)
                {
                    if (activatedQuants.Message != null)
                    {
                        Message = activatedQuants.Message;
                        return;
                    }

                    List<FacilityCharge> actQuants = activatedQuants.Data;

                    if (actQuants != null)
                    {
                        foreach (FacilityCharge fc in actQuants)
                        {
                            var matchQuant = requiredQuants.FirstOrDefault(c => c.Material != null && c.Material.MaterialID == fc.Material.MaterialID);

                            //if (matchQuant.StockQuantity >= fc.StockQuantity)
                            //{
                            //    Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Error, "");
                            //    return;
                            //}

                            requiredQuants.Remove(matchQuant);
                        }
                    }

                    if (requiredQuants.Any())
                    {
                        Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Error, "All required quants are not scanned. Please scan all required quants.");
                        return;
                    }

                    List<Msg> errors = new List<Msg>();

                    foreach (Picking pickingItem in PickingItems)
                    {
                        ACMethodBookingList bookingList = new ACMethodBookingList();

                        foreach(PickingPos pickingPos in pickingItem.PickingPos_Picking)
                        {
                            if (pickingPos.CompleteFactor >= 99)
                            {
                                continue;
                            }

                            FacilityCharge fc = actQuants.FirstOrDefault(c => c.Material.MaterialID == pickingPos.Material.MaterialID);
                            
                            if (fc == null)
                            {
                                return;
                            }

                            ACMethodBooking aCMethodBooking = new ACMethodBooking();
                            aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_Relocation_FacilityCharge_Facility;


                            if (pickingItem.PickingType.MDKey == mes.datamodel.GlobalApp.PickingType.Issue.ToString())
                            {
                                aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_PickingOutward;
                            }
                            else
                                continue;

                            pickingPos.CalculateDefaultPostingQuantity();

                            aCMethodBooking.PickingPosID = pickingPos.PickingPosID;
                            aCMethodBooking.OutwardQuantity = pickingPos.PostingQuantity;

                            aCMethodBooking.OutwardFacilityID = fc.Facility.FacilityID;
                            aCMethodBooking.OutwardFacilityChargeID = fc.FacilityChargeID;
                            
                            aCMethodBooking.InwardQuantity = pickingPos.PostingQuantity;
                            bookingList.Add(aCMethodBooking);
                            
                        }

                        var bookResponse = await _WebService.BookFacilitiesAsync(bookingList);
                        if (bookResponse.Suceeded)
                        {
                            if (bookResponse.Message != null)
                            {
                                errors.Add(bookResponse.Message);
                            }

                            if (bookResponse.Data != null)
                            {
                                errors.Add(bookResponse.Message); //TODO
                            }
                        }
                        else if (bookResponse.Message != null)
                        {
                            errors.Add(bookResponse.Message);
                        }
                    }
                }
                else if (activatedQuants.Message != null)
                {
                    Message = activatedQuants.Message;
                }
                else
                {
                    Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Error, "Problem with retriving activated quants.");
                }
            }
            catch (Exception e)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
