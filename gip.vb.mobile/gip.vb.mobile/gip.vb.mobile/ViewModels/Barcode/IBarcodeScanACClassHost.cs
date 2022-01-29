using gip.core.webservices;
using System;
using System.Collections.Generic;
using System.Text;

namespace gip.vb.mobile.ViewModels
{
    public interface IBarcodeScanACClassHost
    {
        void OnACClassScanned(ACClass acClass);
    }
}
