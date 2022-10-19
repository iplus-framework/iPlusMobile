﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.webservices;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.ViewModels
{
    public class ProdOrderInMaterialsViewModel : BaseViewModel
    {
        public ProdOrderInMaterialsViewModel(ProdOrderPartslistPos targetPos)
        {
            TargetPos = targetPos;
            ProdOrderInMaterials = new ObservableCollection<ProdOrderPartslistPosRelation>();
            LoadProdOrderInMaterialsCommand = new Command(async () => await ExecuteLoadProdOrderInMaterialsCommand());
            Title = Strings.AppStrings.InputMaterials_Header; 
        }


        public ProdOrderPartslistPos TargetPos
        {
            get; set;
        }

        public ObservableCollection<ProdOrderPartslistPosRelation> ProdOrderInMaterials { get; set; }
        public Command LoadProdOrderInMaterialsCommand
        {
            get; set;
        }


        public async Task ExecuteLoadProdOrderInMaterialsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                ProdOrderInMaterials.Clear();
                var response = await _WebService.GetProdOrderInputMaterialsAsync(TargetPos?.ProdOrderPartslistPosID.ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                {
                    foreach (var item in response.Data)
                    {
                        ProdOrderInMaterials.Add(item);
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