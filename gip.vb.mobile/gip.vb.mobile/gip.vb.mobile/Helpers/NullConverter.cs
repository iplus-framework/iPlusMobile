using System;
using System.Globalization;
using Xamarin.Forms;

namespace gip.vb.mobile.Helpers
{
    public class NullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int temp;
            if (string.IsNullOrEmpty((string)value) || !int.TryParse((string)value, out temp))
                return null;
            else
                return temp;
        }
    }
}
