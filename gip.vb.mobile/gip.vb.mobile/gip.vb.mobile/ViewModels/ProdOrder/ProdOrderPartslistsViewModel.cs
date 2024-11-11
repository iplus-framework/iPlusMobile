// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.webservices;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class ProdOrderPartslistsViewModel : BaseViewModel
    {
        public ObservableCollection<ProdOrderPartslist> ProdOrderPartslists { get; set; }
        public Command LoadProdOrderPartslistsCommand { get; set; }

        public ProdOrderPartslistsViewModel()
        {
            ProdOrderPartslists = new ObservableCollection<ProdOrderPartslist>();
            LoadProdOrderPartslistsCommand = new Command(async () => await ExecuteLoadProdOrderPartslistsCommand());
            LoadBarcodeEntityCommand = new Command(async() => await ExecuteLoadProdOrderPartslistsCommand());
        }

        public async Task ExecuteLoadProdOrderPartslistsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                ProdOrderPartslists.Clear();
                var response = await _WebService.GetProdOrderPartslistsAsync();
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    foreach (var item in response.Data)
                    {
                        ProdOrderPartslists.Add(item);
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

        public Command LoadBarcodeEntityCommand { get; set; }
        public async Task<ProdOrderPartslistPos> ExecuteLoadBarcodeEntityCommand()
        {
            if (IsBusy)
                return null;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetProdOrderIntermOrIntermBatchByMachineAsync(this.CurrentBarcode);
                this.WSResponse = response;
                return response.Data;
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
                return null;
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
