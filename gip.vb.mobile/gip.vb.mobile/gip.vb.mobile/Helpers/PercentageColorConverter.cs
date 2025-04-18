// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace gip.vb.mobile.Helpers
{
    public class PercentageColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? val = value as double?;
            if (!val.HasValue || val.Value < 25)
                return Color.Red;

            else if (val.Value < 50)
                return Color.Orange;

            else if (val.Value < 66)
                return Color.Yellow;

            else if (val.Value < 82)
                return Color.Green;

            return Color.Lime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
