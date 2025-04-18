// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
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
    public class ProdOrderPLIntermediatesViewModel : BaseViewModel
    {
        public ProdOrderPLIntermediatesViewModel(ProdOrderPartslist prodOrderPartslist)
        {
            ProdOrderPartslistItem = prodOrderPartslist;
            ProdOrderPLIntermediates = new ObservableCollection<ProdOrderPartslistPos>();
            LoadProdOrderPLIntermediatesCommand = new Command(async () => await ExecuteLoadProdOrderPLIntermediatesCommand());
        }

        public ProdOrderPartslist ProdOrderPartslistItem
        {
            get;
            set;
        }

        public ObservableCollection<ProdOrderPartslistPos> ProdOrderPLIntermediates { get; set; }
        public Command LoadProdOrderPLIntermediatesCommand { get; set; }

        public async Task ExecuteLoadProdOrderPLIntermediatesCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                ProdOrderPLIntermediates.Clear();
                var response = await _WebService.GetProdOrderPLIntermediatesAsync(ProdOrderPartslistItem?.ProdOrderPartslistID.ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    foreach (var item in response.Data)
                    {
                        ProdOrderPLIntermediates.Add(item);
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
