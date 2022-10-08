using System;
using System.Resources;
using System.Globalization;
using gip.vbm.mobile.Helpers;
using gip.vbm.mobile.Strings;
using gip.vbm.mobile.Themes;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile
{
    public class MyXResourceLoader : IXResourceLoader
    {
        public static readonly CultureInfo OSDefaultCultureInfo;

        private static CultureInfo _CurrentCultureInfo;
        public static CultureInfo CurrentCultureInfo
        {
            get
            {
                if (_CurrentCultureInfo != null)
                    return _CurrentCultureInfo;
                _CurrentCultureInfo = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                if (_CurrentCultureInfo == null)
                    return OSDefaultCultureInfo;
                return _CurrentCultureInfo;
            }
        }

        static MyXResourceLoader()
        {
            OSDefaultCultureInfo = DependencyService.Get<ILocalize>().GetOSDefaultCultureInfo();
        }

        public string GetString(string key)
        {
            ResourceManager temp = new ResourceManager(ResourceID, typeof(AppStrings).Assembly);
            key = key.Replace("/", "_");
            key = key.Replace(".", "_");
            string result = temp.GetString(key, CurrentCultureInfo);
            return result;
        }

        private static string _ResourceID;
        public static string ResourceID
        {
            get
            {
                if (!String.IsNullOrEmpty(_ResourceID))
                    return _ResourceID;

//#if __IOS__
//_ResourceID = "gip.vbm.mobile.iOS.AppStrings";
//#endif
//#if __ANDROID__
                //_ResourceID = "gip.vbm.mobile.Android.Strings.AppStrings";
                _ResourceID = "gip.vbm.mobile.Strings.AppStrings";
                //#endif

                return _ResourceID;
            }
        }

        public static void ChangeTheme(ResourceDictionary dict, bool lightTheme)
        {
            dict.MergedDictionaries.Clear();
            if (lightTheme)
            {
                dict.Add(new LightTheme());
                //Style style = new Style(typeof(Label));
                //style.Setters.Add(new Setter() { Property = Label.TextColorProperty, Value = Color.Black });
                //dict.Add(style);

                //style = new Style(typeof(Label));
                //style.Setters.Add(new Setter() { Property = Label.TextColorProperty, Value = Color.FromRgb((int)0, (int)100, (int)200) });
                //dict.Add("HRLabelForDesc", style);

                //style = new Style(typeof(Label));
                //style.Setters.Add(new Setter() { Property = Label.TextColorProperty, Value = Color.Black });
                //style.Setters.Add(new Setter() { Property = Label.FontSizeProperty, Value = (double)16 });
                //dict.Add("HRLabelForData", style);

                //style = new Style(typeof(Entry));
                //style.Setters.Add(new Setter() { Property = Entry.TextColorProperty, Value = Color.Black });
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgba((int)100, (int)100, (int)100, (int)40) });
                //dict.Add("HREntry", style);

                //style = new Style(typeof(NumericEntry));
                //style.Setters.Add(new Setter() { Property = Entry.TextColorProperty, Value = Color.Black });
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgba((int)100, (int)100, (int)100, (int)40) });
                //dict.Add(style);

                //style = new Style(typeof(Button));
                //style.Setters.Add(new Setter() { Property = Button.TextColorProperty, Value = Color.White });
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgb((int)100, (int)100, (int)100) });
                //dict.Add("HRButton", style);

                //style = new Style(typeof(Picker));
                //style.Setters.Add(new Setter() { Property = Picker.TextColorProperty, Value = Color.Black });
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgba((int)100, (int)100, (int)100, (int)40) });
                //dict.Add(style);

                //style = new Style(typeof(DatePicker));
                //style.Setters.Add(new Setter() { Property = DatePicker.TextColorProperty, Value = Color.Black });
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgba((int)100, (int)100, (int)100, (int)40) });
                //dict.Add(style);

                //style = new Style(typeof(TimePicker));
                //style.Setters.Add(new Setter() { Property = TimePicker.TextColorProperty, Value = Color.Black });
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgba((int)100, (int)100, (int)100, (int)40) });
                //dict.Add(style);

                //style = new Style(typeof(CheckBox));
                //style.Setters.Add(new Setter() { Property = CheckBox.TextColorProperty, Value = Color.Black });
                //dict.Add(style);

                //style = new Style(typeof(RadioButton));
                //style.Setters.Add(new Setter() { Property = RadioButton.TextColorProperty, Value = Color.Black });
                //dict.Add(style);

                //style = new Style(typeof(Editor));
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.White });
                //style.Setters.Add(new Setter() { Property = Editor.TextColorProperty, Value = Color.Black });
                //dict.Add("VBEditor", style);

                //style = new Style(typeof(SearchBar));
                //style.Setters.Add(new Setter() { Property = SearchBar.TextColorProperty, Value = Color.Black });
                //style.Setters.Add(new Setter() { Property = SearchBar.PlaceholderColorProperty, Value = Color.Black });
                //style.Setters.Add(new Setter() { Property = SearchBar.CancelButtonColorProperty, Value = Color.Black });
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgb((int)31, (int)174, (int)206) });
                //dict.Add(style);

                //style = new Style(typeof(Pivot));
                //style.Setters.Add(new Setter() { Property = Pivot.SelectedColorProperty, Value = Color.FromRgb((int)0, (int)100, (int)200) });
                //style.Setters.Add(new Setter() { Property = Pivot.UnselectedColorProperty, Value = Color.FromRgb((int)40, (int)40, (int)40) });
                //dict.Add(style);

                //dict.MergedWith = typeof(Xamarin.Forms.Themes.LightThemeResources);

            }
            else
            {
                dict.Add(new DarkTheme());
                //dict.MergedWith = typeof(Xamarin.Forms.Themes.DarkThemeResources);

                //Style style = new Style(typeof(Label));
                //style.Setters.Add(new Setter() { Property = Label.TextColorProperty, Value = Color.White });
                //dict.Add(style);

                //style = new Style(typeof(Label));
                //style.Setters.Add(new Setter() { Property = Label.TextColorProperty, Value = Color.FromRgb((int)31, (int)174, (int)206) });
                //dict.Add("HRLabelForDesc", style);

                //style = new Style(typeof(Label));
                //style.Setters.Add(new Setter() { Property = Label.TextColorProperty, Value = Color.White });
                //style.Setters.Add(new Setter() { Property = Label.FontSizeProperty, Value = (double)16 });
                //dict.Add("HRLabelForData", style);

                //style = new Style(typeof(Entry));
                //style.Setters.Add(new Setter() { Property = Entry.TextColorProperty, Value = Color.White });
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgba((int)100, (int)100, (int)100, (int)40) });
                //dict.Add("HREntry", style);

                //style = new Style(typeof(NumericEntry));
                //style.Setters.Add(new Setter() { Property = Entry.TextColorProperty, Value = Color.White });
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgba((int)100, (int)100, (int)100, (int)40) });
                //dict.Add(style);

                //style = new Style(typeof(Button));
                //style.Setters.Add(new Setter() { Property = Button.TextColorProperty, Value = Color.Black });
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgb((int)140, (int)140, (int)140) });
                //dict.Add("HRButton", style);

                //style = new Style(typeof(Picker));
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgba((int)100, (int)100, (int)100, (int)40) });
                //style.Setters.Add(new Setter() { Property = Picker.TextColorProperty, Value = Color.White });
                //dict.Add(style);

                //style = new Style(typeof(DatePicker));
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgba((int)100, (int)100, (int)100, (int)40) });
                //style.Setters.Add(new Setter() { Property = DatePicker.TextColorProperty, Value = Color.White });
                //dict.Add(style);

                //style = new Style(typeof(TimePicker));
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgba((int)100, (int)100, (int)100, (int)40) });
                //style.Setters.Add(new Setter() { Property = TimePicker.TextColorProperty, Value = Color.White });
                //dict.Add(style);

                //style = new Style(typeof(CheckBox));
                //style.Setters.Add(new Setter() { Property = CheckBox.TextColorProperty, Value = Color.White });
                //dict.Add(style);

                //style = new Style(typeof(RadioButton));
                //style.Setters.Add(new Setter() { Property = RadioButton.TextColorProperty, Value = Color.White });
                //dict.Add(style);

                //style = new Style(typeof(Editor));
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgb((int)40, (int)40, (int)40) });
                //style.Setters.Add(new Setter() { Property = Editor.TextColorProperty, Value = Color.White });
                //dict.Add("VBEditor", style);

                //style = new Style(typeof(SearchBar));
                //style.Setters.Add(new Setter() { Property = SearchBar.TextColorProperty, Value = Color.White });
                //style.Setters.Add(new Setter() { Property = SearchBar.PlaceholderColorProperty, Value = Color.White });
                //style.Setters.Add(new Setter() { Property = SearchBar.CancelButtonColorProperty, Value = Color.White });
                //style.Setters.Add(new Setter() { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgba((int)100, (int)100, (int)100, (int)40) });
                //dict.Add(style);

                //style = new Style(typeof(Pivot));
                //style.Setters.Add(new Setter() { Property = Pivot.SelectedColorProperty, Value = Color.FromRgb((int)0, (int)100, (int)200) });
                //style.Setters.Add(new Setter() { Property = Pivot.UnselectedColorProperty, Value = Color.FromRgb((int)40, (int)40, (int)40) });
                //dict.Add(style);
            }
        }
    }
}
