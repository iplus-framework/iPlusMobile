using System;
using System.Resources;
using System.Globalization;
using System.Reflection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Helpers
{
    public class TranslateExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public string FullQName { get; set; }

        readonly CultureInfo OSDefaultCultureInfo;

        private CultureInfo _CurrentCultureInfo;
        public CultureInfo CurrentCultureInfo
        {
            get
            {
                if (_CurrentCultureInfo != null)
                    return _CurrentCultureInfo;
                DependencyService.Get<ILocalize>().SetLocale(ViewModels.SettingsViewModel.StaticSettingModel.SelectedLanguage);
                _CurrentCultureInfo = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                if (_CurrentCultureInfo == null)
                    return OSDefaultCultureInfo;
                return _CurrentCultureInfo;
            }
        }

        public TranslateExtension()
        {
            OSDefaultCultureInfo = DependencyService.Get<ILocalize>().GetOSDefaultCultureInfo();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (String.IsNullOrEmpty(Text))
                return "";
            string translation = "";
            if (!String.IsNullOrEmpty(FullQName))
            {
                translation = App.UserRights.Translation.GetTranslation(FullQName, Text);
                if (!String.IsNullOrEmpty(translation))
                    return translation;
            }

            IProvideValueTarget service = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (service != null && App.UserRights != null && App.UserRights.Translation != null)
            {
                //Xamarin.Forms.Xaml.Internals.SimpleValueTargetProvider formsProvider = service as Xamarin.Forms.Xaml.Internals.SimpleValueTargetProvider;
                //we need dependency objects / properties
                var target = service.TargetObject as BindableObject;
                if (target != null && target.BindingContext != null)
                {
                    Type type = target.BindingContext.GetType();
                    if (type != null)
                    {
                        translation = App.UserRights.Translation.GetTranslation(type.FullName, Text);
                        if (!String.IsNullOrEmpty(translation))
                            return translation;
                    }
                }
            }

            ResourceManager temp = new ResourceManager(MyXResourceLoader.ResourceID, typeof(TranslateExtension).Assembly);
            try
            {
                translation = temp.GetString(Text, CurrentCultureInfo);
                if (String.IsNullOrEmpty(translation))
                    return Text;
            }
            catch (Exception)
            {
                return Text;
            }
//            if (translation == null)
//            {
//#if DEBUG
//                throw new ArgumentException(
//                    String.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", Text, MyXResourceLoader.ResourceID, CurrentCultureInfo.Name),
//                    "Text");
//#else
//				translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
//#endif
//            }
            return translation;
        }
    }
}
