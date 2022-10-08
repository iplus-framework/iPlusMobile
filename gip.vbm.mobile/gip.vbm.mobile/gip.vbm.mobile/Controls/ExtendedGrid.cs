using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Controls
{
    public class ExtendedGrid : Grid
    {
        public ExtendedGrid() : base()
        {

        }

        public static readonly BindableProperty VisualStateProperty =
            BindableProperty.Create("VisualState",
                                    typeof(string),
                                    typeof(ExtendedGrid),
                                    "");

        public string VisualState
        {
            get { return (string)GetValue(VisualStateProperty); }
            set { SetValue(VisualStateProperty, value); }
        }
    }
}
