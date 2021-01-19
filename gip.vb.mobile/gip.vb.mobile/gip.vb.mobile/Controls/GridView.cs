using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace gip.vb.mobile.Controls
{
    /// <summary>
    /// GridView
    /// </summary>
    public class GridView : Layout<View>
    {
        public GridView()
        {
            // https://github.com/kiwaa/Xamarin.Forms.GridView/blob/master/GridView/GridView/GridView.cs
            // https://gist.github.com/NicoVermeir/7ffb34ebd639ed958382  http://www.spikie.be/blog/post/2015/04/02/.aspx
            // https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/ListView.cs
            // http://xfcomplete.net/xamarin.forms/2016/01/13/creating-custom-layouts-with-xamarinforms/
            // https://github.com/conceptdev/xamarin-forms-samples/blob/master/Evolve13/Evolve13/Controls/WrapLayout.cs
            // https://chaseflorell.github.io/xamarin/2015/03/14/xamarinforms-gridview/
            //InputTransparent = true;
        }

        #region Properties
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(GridView), default(IEnumerable), BindingMode.OneWay, null, OnItemsSourceChanged);
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var view = (GridView)bindable;
            view.ReCreateChildrens();
        }

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create("ItemTemplate", typeof(DataTemplate), typeof(GridView), default(DataTemplate), BindingMode.OneWay, null, OnItemsTemplateChanged);
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        private static void OnItemsTemplateChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var view = (GridView)bindable;
            view.ReCreateChildrens();
        }

        public static readonly BindableProperty MaxItemsPerRowProperty = BindableProperty.Create("MaxItemsPerRow", typeof(int), typeof(GridView), default(int), BindingMode.OneWay, null, OnSizePropertiesChanged);
        public int MaxItemsPerRow
        {
            get { return (int)GetValue(MaxItemsPerRowProperty); }
            set { SetValue(MaxItemsPerRowProperty, value); }
        }

        public static readonly BindableProperty ItemHeightProperty = BindableProperty.Create("ItemHeight", typeof(int), typeof(GridView), default(int), BindingMode.OneWay, null, OnSizePropertiesChanged);
        public int ItemHeight
        {
            get { return (int)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly BindableProperty RowSpacingProperty = BindableProperty.Create("RowSpacing", typeof(double), typeof(GridView), default(double), BindingMode.OneWay, null, OnSizePropertiesChanged);
        public double RowSpacing
        {
            get { return (double)GetValue(RowSpacingProperty); }
            set { SetValue(RowSpacingProperty, value); }
        }

        public static readonly BindableProperty ColumnSpacingProperty = BindableProperty.Create("ColumnSpacing", typeof(double), typeof(GridView), default(double), BindingMode.OneWay, null, OnSizePropertiesChanged);
        public double ColumnSpacing
        {
            get { return (double)GetValue(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(GridView), null);
        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        private static void OnSizePropertiesChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var view = (GridView)bindable;
            view.ReCreateChildrens();
        }
        #endregion

        #region Events
        public event EventHandler<ItemTappedEventArgs> ItemTapped;
        #endregion


        #region Methods
        private void ReCreateChildrens()
        {
            if (ItemsSource == null || ItemTemplate == null)
                return;

            if (Children.Any())
            {
                if (this.ItemTapped != null)
                {
                    foreach (var item in Children)
                    {
                        var tapRecognizer = item.GestureRecognizers.FirstOrDefault() as TapGestureRecognizer;
                        if (tapRecognizer != null)
                            tapRecognizer.Tapped -= OnInternalItemTap;
                    }
                }
                Children.Clear();
            }

            foreach (var item in ItemsSource)
            {
                var viewCell = ItemTemplate.CreateContent() as ViewCell;
                if (viewCell != null && viewCell.View != null)
                {
                    if (this.ItemTapped != null)
                    {
                        ITapableControl tapCtl = viewCell.View as ITapableControl;
                        if (tapCtl != null)
                        {
                            tapCtl.ItemTapped += OnInternalItemTap;
                        }
                        else
                        {
                            var tapRecognizer = new TapGestureRecognizer();
                            tapRecognizer.Tapped += OnInternalItemTap;
                            viewCell.View.GestureRecognizers.Add(tapRecognizer);
                        }
                    }
                    if (Command != null)
                    {
                        ITapableControl tapCtl = viewCell.View as ITapableControl;
                        if (tapCtl != null)
                        {
                            tapCtl.Command = Command;
                        }
                        else
                        {
                            var tapRecognizer = new TapGestureRecognizer();
                            tapRecognizer.Command = this.Command;
                            tapRecognizer.CommandParameter = item;
                            viewCell.View.GestureRecognizers.Add(tapRecognizer);
                        }
                    }
                    viewCell.View.BindingContext = item;

                    Children.Add(viewCell.View);
                }
            }

            InvalidateLayout();
        }


        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            var colWidth = width / MaxItemsPerRow;
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                if (!child.IsVisible)
                    continue;

                var virtualColumn = i % MaxItemsPerRow;
                var virtualRow = i / MaxItemsPerRow;

                var rowSpacing = (virtualRow != 0) ? RowSpacing : 0;
                var colSpacing = (virtualColumn != 0) ? ColumnSpacing : 0;

                var childX = x + colWidth * virtualColumn;
                var childY = y + (ItemHeight + rowSpacing) * virtualRow;
                LayoutChildIntoBoundingRegion(child, new Rectangle(childX, childY, (colWidth - ColumnSpacing), ItemHeight));
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            // Check our cache for existing results
            SizeRequest cachedResult;
            var constraintSize = new Size(widthConstraint, heightConstraint);
            if (_measureCache.TryGetValue(constraintSize, out cachedResult))
            {
                return cachedResult;
            }

            var height = 0.0;
            var minHeight = 0.0;
            var width = 0.0;
            var minWidth = 0.0;

            var visibleChildrensCount = (double)Children.Count(c => c.IsVisible);
            var rowsCount = Math.Ceiling(visibleChildrensCount / MaxItemsPerRow);
            height = minHeight = (ItemHeight + RowSpacing) * rowsCount - RowSpacing;
            width = minWidth = widthConstraint;

            // store our result in the cache for next time
            var result = new SizeRequest(new Size(width, height), new Size(minWidth, minHeight));
            _measureCache[constraintSize] = result;
            return result;
        }

        protected override void InvalidateMeasure()
        {
            _measureCache.Clear();
            base.InvalidateMeasure();
        }

        protected override void InvalidateLayout()
        {
            _measureCache.Clear();
            base.InvalidateLayout();
        }

        protected override void OnChildMeasureInvalidated()
        {
            _measureCache.Clear();
            base.OnChildMeasureInvalidated();
        }

        readonly Dictionary<Size, SizeRequest> _measureCache = new Dictionary<Size, SizeRequest>();

        protected override void OnAdded(View view)
        {
            base.OnAdded(view);
        }

        protected override void OnRemoved(View view)
        {
            base.OnRemoved(view);
        }

        void OnInternalItemTap(object sender, EventArgs e)
        {
            View element = sender as View;
            if (element != null)
            {
                ItemTapped?.Invoke(this, new ItemTappedEventArgs(element, element.BindingContext));
            }
        }
        #endregion

    }

}