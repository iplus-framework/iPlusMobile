using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Controls
{
    /// <summary>
    /// The RoundedBoxView
    /// </summary>
    public class RoundedBoxView : BoxView
	{
        public static readonly BindableProperty RCornerRadiusProperty = BindableProperty.Create("RCornerRadius", typeof(double), typeof(RoundedBoxView), default(double));

        public double RCornerRadius
        {
            get { return (double)GetValue(RCornerRadiusProperty); }
            set { SetValue(RCornerRadiusProperty, value); }
        }

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create("BorderThickness", typeof(int), typeof(RoundedBoxView), default(int));

        public int BorderThickness
        {
            get { return (int)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create("BorderColor", typeof(Color), typeof(RoundedBoxView), Colors.White);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
    }
}