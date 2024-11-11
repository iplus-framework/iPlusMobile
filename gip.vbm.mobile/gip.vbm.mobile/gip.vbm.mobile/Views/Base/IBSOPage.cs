// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    public interface IBSOPage
    {
        NavParameter NavParam
        {
            get; set;
        }

        void OnNavigatedTo(NavigationEventArgs e, NavigationMode navigationMode);
        void OnNavigatedFrom(NavigationEventArgs e, NavigationMode navigationMode);
    }
}
