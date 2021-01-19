using Xamarin.Forms;
using gip.vb.mobile.Helpers;

namespace gip.vb.mobile
{
    public class MyXLocalSettings : IXLocalSettings
    {
        public T GetValue<T>(string key, T defaultValue)
        {
            if (Application.Current.Properties.ContainsKey(key))
            {
                return (T)Application.Current.Properties[key];
            }
            else
                return defaultValue; //default(T);
        }

        public void SetValue<T>(string key, T value)
        {
            Application.Current.Properties[key] = (T)value;
        }
    }
}
