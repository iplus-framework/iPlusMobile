using System;
using System.Globalization;
using Xamarin.Forms;

namespace gip.vb.mobile.Helpers
{
    public class StringLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;
            string strVal = value as String;
            if (strVal == null)
                return 0;
            if (String.IsNullOrEmpty(strVal))
                return 0;
            return strVal.Length;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
