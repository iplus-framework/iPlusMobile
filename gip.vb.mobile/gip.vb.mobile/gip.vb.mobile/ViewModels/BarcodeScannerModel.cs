using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static gip.mes.datamodel.BarcodeSequenceBase;

namespace gip.vb.mobile.ViewModels
{
    public class BarcodeScannerModel : BaseViewModel
    {

        #region ctor's
        public BarcodeScannerModel(BarcodeIssuerEnum? barcodeIssuer)
        {
            LoadBarcodeEntityCommand = new Command(async () => await ExecuteLoadBarcodeEntityCommand());
            LoadInvokeBarcodeSequence = new Command(async () => await ExecuteInvokeBarcodeSequence());

            BarcodeIssuer = barcodeIssuer;
            if (BarcodeIssuer != null)
                BarcodeSequence = new BarcodeSequence() { BarcodeIssuer = barcodeIssuer.Value };
        }

        #endregion


        #region Impl. BaseViewModel
        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {

        }

        #endregion

        #region Commands

        public Command LoadBarcodeEntityCommand { get; set; }
        public Command LoadInvokeBarcodeSequence{ get; set; }

        #endregion

        #region Parameters

        public BarcodeIssuerEnum? BarcodeIssuer { get; private set; }

        private bool _ZXingIsScanning;
        public bool ZXingIsScanning
        {
            get
            {
                return _ZXingIsScanning;
            }
            set
            {
                SetProperty(ref _ZXingIsScanning, value);
                OnPropertyChanged();
            }
        }

        private string _CurrentBarcode;
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


        private BarcodeSequence _BarcodeSequence;
        public BarcodeSequence BarcodeSequence
        {
            get
            {
                return _BarcodeSequence;
            }
            set
            {
                SetProperty(ref _BarcodeSequence, value);
            }
        }

        #endregion

        #region Tasks
        public async Task ExecuteLoadBarcodeEntityCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetBarcodeEntityAsync(this.CurrentBarcode);
                this.WSResponse = response;
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

        public async Task ExecuteInvokeBarcodeSequence()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.InvokeBarcodeSequenceAsync(BarcodeSequence);
                this.WSResponse = response;
                if (response.Suceeded)
                    BarcodeSequence = response.Data;
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

        #endregion
    }
}
