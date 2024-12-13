// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Globalization;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Helpers
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                String strColor = value as String;
                if (String.IsNullOrEmpty(strColor))
                    return Colors.White;
                HexColor hexColor = new HexColor(strColor);
                Color color = hexColor;
                return color;
            }
            catch
            {
            }
            return Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Color color = (Color) value;
                HexColor hexColor = color;
                return hexColor.ToString();
            }
            catch
            {
            }
            return "#FFFFFFFF";
        }
    }
}
