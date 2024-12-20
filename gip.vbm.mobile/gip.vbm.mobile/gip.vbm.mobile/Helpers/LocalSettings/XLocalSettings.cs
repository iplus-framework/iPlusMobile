// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace gip.vbm.mobile.Helpers
{
    public static class XLocalSettings
    {
        private static IXLocalSettings _Settings;
        public static void SetLocalSettings(IXLocalSettings settings)
        {
            _Settings = settings;
        }

        public static T GetValue<T>(String key, T defaultValue)
        {
            if (_Settings != null)
                return _Settings.GetValue<T>(key, defaultValue);
            return default(T);
        }

        public static void SetValue<T>(String key, T value)
        {
            if (_Settings != null)
                _Settings.SetValue<T>(key, value);
        }
    }
}
