using gip.core.datamodel;
using gip.core.webservices;
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
    public class PickingWorkplaceActItemsViewModel : BaseViewModel
    {
        public PickingWorkplaceActItemsViewModel()
        {
            GetActivatedQuantsCommand = new Command(async () => await ExecuteGetActivatedQuants());
            ActivationQuants = new ObservableCollection<FacilityCharge>();
        }


        private ObservableCollection<FacilityCharge> _ActivationQuants;
        public ObservableCollection<FacilityCharge> ActivationQuants
        {
            get => _ActivationQuants;
            set
            {
                SetProperty(ref _ActivationQuants, value);
            }
        }

        private PickingsWorkplaceViewModel _PickingsWorkplaceModel;
        public PickingsWorkplaceViewModel PickingsWorkplaceModel
        {
            get => _PickingsWorkplaceModel;
            set
            {
                SetProperty(ref _PickingsWorkplaceModel, value);
                GetActivatedQuantsCommand.Execute(null);
            }
        }

        public Command GetActivatedQuantsCommand
        {
            get;
            set;
        }

        public async Task ExecuteGetActivatedQuants()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (PickingsWorkplaceModel != null && PickingsWorkplaceModel.RegisteredWorkplace != null)
                {
                    var activatedQuants = await _WebService.GetRegisteredFacilityChargesAsync(PickingsWorkplaceModel.RegisteredWorkplace.ACClassID.ToString());
                    if (activatedQuants.Suceeded)
                    {
                        if (activatedQuants.Message != null)
                        {
                            Message = activatedQuants.Message;
                            return;
                        }

                        List<FacilityCharge> activatedQuantsList = activatedQuants.Data;

                        var quants = PickingsWorkplaceModel.PickingItems
                                                                 .SelectMany(c => c.PickingPos_Picking)
                                                                 .GroupBy(c => c.Material.MaterialID)
                                                                 .Select(p => new FacilityCharge()
                                                                 {
                                                                     Material = p.FirstOrDefault()?.Material,
                                                                     MDUnit = p.FirstOrDefault()?.MDUnit
                                                                 }).ToList();

                        if (activatedQuantsList != null)
                        {
                            foreach (FacilityCharge fc in activatedQuantsList)
                            {
                                FacilityCharge itemToReplace = quants.FirstOrDefault(c => c.Material.MaterialID == fc.Material.MaterialID);

                                if (itemToReplace != null)
                                {
                                    quants.Remove(itemToReplace);
                                }
                                quants.Add(fc);
                            }
                        }

                        ActivationQuants = new ObservableCollection<FacilityCharge>(quants);
                    }
                    else if (activatedQuants.Message != null)
                    {
                        Message = activatedQuants.Message;
                    }
                }
                else
                {
                    Message = new Msg(eMsgLevel.Error, "Workplace is not registered!");
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


        public override void DialogResponse(Global.MsgResult result, string enteredValue = null)
        {
            
        }
    }
}
