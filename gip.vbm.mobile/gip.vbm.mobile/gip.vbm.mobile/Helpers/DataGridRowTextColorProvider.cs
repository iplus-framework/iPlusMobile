// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Globalization;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Xamarin.Forms.DataGrid;

namespace gip.vbm.mobile.Helpers
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
            return Color.FromRgb(0xFF, 0xFF, 0xFF);
        }
    }
}
