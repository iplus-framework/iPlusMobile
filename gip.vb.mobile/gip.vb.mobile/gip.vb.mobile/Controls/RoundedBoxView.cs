// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using Xamarin.Forms;

namespace gip.vb.mobile.Controls
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

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create("BorderColor", typeof(Color), typeof(RoundedBoxView), Color.White);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
    }
}