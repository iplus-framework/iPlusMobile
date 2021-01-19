using System;
using System.Globalization;
using Xamarin.Forms;

namespace gip.vb.mobile.Helpers
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string paramAsString = "{" + parameter as string + "}";
            string result = string.Format(paramAsString, value);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
