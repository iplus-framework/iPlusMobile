using gip.core.autocomponent;
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
            ActivateDeactivateQuantCommnd = new Command(async () => await ExecuteActivateDeactivateQuant());
            ActivationQuants = new ObservableCollection<FacilityCharge>();
            BarcodeScanModel = new BarcodeScanModelBase();
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

        public BarcodeScanModelBase BarcodeScanModel
        {
            get;
            private set;
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
                                                                     MDUnit = p.FirstOrDefault()?.MDUnit,
                                                                     StockQuantity = p.Sum(x => x.TargetQuantity)
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

        public Command ActivateDeactivateQuantCommnd
        {
            get;
            set;
        }

        public async Task ExecuteActivateDeactivateQuant()
        {
            BarcodeSequence sequence = BarcodeScanModel.Item;
            if (sequence == null)
            {
                Message = new Msg(eMsgLevel.Error, Strings.AppStrings.ScanFCActivate_Text);
                return;
            }

            FacilityCharge fc = sequence.LastAddedSequence?.FacilityCharge;
            if (fc == null)
            {
                Message = new Msg(eMsgLevel.Error, Strings.AppStrings.ScanFCToActivate_Text);
                return;
            }

            var itemToActivateDeactivate = ActivationQuants.FirstOrDefault(c => c.Material != null && c.Material.MaterialID == fc.Material.MaterialID);
            if (itemToActivateDeactivate == null)
            {
                Message = new Msg(eMsgLevel.Error, Strings.AppStrings.ScannedFCIsNotReq_Text);
                return;
            }

            bool isdeactivation = itemToActivateDeactivate.FacilityChargeID != Guid.Empty && itemToActivateDeactivate.FacilityChargeID == fc.FacilityChargeID;

            FacilityChargeParamItem actItem = new FacilityChargeParamItem()
            {
                FacilityChargeID = fc.FacilityChargeID,
                Material = fc.Material,
                ParamID = PickingsWorkplaceModel.RegisteredWorkplace.ACClassID
            };

            IsBusy = true;

            try
            {

                WSResponse<bool> response = null;

                if (!isdeactivation)
                {
                    response = await _WebService.ActivateFacilityChargeAsync(actItem);
                }
                else
                {
                    response = await _WebService.DeactivateFacilityChargeAsync(actItem);
                }
                if (response.Suceeded)
                {
                    if (response.Message != null)
                    {
                        Message = response.Message;
                    }

                    if (response.Data)
                    {
                        if (!isdeactivation)
                        {
                            Message = new Msg(eMsgLevel.Info, Strings.AppStrings.ActSuccessful_Text);

                            var temp = ActivationQuants?.FirstOrDefault();
                            if (temp != null && temp != itemToActivateDeactivate && temp.FacilityChargeID != Guid.Empty)
                            {
                                ActivationQuants.Remove(temp);
                                ActivationQuants.Add(temp);
                            }

                            ActivationQuants.Remove(itemToActivateDeactivate);
                            ActivationQuants.Insert(0, fc);
                        }
                        else
                        {
                            Message = new Msg(eMsgLevel.Info, Strings.AppStrings.DeactSuccessful_Text);
                            fc.FacilityChargeID = Guid.Empty;
                            fc.StockQuantity = 0.0;
                            fc.Facility = null;
                            fc.FacilityLot = null;
                            ActivationQuants.Remove(itemToActivateDeactivate);
                            ActivationQuants.Insert(0,fc);
                        }
                    }
                    else
                    {
                        Message = new Msg(eMsgLevel.Info, Strings.AppStrings.ActIsNotSuccessful_Text);
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

        public override void DialogResponse(Global.MsgResult result, string enteredValue = null)
        {
            
        }
    }
}
