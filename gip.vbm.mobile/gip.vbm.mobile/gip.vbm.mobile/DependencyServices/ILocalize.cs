using System;
using System.Globalization;

namespace gip.vbm.mobile
{
    public interface ILocalize
    {
        CultureInfo GetOSDefaultCultureInfo();

        CultureInfo GetCurrentCultureInfo();

        void SetLocale(string i18nLangTag = null);
    }
}
