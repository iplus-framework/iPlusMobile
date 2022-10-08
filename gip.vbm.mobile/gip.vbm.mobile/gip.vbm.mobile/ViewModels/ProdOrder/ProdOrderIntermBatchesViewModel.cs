﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.webservices;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.ViewModels
{
    public class ProdOrderIntermBatchesViewModel : BaseViewModel
    {
        public ProdOrderIntermBatchesViewModel(ProdOrderPartslistPos intermediatePos)
        {
            IntermediatePos = intermediatePos;
            ProdOrderIntermediateBatches = new ObservableCollection<ProdOrderPartslistPos>();
            LoadProdOrderIntermBatchesCommand = new Command(async () => await ExecuteLoadProdOrderIntermBatchesCommand());
            Title = string.Format("{0}, {1}", intermediatePos.ProdOrderPartslist.Partslist.PartslistNo, intermediatePos.Material.MaterialNo);
        }

        public ProdOrderPartslistPos IntermediatePos
        {
            get; set;
        }

        public ObservableCollection<ProdOrderPartslistPos> ProdOrderIntermediateBatches { get; set; }
        public Command LoadProdOrderIntermBatchesCommand
        {
            get; set;
        }

        public async Task ExecuteLoadProdOrderIntermBatchesCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                ProdOrderIntermediateBatches.Clear();
                var response = await _WebService.GetProdOrderIntermBatchesAsync(IntermediatePos?.ProdOrderPartslistPosID.ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    foreach (var item in response.Data)
                    {
                        ProdOrderIntermediateBatches.Add(item);
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
        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            
        }
    }
}
