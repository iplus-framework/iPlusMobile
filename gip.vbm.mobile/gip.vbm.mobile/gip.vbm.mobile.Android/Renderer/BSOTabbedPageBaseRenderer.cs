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
using gip.vbm.mobile.Views;
using gip.vbm.mobile.Droid;
using Android.Graphics;
using Android.Graphics.Drawables;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

//[assembly: ExportRenderer(typeof(BSOTabbedPageBase), typeof(BSOTabbedPageBaseRenderer))]
namespace gip.vbm.mobile.Droid
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