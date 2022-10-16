using gip.core.datamodel;
using gip.mes.webservices;
using gip.vbm.mobile.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.vbm.mobile.Views;
using static gip.mes.datamodel.BarcodeSequenceBase;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.ViewModels
{
    public class BarcodeScanInventoryModel : BarcodeScanModelBase
    {
        #region ctor's
        public BarcodeScanInventoryModel(BSOPageBase parent) : base()
        {
            BarcodeIssuer = BarcodeIssuerEnum.Inventory;
            ParentBSO = parent;
        }

        #endregion

        public BSOPageBase ParentBSO
        {
            get;
            private set;
        }

        #region Tasks

        public override async Task<bool> ExecuteCustomBarcodeCommand()
        {
            if (ParentBSO is BSOInventoryMode)
                return await ExecuteDecodeEntityCommand();
            else
                return await ExecuteInvokeBarcodeSequenceCommand();
        }

        public override async Task<bool> ExecuteInvokeBarcodeSequenceCommand()
        {
            bool success = false;
            if (IsBusy)
                return false;

            IsBusy = true;

            try
            {
                if (ExchangedBarcodeSeq.State == ActionState.ScanAgain)
                {
                    if (ExchangedBarcodeSeq.Sequence == null || !ExchangedBarcodeSeq.Sequence.Any(c => c.Barcode.ToLower() == CurrentBarcode.Trim().ToLower()))
                    {
                        ExchangedBarcodeSeq.BarcodeIssuer = BarcodeIssuer.HasValue ? BarcodeIssuer.Value : BarcodeIssuerEnum.Inventory;

                        IsListVisible = false;
                        List<object> barcodeEntryTemp = new List<object>();
                        ExchangedBarcodeSeq.CurrentBarcode = CurrentBarcode;
                        ExchangedBarcodeSeq.Message = null;
                        var response = await _WebService.InvokeBarcodeSequenceAsync(ExchangedBarcodeSeq);
                        WSResponse = response;
                        success = response.Suceeded;
                        if (response.Suceeded && response.Data != null)
                        {
                            ExchangedBarcodeSeq = response.Data;
                            DecodedEntitiesList = response.Data.Sequence.Select(c => c.ValidEntity).DefaultIfEmpty().ToList();
                            Message = response.Data.Message; // now use sequence related message
                            IsListVisible = true;
                        }
                    }
                    else
                        Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = AppStrings.BarcodeSequenceAlreadyHaveCode_Text };

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
            return success;
        }

        #endregion

        #region Methods

        public override void Clear()
        {
            base.Clear();

            CurrentBarcode = null;
            //Item = null;
            ZXingIsScanning = false;
            IsListVisible = false;
            DecodedEntitiesList = null;
            if (ExchangedBarcodeSeq != null)
            {
                ExchangedBarcodeSeq.Sequence.Clear();
                ExchangedBarcodeSeq.State = ActionState.ScanAgain;
            }
            Message = null;
        }

        public void AddToCurrentBarcodeEntity(object validEntity)
        {
            List<object> entityList = new List<object>();
            if (DecodedEntitiesList != null && DecodedEntitiesList.Any())
                foreach (var item in DecodedEntitiesList)
                    entityList.Add(item);
            entityList.Add(validEntity);
            DecodedEntitiesList = entityList;
        }


        /// <summary>
        /// Add usualy predefined items in sequence
        /// For example: Faciltiy if user work on this faciltiy
        /// Predefined faciltiy can be removed later of 
        /// user scan another faciltiy
        /// </summary>
        /// <param name="barcodeEntity"></param>
        public void AddBarcodeEntity(BarcodeEntity barcodeEntity)
        {
            AddToCurrentBarcodeEntity(barcodeEntity.ValidEntity);
            ExchangedBarcodeSeq.AddSequence(barcodeEntity);
        }
        #endregion
    }
}
