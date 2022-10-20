using System;
using System.Collections.Generic;
using System.Text;

namespace gip.vbm.mobile.DependencyServices
{
    public interface IUtility
    {
        void LongAlert(string message);
        void ShortAlert(string message);
        void CloseApp();
    }
}
