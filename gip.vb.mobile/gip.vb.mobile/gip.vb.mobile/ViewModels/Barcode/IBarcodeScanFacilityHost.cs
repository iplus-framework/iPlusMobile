using gip.mes.webservices;
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
