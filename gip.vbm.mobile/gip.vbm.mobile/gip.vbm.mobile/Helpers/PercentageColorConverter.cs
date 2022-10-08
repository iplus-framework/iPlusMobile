using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Helpers
{
    public class PercentageColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? val = value as double?;
            if (!val.HasValue || val.Value < 25)
                return Colors.Red;

            else if (val.Value < 50)
                return Colors.Orange;

            else if (val.Value < 66)
                return Colors.Yellow;

            else if (val.Value < 82)
                return Colors.Green;

            return Colors.Lime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
