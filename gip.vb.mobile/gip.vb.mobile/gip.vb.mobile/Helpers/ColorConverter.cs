using System;
using System.Globalization;
using Xamarin.Forms;

namespace gip.vb.mobile.Helpers
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                String strColor = value as String;
                if (String.IsNullOrEmpty(strColor))
                    return Color.White;
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
                Color color = (Color) value;
                HexColor hexColor = color;
                return hexColor.ToString();
            }
            catch
            {
            }
            return "#FFFFFFFF";
        }
    }
}
