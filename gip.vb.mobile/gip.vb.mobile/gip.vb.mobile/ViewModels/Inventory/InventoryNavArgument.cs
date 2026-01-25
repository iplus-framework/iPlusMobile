// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.webservices;
using System;

namespace gip.vb.mobile.ViewModels.Inventory
{
    public class InventoryNavArgument
    {

        #region ctor's
        public InventoryNavArgument()
        {

        }


        #endregion

        #region Properties

        public string StorageLocationNo { get; set; }
        public string FacilityInventoryNo { get; set; }

        public Guid FacilityInventoryID { get; set; }

        public EditModeEnum EditMode { get; set; }

        public bool IsValidateAndComplete { get; set; }

        public Facility SelectedStorageLocation { get; set; }

        public Facility SelectedFacility { get; set; }

        public FacilityInventory SelectedFacilityInventory { get; set; }

        public FacilityInventoryPos SelectedInventoryLine { get; set; }
        #endregion

    }
}
