// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.webservices;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class ProdOrderInMaterialsViewModel : BaseViewModel
    {
        public ProdOrderInMaterialsViewModel(ProdOrderPartslistPos targetPos, BarcodeScanManuModel taskModel)
        {
            TargetPos = targetPos;
            TaskModel = taskModel;
            ProdOrderInMaterials = new ObservableCollection<ProdOrderPartslistPosRelation>();
            LoadProdOrderInMaterialsCommand = new Command(async () => await ExecuteLoadProdOrderInMaterialsCommand());
            Title = Strings.AppStrings.InputMaterials_Header; 
        }


        public ProdOrderPartslistPos TargetPos
        {
            get; set;
        }

        public BarcodeScanManuModel TaskModel
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

                bool isMultipleMaterialWFConnection = false;
                ProdOrderPartslistPos intermediateBatch = null;
                IEnumerable<Guid> intermediateBatchIDs = null;

                if (TaskModel != null)
                {
                    ProdOrderPartslistWFInfo wfInfo = TaskModel.SelectedSequence as ProdOrderPartslistWFInfo;
                    if (wfInfo != null && wfInfo.IntermediateBatch != null && wfInfo.MaterialWFConnectionMode > 0)
                    {
                        intermediateBatch = wfInfo.IntermediateBatch;
                        isMultipleMaterialWFConnection = true;
                        intermediateBatchIDs = wfInfo.IntermediateBatchIDs;
                    }
                }

                if (isMultipleMaterialWFConnection)
                {
                    List<ProdOrderPartslistPosRelation> inMaterials = new List<ProdOrderPartslistPosRelation>();

                    foreach (Guid intermediateBatchID in intermediateBatchIDs)
                    {
                        var response = await _WebService.GetProdOrderInputMaterialsAsync(intermediateBatchID.ToString());
                        this.WSResponse = response;
                        if (response.Suceeded)
                        {
                            foreach (var item in response.Data)
                            {
                                inMaterials.Add(item);
                            }
                        }
                    }

                    ProdOrderInMaterials = new ObservableCollection<ProdOrderPartslistPosRelation>(inMaterials.OrderBy(c => c.Sequence));
                    OnPropertyChanged(nameof(ProdOrderInMaterials));

                }
                else
                {
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
