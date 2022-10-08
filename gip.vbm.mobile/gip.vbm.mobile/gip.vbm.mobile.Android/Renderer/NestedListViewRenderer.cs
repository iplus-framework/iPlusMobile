using System;
using System.ComponentModel;
using Android.Content.Res;
using Android.Graphics;
using gip.vbm.mobile.Droid;
using gip.vbm.mobile.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: ExportRenderer(typeof(NestedListView), typeof(ListViewRenderer))]

namespace gip.vbm.mobile.Droid
{
    /// <summary>
    /// Class NestedListViewRenderer.
    /// </summary>
    public class NestedListViewRenderer : ListViewRenderer
    {
        public NestedListViewRenderer(Android.Content.Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                var listview = this.Control as Android.Widget.ListView;
                NestedListView nestedListView = e.NewElement as NestedListView;
                if (nestedListView != null)
                    listview.NestedScrollingEnabled = nestedListView.IsNestedScroll;
                else
                    listview.NestedScrollingEnabled = true;
            }
        }
    }
}