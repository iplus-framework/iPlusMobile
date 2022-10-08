using System;
using System.Globalization;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Helpers
{
    public class SolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                String strColor = value as String;
                if (String.IsNullOrEmpty(strColor))
                {
                    return Color.White;
                }
                HexColor hexColor = new HexColor(strColor);
                Color color = hexColor;
                return color;
            }
            catch
            {
            }
            return Color.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Color brush = (Color)parameter;
                if (brush != null)
                {
                    HexColor hexColor = brush;
                    return hexColor.ToString();
                }
            }
            catch
            {
            }
            return "#FFFFFFFF";
        }
    }
}
