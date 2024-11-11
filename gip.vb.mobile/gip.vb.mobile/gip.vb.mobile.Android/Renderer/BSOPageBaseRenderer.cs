// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using gip.vb.mobile.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Android.Graphics.Drawables;
using gip.vb.mobile.Views;

//[assembly: ExportRenderer(typeof(BSOPageBase), typeof(BSOPageBaseRenderer))]
namespace gip.vb.mobile.Droid
{
    //public class BSOPageBaseRenderer : PageRenderer
    //{
    //    public BSOPageBaseRenderer(Android.Content.Context context) : base(context)
    //    {
    //    }

    //    protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
    //    {
    //        base.OnElementChanged(e);

    //        if (e.OldElement != null || Element == null)
    //        {
    //            return;
    //        }

    //        try
    //        {
    //            bool lightTheme = gip.vb.mobile.App.SettingsViewModel.LightTheme;
    //            Bitmap bmp = null;
    //            if (lightTheme)
    //            {
    //                this.SetBackgroundColor(Android.Graphics.Color.White);
    //                bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.BallBackground);
    //            }
    //            else
    //            {
    //                this.SetBackgroundColor(Android.Graphics.Color.Black);
    //                bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.BallBackgroundDark);
    //            }

    //            BitmapDrawable bitmapDrawable = new BitmapDrawable(Resources, bmp);
    //            bitmapDrawable.SetTileModeXY(Shader.TileMode.Repeat, Shader.TileMode.Repeat);
    //            this.Background = bitmapDrawable;
    //        }
    //        catch (Exception)
    //        {
    //            //System.Diagnostics.Debug.WriteLine(@"          ERROR: ", ex.Message);
    //        }
    //    }

    //}
}