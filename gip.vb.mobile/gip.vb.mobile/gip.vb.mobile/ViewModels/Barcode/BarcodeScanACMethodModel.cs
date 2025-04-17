﻿using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Text;

namespace gip.vb.mobile.ViewModels
{
    public class BarcodeScanACMethodModel : BaseViewModel
    {
        public BarcodeScanACMethodModel(ProdOrderPartslistWFInfo wfInfo)
        {
            WFInfo = wfInfo;
        }

        private ProdOrderPartslistWFInfo _WFInfo;
        public ProdOrderPartslistWFInfo WFInfo
        {
            get => _WFInfo;
            set
            {
                _WFInfo = value;
                OnPropertyChanged();
            }
        }

        public bool Apply(bool userChangedEndTime)
        {
            if (WFInfo.UserTime != null)
            {
                bool isValid = WFInfo.UserTime.UpdateDate(userChangedEndTime);
                if (!isValid)
                {
                    ShowDialog(new Msg(eMsgLevel.Error, Strings.AppStrings.InvalidDate));
                    return false;
                }
            }
            WFInfo.WFMethod.AutoRemove = true;
            return true;
        }

        public override void DialogResponse(Global.MsgResult result, string enteredValue = null)
        {
        }
    }
}
