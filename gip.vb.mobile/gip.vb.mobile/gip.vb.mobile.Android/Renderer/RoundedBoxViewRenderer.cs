using System;
using System.ComponentModel;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using gip.vb.mobile.Droid;
using View = Android.Views.View;
using gip.vb.mobile.Controls;
using Android.Graphics.Drawables;
using Android.OS;

[assembly: ExportRenderer(typeof(RoundedBoxView), typeof(RoundedBoxViewRenderer))]

namespace gip.vb.mobile.Droid
{
    /// <summary>
    /// Class CheckBoxRenderer.
    /// </summary>
    public class RoundedBoxViewRenderer : ViewRenderer<RoundedBoxView, View>
    {
        public RoundedBoxViewRenderer(Android.Content.Context context) : base(context)
        {
        }

        public static void Init()
        {
        }

        private RoundedBoxView _formControl
        {
            get { return Element; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<RoundedBoxView> e)
        {
            base.OnElementChanged(e);

            this.InitializeFrom(_formControl);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            this.UpdateFrom(_formControl, e.PropertyName);
        }

        public void InitializeFrom(RoundedBoxView formsControl)
        {
            if (formsControl == null)
                return;

            var background = new GradientDrawable();
            background.SetColor(formsControl.Color.ToAndroid());
            Background = background;

            UpdateCornerRadius(formsControl.RCornerRadius);
            UpdateBorder(formsControl.BorderColor, formsControl.BorderThickness);
        }

        public void UpdateFrom(RoundedBoxView formsControl, string propertyChanged)
        {
            if (formsControl == null)
                return;

            if (propertyChanged == RoundedBoxView.RCornerRadiusProperty.PropertyName)
            {
                UpdateCornerRadius(formsControl.RCornerRadius);
            }
            //if (propertyChanged == VisualElement.BackgroundColorProperty.PropertyName)
            //{
            //    var background = Background as GradientDrawable;

            //    if (background != null)
            //    {
            //        background.SetColor(formsControl.BackgroundColor.ToAndroid());
            //    }
            //}
            if (propertyChanged == RoundedBoxView.ColorProperty.PropertyName)
            {
                var background = Background as GradientDrawable;

                if (background != null)
                {
                    background.SetColor(formsControl.Color.ToAndroid());
                }
            }

            if (propertyChanged == RoundedBoxView.BorderColorProperty.PropertyName)
            {
                UpdateBorder(formsControl.BorderColor, formsControl.BorderThickness);
            }

            if (propertyChanged == RoundedBoxView.BorderThicknessProperty.PropertyName)
            {
                UpdateBorder(formsControl.BorderColor, formsControl.BorderThickness);
            }
        }

        public void UpdateBorder(Xamarin.Forms.Color color, int thickness)
        {
            var backgroundGradient = Background as GradientDrawable;

            if (backgroundGradient != null)
            {
                var relativeBorderThickness = thickness * 3;
                backgroundGradient.SetStroke(relativeBorderThickness, color.ToAndroid());
            }
        }

        public void UpdateCornerRadius(double cornerRadius)
        {
            var backgroundGradient = Background as GradientDrawable;

            if (backgroundGradient != null)
            {
                var relativeCornerRadius = (float)(cornerRadius * 3.7);
                backgroundGradient.SetCornerRadius(relativeCornerRadius);
            }
        }

    }
}