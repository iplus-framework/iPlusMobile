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

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }

        #endregion
    }
}
