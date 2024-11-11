// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.vb.mobile.Droid;
using gip.vb.mobile.Controls;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Views;

//[assembly: ExportRenderer(typeof(ExtendedViewCell), typeof(ExtendedViewCellRenderer))]

//namespace gip.vb.mobile.Droid
//{
//    /// <summary>
//    /// Class SeparatorRenderer.
//    /// </summary>
//    public class ExtendedViewCellRenderer : ViewCellRenderer
//    {
//        private Android.Views.View _cellCore;
//        private Drawable _unselectedBackground;
//        private bool _selected;

//        protected override global::Android.Views.View GetCellCore(Cell item, global::Android.Views.View convertView, ViewGroup parent, Android.Content.Context context)
//        {
//            _cellCore = base.GetCellCore(item, convertView, parent, context);

//            _selected = false;
//            _unselectedBackground = _cellCore.Background;

//            return _cellCore;
//        }

//        protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
//        {
//            base.OnCellPropertyChanged(sender, args);

//            if (args.PropertyName == "IsSelected")
//            {
//                _selected = !_selected;

//                if (_selected)
//                {
//                    var extendedViewCell = sender as ExtendedViewCell;
//                    _cellCore.SetBackgroundColor(extendedViewCell.SelectedBackgroundColor.ToAndroid());
//                }
//                else
//                {
//                    _cellCore.SetBackground(_unselectedBackground);
//                }
//            }
//        }
//    }
//}

