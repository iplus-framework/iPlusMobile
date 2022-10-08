using System;
using System.Globalization;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Helpers
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int precision = -1;
            if (parameter != null)
            {
                try
                {
                    precision = System.Convert.ToInt32(parameter);
                }
                catch
                {
                }
            }

            try
            {
                Decimal result = Decimal.Parse(value as string);
                if (precision >= 0)
                {
                    result = Math.Round(result, precision);
                }
                return result;
            }
            catch
            {
                return Decimal.Zero;
            }
        }
    }
}
