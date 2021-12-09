using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class PickingByMaterialViewModel : BaseViewModel
    {

        public PickingByMaterialViewModel()
        {
            
        }

        private PickingMaterial _Item;

        public PickingMaterial Item
        {
            get => _Item;
            set
            {
                SetProperty<PickingMaterial>(ref _Item, value);
                if (_Item != null)
                {
                    Title = _Item.Material.MaterialName1;
                }
            }
        }

        private PickingPos _SelectedPickingPos;
        public PickingPos SelectedPickingPos
        {
            get => _SelectedPickingPos;
            set
            {
                SetProperty<PickingPos>(ref _SelectedPickingPos, value);
            }
        }

        public string _CurrentBarcode;
        public string CurrentBarcode
        {
            get
            {
                return _CurrentBarcode;
            }
            set
            {
                SetProperty(ref _CurrentBarcode, value);
            }
        }

        public BarcodeEntity _WSBarcodeEntityResult;
        public BarcodeEntity WSBarcodeEntityResult
        {
            get
            {
                return _WSBarcodeEntityResult;
            }
            set
            {
                SetProperty(ref _WSBarcodeEntityResult, value);
            }
        }

        public List<object> _CurrentBarcodeEntity;
        public List<object> CurrentBarcodeEntity
        {
            get
            {
                return _CurrentBarcodeEntity;
            }
            set
            {
                SetProperty(ref _CurrentBarcodeEntity, value);
            }
        }

        public Command LoadBarcodeEntityCommand { get; set; }
        public async Task ExecuteLoadBarcodeEntityCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetBarcodeEntityAsync(this.CurrentBarcode);
                this.WSResponse = response;
                this.WSBarcodeEntityResult = response.Data;
                if (response.Suceeded)
                    CurrentBarcodeEntity = new List<object> { response.Data.ValidEntity };
                else
                    CurrentBarcodeEntity = new List<object>();
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



        public void ChangePostingQuantity()
        {
            Msg msg = new Msg(eMsgLevel.QuestionPrompt, "Change the posting quantity: ");
            ShowDialog(msg, "", Keyboard.Numeric, SelectedPickingPos?.PostingQuantity.ToString(), 1);
        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            if (DialogOptions.RequestID == 1)
            {
                if (result == Global.MsgResult.OK)
                {
                    double postingQuantity = 0;
                    if (double.TryParse(entredValue, out postingQuantity) && SelectedPickingPos != null)
                    {
                        SelectedPickingPos.PostingQuantity = postingQuantity;
                    }
                }
            }
        }
    }
}
