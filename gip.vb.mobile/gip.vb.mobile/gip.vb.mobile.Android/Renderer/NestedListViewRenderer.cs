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

[assembly: ExportRenderer(typeof(NestedListView), typeof(ListViewRenderer))]

namespace gip.vb.mobile.Droid
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