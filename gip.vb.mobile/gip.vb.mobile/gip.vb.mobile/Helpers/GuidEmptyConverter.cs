using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace gip.vb.mobile.Helpers
{
    public class GuidEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guid? guid = value as Guid?;
            if (guid.HasValue && guid.Value == Guid.Empty)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? boolValue = value as bool?;
            if (boolValue.HasValue && boolValue.Value)
                return Guid.Empty;

            return null;
        }
    }
}
