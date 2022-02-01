using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class PickingWorkplaceActViewModel : BaseViewModel
    {
        public PickingWorkplaceActViewModel() : base ()
        {
            BarcodeScanModel = new BarcodeScanModelBase(true);
            ActivateFacilityChargeCommand = new Command(async () => await ExecuteActivateFacilityCharge());
        }

        private FacilityCharge _Item;
        public FacilityCharge Item
        {
            get => _Item;
            set
            {
                SetProperty(ref _Item, value);
            }
        }

        public ACClass Workplace
        {
            get;
            set;
        }

        public BarcodeScanModelBase BarcodeScanModel
        {
            get;
            private set;
        }

        public Command ActivateFacilityChargeCommand
        {
            get;
            set;
        }

        public async Task ExecuteActivateFacilityCharge()
        {
            if (IsBusy || BarcodeScanModel == null || Item == null)
                return;

            if (Workplace == null)
            {
                Message = new Msg(eMsgLevel.Error, "Workplace is empty");
                return;
            }

            try
            {
                BarcodeSequence sequence = BarcodeScanModel.Item;
                if (sequence == null)
                {
                    Message = new Msg(eMsgLevel.Error, "Please, scan facility charge first then try activate");
                    return;
                }

                FacilityCharge fc = sequence.LastAddedSequence?.FacilityCharge;
                if (fc == null)
                {
                    Message = new Msg(eMsgLevel.Error, "Please, scan facility charge first then try activate");
                    return;
                }

                if (fc.Material.MaterialID != Item.Material.MaterialID)
                {
                    Message = new Msg(eMsgLevel.Error, "Material mismatch, please scan right material.");
                    return;
                }

                FacilityChargeActivationItem actItem = new FacilityChargeActivationItem()
                {
                    FacilityChargeID = fc.FacilityChargeID,
                    Material = fc.Material,
                    WorkplaceID = Workplace.ACClassID
                };

                IsBusy = true;

                WSResponse<bool> response = await _WebService.ActivateFacilityChargeAsync(actItem);
                if (response.Suceeded)
                {
                    if (response.Message != null)
                    {
                        Message = response.Message;
                    }

                    if (response.Data)
                    {
                        Message = new Msg(eMsgLevel.Info, "Activation is successful");
                        Item = fc;
                    }
                    else
                    {
                        Message = new Msg(eMsgLevel.Info, "Activation is not successful");
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
