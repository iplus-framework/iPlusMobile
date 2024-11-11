// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace gip.vbm.mobile.Helpers
{
    public static class XResourceLoader
    {
        private static IXResourceLoader _ResourceLoader;
        public static void SetResourceLoader(IXResourceLoader resourceLoader)
        {
            _ResourceLoader = resourceLoader;
        }

        public static string GetString(String resource)
        {
            if (_ResourceLoader != null)
            {
                return _ResourceLoader.GetString(resource);
                //string rString = _ResourceLoader.GetString(resource);
                //if (!String.IsNullOrEmpty(rString))
                //    rString = rString.Replace("`n", "\n");
                //return rString;
            }
            return resource;
        }
    }
}
