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
using gip.vb.mobile.Views;
using gip.vb.mobile.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Android.Graphics.Drawables;

//[assembly: ExportRenderer(typeof(BSOTabbedPageBase), typeof(BSOTabbedPageBaseRenderer))]
namespace gip.vb.mobile.Droid
{
    //public class BSOTabbedPageBaseRenderer : PageRenderer
    //{
    //    protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
    //    {
    //        base.OnElementChanged(e);

    //        if (e.OldElement != null || Element == null)
    //        {
    //            return;
    //        }

    //        try
    //        {
    //            Bitmap bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.BallBackground);
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