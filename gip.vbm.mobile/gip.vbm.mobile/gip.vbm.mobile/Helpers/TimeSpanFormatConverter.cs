using System;
using System.Globalization;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Helpers
{
    public class TimeSpanFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string paramAsString = "{0:00}:{1:00}";
            TimeSpan timeSpan = (TimeSpan) value;
            int minutes = timeSpan.Minutes;
            if (timeSpan.Hours > 0)
                minutes += (timeSpan.Hours * 60);
            return string.Format(paramAsString, minutes, timeSpan.Seconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
