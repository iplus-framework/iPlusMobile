// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    public class NavigationPageBSO : NavigationPage
    {
        public NavigationPageBSO()
            : base()
        {
        }

        public NavigationPageBSO(Page root)
            : base(root)
        {
        }

        protected override void OnAppearing()
        {
            if (this.CurrentPage != null)
            {
                SetHasNavigationBar(this.CurrentPage, false);
            }
            base.OnAppearing();
        }
    }
}
