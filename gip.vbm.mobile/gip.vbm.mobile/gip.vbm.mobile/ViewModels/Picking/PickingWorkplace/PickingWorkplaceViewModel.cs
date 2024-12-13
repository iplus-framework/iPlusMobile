// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.webservices;
using System;
using System.Collections.Generic;
using System.Text;

namespace gip.vbm.mobile.ViewModels
{
    public class PickingWorkplaceViewModel : PickingViewModel, IBarcodeScanACClassHost
    {
        public PickingWorkplaceViewModel() : base ()
        {
        }

        private ACClass _RegisteredWorkplace;
        public ACClass RegisteredWorkplace
        {
            get => _RegisteredWorkplace;
            set
            {
                SetProperty(ref _RegisteredWorkplace, value);
            }
        }

        public void OnACClassScanned(ACClass acClass)
        {
            Message = null;
            if (acClass == null)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Error, Strings.AppStrings.WrongScanWorkplace_Text);
                return;
            }
            RegisteredWorkplace = acClass;
        }
    }
}
