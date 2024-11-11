// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Text;

namespace gip.vb.mobile.ViewModels
{
    public interface IBarcodeScanFacilityHost
    {
        void OnFacilityScanned(Facility facility);

        void OnSearchFacility(string searchText);
    }
}
