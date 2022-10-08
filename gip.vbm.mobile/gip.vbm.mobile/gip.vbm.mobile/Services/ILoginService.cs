using gip.core.webservices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gip.vbm.mobile.Services
{
    public interface ILoginService
    {
        Task<VBUserRights> Login(VBUser user);
        void ReloadSettings();
    }
}
