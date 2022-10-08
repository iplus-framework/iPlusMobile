using gip.vbm.mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Controls
{
    public enum BoxViewButtonEffect
    {
        HighlightFillWithTheme,
        HighlightBorderWithTheme,
        HighlightFill,
        HighlightBorder,
        HighlightDarkLightFill,
        HighlightDarkLightBorder
    }


    //[Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
    public partial class BoxViewButton : ContentView
	{
		public BoxViewButton ()
		{
			InitializeComponent();
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = "Source";
            ctlImage.SetBinding(Image.SourceProperty, binding);
            var tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += OnInternalItemTap;
            this.GestureRecognizers.Add(tapRecognizer);
        }

        bool _IsInitialized = false;
        protected override void OnParentSet()
        {
            if (!_IsInitialized)
            {
                InitColors();
                _IsInitialized = true;
            }
            base.OnParentSet();
        }

        public static readonly BindableProperty ButtonEffectProperty = BindableProperty.Create("ButtonEffect", typeof(BoxViewButtonEffect), typeof(BoxViewButton), BoxViewButtonEffect.HighlightFillWithTheme, BindingMode.OneWay, null, OnButtonEffectChanged);

        public BoxViewButtonEffect ButtonEffect
        {
            get { return (BoxViewButtonEffect)GetValue(ButtonEffectProperty); }
            set { SetValue(ButtonEffectProperty, value); }
        }


        public static readonly BindableProperty IsToggleableProperty = BindableProperty.Create("IsToggleable", typeof(bool), typeof(BoxViewButton), default(bool));

        public bool IsToggleable
        {
            get { return (bool)GetValue(IsToggleableProperty); }
            set { SetValue(IsToggleableProperty, value); }
        }

        public static readonly BindableProperty BorderCheckedColorProperty = BindableProperty.Create("BorderCheckedColor", typeof(Color), typeof(BoxViewButton), Color.FromRgba(0xFF,0xFF,0xFF,0x00));

        public Color BorderCheckedColor
        {
            get { return (Color)GetValue(BorderCheckedColorProperty); }
            set { SetValue(BorderCheckedColorProperty, value); }
        }


        public static readonly BindableProperty BorderUnCheckedColorProperty = BindableProperty.Create("BorderUnCheckedColor", typeof(Color), typeof(BoxViewButton), Color.FromRgba(0xFF, 0xFF, 0xFF, 0x00));

        public Color BorderUnCheckedColor
        {
            get { return (Color)GetValue(BorderUnCheckedColorProperty); }
            set { SetValue(BorderUnCheckedColorProperty, value); }
        }


        public static readonly BindableProperty FillCheckedColorProperty = BindableProperty.Create("FillCheckedColor", typeof(Color), typeof(BoxViewButton), Color.FromRgba(0xFF, 0xFF, 0xFF, 0x00));

        public Color FillCheckedColor
        {
            get { return (Color)GetValue(FillCheckedColorProperty); }
            set { SetValue(FillCheckedColorProperty, value); }
        }

        public static readonly BindableProperty FillUnCheckedColorProperty = BindableProperty.Create("FillUnCheckedColor", typeof(Color), typeof(BoxViewButton), Color.FromRgba(0xFF, 0xFF, 0xFF, 0x00));

        public Color FillUnCheckedColor
        {
            get { return (Color)GetValue(FillUnCheckedColorProperty); }
            set { SetValue(FillUnCheckedColorProperty, value); }
        }

        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create("IsChecked", typeof(bool?), typeof(BoxViewButton), null, BindingMode.TwoWay, null, OnCheckedPropertyChanged);
        /// <summary>
        /// Gets or sets a value indicating whether the control is checked.
        /// </summary>
        /// <value>The checked state.</value>
        public bool? IsChecked
        {
            get
            {
                return this.GetValue<bool?>(IsCheckedProperty);
            }

            set
            {
                if (this.IsChecked != value)
                {
                    this.SetValue(IsCheckedProperty, value);
                    if (this.CheckedChanged != null)
                        this.CheckedChanged.Invoke(this, new EventArgs<bool?>(value));
                }
            }
        }

        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(ImageSource), typeof(BoxViewButton), null, BindingMode.OneWay, null);
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }


        public static readonly BindableProperty IsImageVisibleProperty = BindableProperty.Create("IsImageVisible", typeof(bool), typeof(BoxViewButton), true, BindingMode.OneWay, null, OnImageVisiblityChanged);

        public bool IsImageVisible
        {
            get { return (bool)GetValue(IsImageVisibleProperty); }
            set { SetValue(IsImageVisibleProperty, value); }
        }


        public static readonly BindableProperty ToolTipProperty = BindableProperty.Create("ToolTip", typeof(string), typeof(BoxViewButton), default(string));
        public String ToolTip
        {
            get { return (String)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        #region Events
        /// <summary>
        /// The checked changed event.
        /// </summary>
        public event EventHandler<EventArgs<bool?>> CheckedChanged;

        public event EventHandler Clicked;
        #endregion


        private static void OnImageVisiblityChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var item = (BoxViewButton)bindable;
            item.UpdateImageVisibility();
        }

        private void UpdateImageVisibility()
        {
            if (ctlImage.IsVisible != this.IsImageVisible)
            {
                ctlImage.IsVisible = this.IsImageVisible;
            }
        }

        private static void OnButtonEffectChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var button = (BoxViewButton)bindable;
            button.InitColors();
        }

        private void InitColors()
        {
            if (ButtonEffect == BoxViewButtonEffect.HighlightBorderWithTheme
                || ButtonEffect == BoxViewButtonEffect.HighlightBorder
                || ButtonEffect == BoxViewButtonEffect.HighlightDarkLightBorder)
            {
                if (ctlBoxView.BorderThickness <= 0)
                    ctlBoxView.BorderThickness = 1;
            }

            if (ButtonEffect == BoxViewButtonEffect.HighlightBorderWithTheme || ButtonEffect == BoxViewButtonEffect.HighlightFillWithTheme)
            {
                HexColor hColor = new HexColor(App.SettingsViewModel.ThemeColor);
                Color colorTransBorder = hColor;
                Color colorTransFill = hColor;
                if (hColor.A > 100)
                    colorTransFill = Color.FromRgba((int)hColor.R, (int)hColor.G, (int)hColor.B, 40);
                BorderCheckedColor = colorTransBorder;
                FillCheckedColor = colorTransFill;
                BorderUnCheckedColor = Color.FromRgba(0xFF, 0xFF, 0xFF, 0x00);
                if (App.SettingsViewModel.LightTheme)
                {
                    FillUnCheckedColor = Color.FromRgba((int)0, (int)0, (int)0, (int)20);
                    ctlBoxView.Color = FillUnCheckedColor;
                }
                else
                {
                    FillUnCheckedColor = Color.FromRgba((int)255, (int)255, (int)255, (int)20);
                    ctlBoxView.Color = FillUnCheckedColor;
                }

            }
            else if (ButtonEffect == BoxViewButtonEffect.HighlightDarkLightBorder || ButtonEffect == BoxViewButtonEffect.HighlightDarkLightFill)
            {
                if (App.SettingsViewModel.LightTheme)
                {
                    BorderCheckedColor = Color.FromRgba((int)0, (int)0, (int)0, (int)60);
                    BorderUnCheckedColor = Color.FromRgba(0xFF, 0xFF, 0xFF, 0x00);
                    FillUnCheckedColor = Color.FromRgba((int)0, (int)0, (int)0, (int)20);
                    ctlBoxView.Color = FillUnCheckedColor;
                    FillCheckedColor = Color.FromRgba((int)0, (int)0, (int)0, (int)40);
                }
                else
                {
                    BorderCheckedColor = Color.FromRgba((int)255, (int)255, (int)255, (int)60);
                    BorderUnCheckedColor = Color.FromRgba(0xFF, 0xFF, 0xFF, 0x00);
                    FillUnCheckedColor = Color.FromRgba((int)255, (int)255, (int)255, (int)20);
                    ctlBoxView.Color = FillUnCheckedColor;
                    FillCheckedColor = Color.FromRgba((int)255, (int)255, (int)255, (int)40);
                }
            }
        }


        private static void OnCheckedPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var item = (BoxViewButton)bindable;
            item.IsChecked = (bool?)newvalue;
            item.ShowToggleModes();
        }


        private void ShowToggleModes()
        {
            if (IsToggleable && IsChecked.HasValue)
            {
                if (!IsChecked.Value)
                {
                    if (ButtonEffect == BoxViewButtonEffect.HighlightBorderWithTheme
                        || ButtonEffect == BoxViewButtonEffect.HighlightBorder
                        || ButtonEffect == BoxViewButtonEffect.HighlightDarkLightBorder)
                    {
                        ctlBoxView.BorderColor = BorderUnCheckedColor;
                    }
                    else
                    {
                        ctlBoxView.Color = FillUnCheckedColor;
                    }
                }
                else
                {
                    if (ButtonEffect == BoxViewButtonEffect.HighlightBorderWithTheme
                        || ButtonEffect == BoxViewButtonEffect.HighlightBorder
                        || ButtonEffect == BoxViewButtonEffect.HighlightDarkLightBorder)
                    {
                        ctlBoxView.BorderColor = BorderCheckedColor;
                    }
                    else
                    {
                        ctlBoxView.Color = FillCheckedColor;
                    }
                }
            }
        }

        async void OnInternalItemTap(object sender, EventArgs e)
        {
            BoxViewButton element = sender as BoxViewButton;
            if (element != null)
            {
                Clicked?.Invoke(this, new EventArgs());
                if (IsToggleable)
                {
                    if (!this.IsChecked.HasValue)
                        this.IsChecked = true;
                    else
                        this.IsChecked = !this.IsChecked;
                }
                else
                {
                    if (ButtonEffect == BoxViewButtonEffect.HighlightBorderWithTheme
                        || ButtonEffect == BoxViewButtonEffect.HighlightBorder
                        || ButtonEffect == BoxViewButtonEffect.HighlightDarkLightBorder)
                    {
                        ctlBoxView.BorderColor = BorderCheckedColor;
                        await Task.Delay(200);
                        ctlBoxView.BorderColor = BorderUnCheckedColor;
                    }
                    else
                    {
                        ctlBoxView.Color = FillCheckedColor;
                        await Task.Delay(200);
                        ctlBoxView.Color = FillUnCheckedColor;
                    }
                }
            }
        }

        /// <summary>
        /// Xamarin Forms is here buggy, after enabling a button the tapgesturerecognizer doesn't react
        /// </summary>
        /// <param name="isEnabled"></param>
        public void SetButtonEnabled(bool isEnabled)
        {
            bool wasEnabled = this.IsEnabled;
            this.IsEnabled = isEnabled;
            //if (!wasEnabled && isEnabled)
            //{
            //    this.InputTransparent = true;
            //    this.InputTransparent = false;
            //}
            TapGestureRecognizer tapGestureRecognizer = this.GestureRecognizers.Where(c => c is TapGestureRecognizer).FirstOrDefault() as TapGestureRecognizer;
            if (tapGestureRecognizer != null)
            {
                if (wasEnabled && !isEnabled)
                    tapGestureRecognizer.Tapped -= OnInternalItemTap;
                else if (!wasEnabled && isEnabled)
                {
                    //this.GestureRecognizers.Remove(tapGestureRecognizer);
                    //tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += OnInternalItemTap;
                    //this.GestureRecognizers.Add(tapGestureRecognizer);
                }
            }
        }
    }
}
