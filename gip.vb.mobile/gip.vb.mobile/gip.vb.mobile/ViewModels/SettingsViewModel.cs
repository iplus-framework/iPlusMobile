using gip.core.datamodel;
using gip.vb.mobile.Helpers;
using System.Collections.Generic;

namespace gip.vb.mobile.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private static SettingsViewModel _StaticSettingModel;
        public static SettingsViewModel StaticSettingModel
        {
            get
            {
                if (_StaticSettingModel == null)
                    _StaticSettingModel = new SettingsViewModel();
                return _StaticSettingModel;
            }
        }

        #region Settings

        public bool? _UseMockDataStore;
        public bool UseMockDataStore
        {
            get
            {
                if (!_UseMockDataStore.HasValue)
                {
                    _UseMockDataStore = XLocalSettings.GetValue<bool>("UseMockDataStore", false);
                }
                return _UseMockDataStore.Value;
            }
            set
            {
                _UseMockDataStore = value;
                XLocalSettings.SetValue<bool>("UseMockDataStore", _UseMockDataStore.Value);
                OnPropertyChanged();
            }
        }

        public string _IPlusBackendUrl;
        public string IPlusBackendUrl
        {
            get
            {
                if (_IPlusBackendUrl == null)
                {
                    _IPlusBackendUrl = XLocalSettings.GetValue<string>("IPlusBackendUrl", "http://localhost:8730");
                }
                return _IPlusBackendUrl;
            }
            set
            {
                _IPlusBackendUrl = value;
                XLocalSettings.SetValue<string>("IPlusBackendUrl", _IPlusBackendUrl);
                OnPropertyChanged();
            }
        }


        public string _LastUser;
        public string LastUser
        {
            get
            {
                if (_LastUser == null)
                {
                    _LastUser = XLocalSettings.GetValue<string>("LastUser", "Mobile");
                }
                return _LastUser;
            }
            set
            {
                _LastUser = value;
                XLocalSettings.SetValue<string>("LastUser", _LastUser);
                OnPropertyChanged();
            }
        }

        public string _LastPassword;
        public string LastPassword
        {
            get
            {
#if DEBUG
                if (_LastPassword == null)
                {
                    _LastPassword = XLocalSettings.GetValue<string>("LastPassword", "Mobile");
                }
#endif
                return _LastPassword;
            }
            set
            {
                _LastPassword = value;
#if DEBUG
                XLocalSettings.SetValue<string>("LastPassword", _LastPassword);
#endif
                OnPropertyChanged();
            }
        }

        public bool? _LightTheme;
        public bool LightTheme
        {
            get
            {
                if (!_LightTheme.HasValue)
                {
                    _LightTheme = XLocalSettings.GetValue<bool>("LightTheme", false);
                }
                return _LightTheme.Value;
            }
            set
            {
                _LightTheme = value;
                XLocalSettings.SetValue<bool>("LightTheme", _LightTheme.Value);
                OnPropertyChanged();
            }
        }

        public string _ThemeColor;
        public string ThemeColor
        {
            get
            {
                if (_ThemeColor == null)
                {
                    _ThemeColor = XLocalSettings.GetValue<string>("ThemeColor", "#FF87CEFA"); //LightSkyBlue
                }
                return _ThemeColor;
            }
            set
            {
                _ThemeColor = value;
                XLocalSettings.SetValue<string>("ThemeColor", _ThemeColor);
                OnPropertyChanged();
            }
        }

        public string _SelectedLanguage;
        public string SelectedLanguage
        {
            get
            {
                if (_SelectedLanguage == null)
                {
                    _SelectedLanguage = XLocalSettings.GetValue<string>("SelectedLanguage", OSDefaultLang);
                }
                return _SelectedLanguage;
            }
            set
            {
                _SelectedLanguage = value;
                XLocalSettings.SetValue<string>("SelectedLanguage", _SelectedLanguage);
                OnPropertyChanged();
            }
        }

        public const string OSDefaultLang = "-- Android/iOS/Windows --";

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }

        //private static List<string> _SupportedLanguages;
        //public static IEnumerable<string> SupportedLanguages
        //{
        //    get
        //    {
        //        if (_SupportedLanguages != null)
        //            return _SupportedLanguages;
        //        _SupportedLanguages = new List<string>();
        //        _SupportedLanguages.Add(OSDefaultLang);
        //        _SupportedLanguages.Add((new SpeakTexts_ar_AR()).Language);
        //        _SupportedLanguages.Add((new SpeakTexts_de_DE()).Language);
        //        _SupportedLanguages.Add((new SpeakTexts_en_US()).Language);
        //        _SupportedLanguages.Add((new SpeakTexts_es_ES()).Language);
        //        _SupportedLanguages.Add((new SpeakTexts_fr_FR()).Language);
        //        _SupportedLanguages.Add((new SpeakTexts_pt_PT()).Language);
        //        _SupportedLanguages.Add((new SpeakTexts_ru_RU()).Language);
        //        return _SupportedLanguages;
        //    }
        //}


        public bool? _PerfLoggingOn;
        public bool PerfLoggingOn
        {
            get
            {
                if (!_PerfLoggingOn.HasValue)
                {
                    _PerfLoggingOn = XLocalSettings.GetValue<bool>("PerfLoggingOn", false);
                }
                return _PerfLoggingOn.Value;
            }
            set
            {
                _PerfLoggingOn = value;
                XLocalSettings.SetValue<bool>("PerfLoggingOn", _PerfLoggingOn.Value);
                OnPropertyChanged();
            }
        }

        public string _LastCrashDump;
        public string LastCrashDump
        {
            get
            {
                if (_LastCrashDump == null)
                {
                    _LastCrashDump = XLocalSettings.GetValue<string>("LastCrashDump", "");
                }
                return _LastCrashDump;
            }
            set
            {
                _LastCrashDump = value;
                XLocalSettings.SetValue<string>("LastCrashDump", _LastCrashDump);
                OnPropertyChanged();
            }
        }

        #endregion

    }
}
