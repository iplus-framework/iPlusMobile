using System;
using System.Diagnostics;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.webservices;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class PickingDetailViewModel : BaseViewModel
    {
        public PickingDetailViewModel(Picking item = null)
        {
            Item = item;
            UpdatePickingCommand = new Command(async () => await UpdatePicking());
            FinishOrderCommand = new Command(async () => await FinishOrder());
        }

        #region Properties
        private Picking _Item;
        public Picking Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetProperty(ref _Item, value);
                OnPropertyChanged();
                RebuildTitle();
            }
        }

        public void RefreshItem()
        {            
        }
        #endregion

        #region Methods
        void RebuildTitle()
        {
            if (Item != null)
                Title = Item.PickingNo;
            else
                Title = "PickingDetailViewModel";
        }

        public Command UpdatePickingCommand { get; set; }
        public async Task<bool> UpdatePicking()
        {
            bool result = false;
            if (IsBusy)
                return result;

            IsBusy = true;

            try
            {
                var response = await _WebService.UpdatePickingAsync(Item);
                this.WSResponse = response;
                if (response.Suceeded)
                    result = response.Data;
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
            return result;
        }

        public Command FinishOrderCommand { get; set; }
        public async Task<bool> FinishOrder(bool skipCheck = false)
        {
            MsgWithDetails result = null;
            if (IsBusy)
                return false;

            IsBusy = true;

            try
            {
                core.autocomponent.WSResponse<MsgWithDetails> response = null;
                if (!skipCheck)
                {
                    response = await _WebService.FinishPickingOrderAsync(Item.PickingID);
                }
                else
                {
                    response = await _WebService.FinishPickingOrderWithoutCheckAsync(Item.PickingID);
                }

                this.WSResponse = response;
                if (response.Suceeded)
                {
                    result = response.Data;
                    if (result != null)
                    {
                        if (!skipCheck)
                        {
                            result.MessageLevel = eMsgLevel.Question;
                            result.MessageButton = eMsgButton.YesNo;
                            ShowDialog(result, requestID : 1);
                        }
                        else
                        {
                            ShowDialog(result);
                        }
                    }
                    else
                    {
                        Message = new Msg(eMsgLevel.Info, "Order is finished!");
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
            return true;
        }

        public override async void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            if (DialogOptions.RequestID == 1)
            {
                if (result == Global.MsgResult.Yes)
                {
                    await FinishOrder(true);
                }
            }
        }

        #endregion
    }
}
