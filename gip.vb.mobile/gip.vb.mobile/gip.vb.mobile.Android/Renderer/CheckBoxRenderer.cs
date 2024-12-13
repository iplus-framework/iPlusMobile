// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.ComponentModel;
using Android.Content.Res;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using gip.vb.mobile.Droid;
using gip.vb.mobile.Controls;

[assembly: ExportRenderer(typeof(gip.vb.mobile.Controls.CheckBox), typeof(gip.vb.mobile.Droid.CheckBoxRenderer))]

namespace gip.vb.mobile.Droid
{
    /// <summary>
    /// Class CheckBoxRenderer.
    /// </summary>
    public class CheckBoxRenderer : ViewRenderer<gip.vb.mobile.Controls.CheckBox, Android.Widget.CheckBox>
    {
        public CheckBoxRenderer(Android.Content.Context context) : base(context)
        {
        }

        private ColorStateList defaultTextColor;
        
        /// <summary>
        /// Called when [element changed].
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<gip.vb.mobile.Controls.CheckBox> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null)
            {
                var checkBox = new Android.Widget.CheckBox(this.Context);
                checkBox.CheckedChange += CheckBoxCheckedChange;

                defaultTextColor = checkBox.TextColors;
                this.SetNativeControl(checkBox);
            }

            Control.Text = e.NewElement.Text;
            Control.Checked = e.NewElement.IsChecked;
            UpdateTextColor();

            if (e.NewElement.FontSize > 0)
            {
                Control.TextSize = (float)e.NewElement.FontSize;
            }

            if (!string.IsNullOrEmpty(e.NewElement.FontName))
            {
                Control.Typeface = TrySetFont(e.NewElement.FontName);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:ElementPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case "IsChecked":
                    Control.Text = Element.Text;
                    Control.Checked = Element.IsChecked;
                    break;
                case "TextColor":
                    UpdateTextColor();
                    break;
                case "FontName":
                    if (!string.IsNullOrEmpty(Element.FontName))
                    {
                        Control.Typeface = TrySetFont(Element.FontName);
                    }
                    break;
                case "FontSize":
                    if (Element.FontSize > 0)
                    {
                        Control.TextSize = (float)Element.FontSize;
                    }
                    break;
                case "CheckedText":
                case "UncheckedText":
                    Control.Text = Element.Text;
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Property change for {0} has not been implemented.", e.PropertyName);
                    break;
            }
        }

        /// <summary>
        /// CheckBoxes the checked change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Android.Widget.CompoundButton.CheckedChangeEventArgs"/> instance containing the event data.</param>
        void CheckBoxCheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs e)
        {
            this.Element.IsChecked = e.IsChecked;
        }

        /// <summary>
        /// Tries the set font.
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <returns>Typeface.</returns>
        private Typeface TrySetFont(string fontName)
        {
            Typeface tf = Typeface.Default;
            try
            {
                tf = Typeface.CreateFromAsset(Context.Assets, fontName);
                return tf;
            }
            catch (Exception ex)
            {
                Console.Write("not found in assets {0}", ex);
                try
                {
                    tf = Typeface.CreateFromFile(fontName);
                    return tf;
                }
                catch (Exception ex1)
                {
                    Console.Write(ex1);
                    return Typeface.Default;
                }
            }
        }

        /// <summary>
        /// Updates the color of the text
        /// </summary>
        private void UpdateTextColor()
        {
            if (Control == null || Element == null)
                return;

            if (Element.TextColor == Xamarin.Forms.Color.Default)
                Control.SetTextColor(defaultTextColor);
            else
                Control.SetTextColor(Element.TextColor.ToAndroid());
        }
    }
}