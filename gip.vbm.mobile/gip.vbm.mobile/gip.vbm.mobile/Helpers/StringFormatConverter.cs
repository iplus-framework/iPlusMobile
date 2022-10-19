﻿using System;
using System.Globalization;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Helpers
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string paramAsString = String.Format("{{{0}}}", parameter as string);// "{" + parameter as string + "}";
            string result = string.Format(paramAsString, value);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}