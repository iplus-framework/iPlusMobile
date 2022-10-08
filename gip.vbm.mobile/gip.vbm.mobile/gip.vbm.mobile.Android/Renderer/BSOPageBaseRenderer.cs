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
using gip.vbm.mobile.Droid;
using Android.Graphics;
using Android.Graphics.Drawables;
using gip.vbm.mobile.Views;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

//[assembly: ExportRenderer(typeof(BSOPageBase), typeof(BSOPageBaseRenderer))]
namespace gip.vbm.mobile.Droid
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
    //            bool lightTheme = gip.vbm.mobile.App.SettingsViewModel.LightTheme;
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