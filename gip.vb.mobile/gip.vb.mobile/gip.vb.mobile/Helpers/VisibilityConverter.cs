using System;
using System.Globalization;
using Xamarin.Forms;

namespace gip.vb.mobile.Helpers
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool valAsBool = false;
                if (value is TimeSpan)
                {
                    if ((TimeSpan)value != TimeSpan.Zero)
                        valAsBool = true;
                }
                else
                {
                    if (value != null)
                    {
                        if (value is IConvertible)
                            valAsBool = System.Convert.ToBoolean(value);
                        else
                            valAsBool = true;
                    }
                    else
                        valAsBool = false;
                }
                if (parameter != null && parameter is bool && (bool)parameter)
                    valAsBool = !valAsBool;
                else if (parameter != null && parameter is string && ((parameter as string) == "True" || (parameter as string) == "true"))
                    valAsBool = !valAsBool;
                if (valAsBool)
                    return true;
            }
            catch
            {
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
