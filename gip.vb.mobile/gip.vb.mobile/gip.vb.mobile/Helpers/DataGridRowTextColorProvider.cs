// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.DataGrid;

namespace gip.vb.mobile.Helpers
{
    public class DataGridRowTextColorProvider : IColorProvider
    {
        public Color GetColor(int rowIndex, object item)
        {
            object objColor = null;
            if (App.Current.Resources.TryGetValue("PrimaryTextColor", out objColor))
            {
                return (Color) objColor;
            }
            return Color.White;
        }
    }
}
