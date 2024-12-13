// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using Xamarin.Forms;

namespace gip.vb.mobile.Views
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
