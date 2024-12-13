// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.webservices;
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
