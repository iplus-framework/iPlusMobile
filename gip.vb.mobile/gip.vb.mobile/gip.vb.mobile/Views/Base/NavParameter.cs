// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿namespace gip.vb.mobile.Views
{
    public class NavParameter
    {
        public NavParameter(PageStateEnum pageMode)
        {
            PageMode = pageMode;
        }
        public PageStateEnum PageMode { get; set; }
        public object Arguments { get; set; }
    }
}
