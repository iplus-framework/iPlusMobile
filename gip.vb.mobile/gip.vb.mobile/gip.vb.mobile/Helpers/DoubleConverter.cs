using System;
using System.Globalization;
using Xamarin.Forms;

namespace gip.vb.mobile.Helpers
{
    public class DoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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

            if (precision > 0 && value != null)
            {
                double doubleVal = System.Convert.ToDouble(value);
                return doubleVal.ToString(String.Format("F{0}", precision));
            }
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Nullable.GetUnderlyingType(targetType) != null && value == null)
                return null;
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
                Double result = Double.Parse(value as string);
                if (precision >= 0)
                {
                    result = Math.Round(result, precision);
                }
                return result;
            }
            catch
            {
                return 0.0;
            }
        }
    }
}
