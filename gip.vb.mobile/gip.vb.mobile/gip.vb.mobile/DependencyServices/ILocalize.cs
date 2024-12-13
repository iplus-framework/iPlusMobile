// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Globalization;

namespace gip.vb.mobile
{
    public interface ILocalize
    {
        CultureInfo GetOSDefaultCultureInfo();

        CultureInfo GetCurrentCultureInfo();

        void SetLocale(string i18nLangTag = null);
    }
}
