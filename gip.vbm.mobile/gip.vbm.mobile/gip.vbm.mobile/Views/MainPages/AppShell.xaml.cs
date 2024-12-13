// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.webservices;

namespace gip.vbm.mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //this.Items
            foreach (VBMenuItem vbMenutItem in App.UserRights.Menu)
            {
            }
        }
    }
}