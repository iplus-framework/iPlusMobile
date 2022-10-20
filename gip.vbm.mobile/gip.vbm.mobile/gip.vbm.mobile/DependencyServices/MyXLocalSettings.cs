using gip.vbm.mobile.Helpers;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile
{
    public class MyXLocalSettings : IXLocalSettings
    {
        public T GetValue<T>(string key, T defaultValue)
        {
            if (Microsoft.Maui.Storage.Preferences.ContainsKey(key))
            {
                var typeOfT = typeof(T);
                if (typeOfT == typeof(string))
                {
                    string pValue = defaultValue as string;
                    return (T)(object)Microsoft.Maui.Storage.Preferences.Get(key, pValue);
                }
                else if (typeOfT == typeof(bool))
                {
                    bool pValue = (bool)(object)defaultValue;
                    return (T)(object)Microsoft.Maui.Storage.Preferences.Get(key, pValue);
                }
                else
                    return defaultValue;
            }
            else
                return defaultValue; //default(T);
        }

        public void SetValue<T>(string key, T value)
        {
            if (value is string)
                Microsoft.Maui.Storage.Preferences.Set(key, value as string);
            else if (value is bool)
                Microsoft.Maui.Storage.Preferences.Set(key, (bool)(object)value);
            //Application.Current.SavePropertiesAsync();
        }
    }
}
