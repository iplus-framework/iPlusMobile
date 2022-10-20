using System;

namespace gip.vbm.mobile.Helpers
{
    public interface IXLocalSettings
    {
        T GetValue<T>(String key, T defaultValue);
        void SetValue<T>(String key, T value);
    }
}
