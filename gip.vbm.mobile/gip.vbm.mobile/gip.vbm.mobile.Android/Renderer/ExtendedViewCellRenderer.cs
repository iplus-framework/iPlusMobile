using gip.vbm.mobile.Droid;
using gip.vbm.mobile.Controls;
using System.ComponentModel;
using Android.Graphics.Drawables;
using Android.Views;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

//[assembly: ExportRenderer(typeof(ExtendedViewCell), typeof(ExtendedViewCellRenderer))]

//namespace gip.vbm.mobile.Droid
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

