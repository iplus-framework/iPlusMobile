using System;
using Xamarin.Forms;
using System.Threading;
using gip.vb.mobile.ViewModels;
using System.Globalization;

[assembly: Dependency(typeof(gip.vb.mobile.Droid.Localize))]

namespace gip.vb.mobile.Droid
{
    public class Localize : gip.vb.mobile.ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            return Thread.CurrentThread.CurrentCulture;
        }

        public System.Globalization.CultureInfo GetOSDefaultCultureInfo()
        {
            var androidLocale = Java.Util.Locale.Default;
            var i18nLangTag = androidLocale.ToString().Replace("_", "-");
            return new System.Globalization.CultureInfo(i18nLangTag);
        }

        public void SetLocale(string i18nLangTag = null)
        {
            if (String.IsNullOrEmpty(i18nLangTag) || i18nLangTag == SettingsViewModel.OSDefaultLang)
            {
                var androidLocale = Java.Util.Locale.Default; // user's preferred locale
                i18nLangTag = androidLocale.ToString().Replace("_", "-");
            }
            try
            {
                CultureInfo ci = new System.Globalization.CultureInfo(i18nLangTag);
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
            }
            catch (Exception)
            {
            }
        }
    }
}