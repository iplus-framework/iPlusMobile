﻿using gip.vbm.mobile.Droid;
using gip.vbm.mobile.Controls;
using System.ComponentModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: ExportRenderer(typeof(Separator), typeof(SeparatorRenderer))]

namespace gip.vbm.mobile.Droid
{
	/// <summary>
	/// Class SeparatorRenderer.
	/// </summary>
	public class SeparatorRenderer : ViewRenderer<Separator, SeparatorDroidView>
	{
        public SeparatorRenderer(Android.Content.Context context) : base(context)
        {
        }

        /// <summary>
        /// Called when [element changed].
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Separator> e)
		{
			base.OnElementChanged (e);

			if (e.NewElement == null)
			{
				return;
			}

			if (this.Control == null)
			{
				this.SetNativeControl(new SeparatorDroidView(this.Context));
			}

			this.SetProperties();
		}


		/// <summary>
		/// Handles the <see cref="E:ElementPropertyChanged" /> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			this.SetProperties();
		}

		/// <summary>
		/// Sets the properties.
		/// </summary>
		private void SetProperties()
		{
			Control.SpacingBefore = Element.SpacingBefore;
			Control.SpacingAfter = Element.SpacingAfter;
			Control.Thickness = Element.Thickness;
			Control.StrokeColor = Element.Color.ToAndroid();
			Control.StrokeType = Element.StrokeType;
			Control.Orientation = Element.Orientation;

			this.Control.Invalidate();
		}
	}
}

