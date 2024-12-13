// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;

namespace gip.vb.mobile.Helpers
{
    public interface IXLocalSettings
    {
        T GetValue<T>(String key, T defaultValue);
        void SetValue<T>(String key, T value);
    }
}
