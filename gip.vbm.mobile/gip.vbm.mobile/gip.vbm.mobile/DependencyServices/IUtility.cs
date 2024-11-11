// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
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
