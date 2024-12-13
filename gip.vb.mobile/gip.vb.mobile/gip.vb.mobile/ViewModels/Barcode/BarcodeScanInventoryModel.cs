// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.webservices;
using gip.vb.mobile.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.vb.mobile.Views;
using Xamarin.Forms;
using static gip.mes.datamodel.BarcodeSequenceBase;

namespace gip.vb.mobile.ViewModels
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

        public override async Task<bool> ExecuteInvokeBarcode()
        {
            if (ParentBSO is BSOInventoryMode)
                return await ExecuteGetBarcodeEntityCommand();
            else
                return await ExecuteInvokeBarcodeCommand();
        }

        public override async Task<bool> ExecuteInvokeBarcodeCommand()
        {
            bool success = false;
            if (IsBusy)
                return false;

            IsBusy = true;

            try
            {
                if (Item.State == ActionState.ScanAgain)
                {
                    if (Item.Sequence == null || !Item.Sequence.Any(c => c.Barcode.ToLower() == CurrentBarcode.Trim().ToLower()))
                    {
                        Item.BarcodeIssuer = BarcodeIssuer.HasValue ? BarcodeIssuer.Value : BarcodeIssuerEnum.Inventory;

                        IsListVisible = false;
                        List<object> barcodeEntryTemp = new List<object>();
                        Item.CurrentBarcode = CurrentBarcode;
                        Item.Message = null;
                        var response = await _WebService.InvokeBarcodeSequenceAsync(Item);
                        WSResponse = response;
                        success = response.Suceeded;
                        if (response.Suceeded && response.Data != null)
                        {
                            Item = response.Data;
                            BarcodeSequence = response.Data.Sequence.Select(c => c.ValidEntity).DefaultIfEmpty().ToList();
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
            BarcodeSequence = null;
            if (Item != null)
            {
                Item.Sequence.Clear();
                Item.State = ActionState.ScanAgain;
            }
            Message = null;
        }

        public void AddToCurrentBarcodeEntity(object validEntity)
        {
            List<object> entityList = new List<object>();
            if (BarcodeSequence != null && BarcodeSequence.Any())
                foreach (var item in BarcodeSequence)
                    entityList.Add(item);
            entityList.Add(validEntity);
            BarcodeSequence = entityList;
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
            Item.AddSequence(barcodeEntity);
        }
        #endregion
    }
}
