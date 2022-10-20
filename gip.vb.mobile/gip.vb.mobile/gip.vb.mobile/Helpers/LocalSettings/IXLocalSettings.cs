using System;

namespace gip.vb.mobile.Helpers
{
    public interface IXLocalSettings
    {
        T GetValue<T>(String key, T defaultValue);
        void SetValue<T>(String key, T value);
    }
}
