using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.Controls
{
    public class HideableToolbarItem : ToolbarItem
    {
        public HideableToolbarItem() : base()
        {
            this.InitVisibility();
        }

        private async void InitVisibility()
        {
            await Task.Delay(100);
            OnIsVisibleChanged(this, false, IsVisible);
        }


        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create("IsChecked", typeof(bool?), typeof(HideableToolbarItem), null, BindingMode.TwoWay, null, OnCheckedPropertyChanged);

        //public static BindableProperty IsVisibleProperty = BindableProperty.Create<HideableToolbarItem, bool>(o => o.IsVisible, false, propertyChanged: OnIsVisibleChanged);
        public static BindableProperty IsVisibleProperty = BindableProperty.Create("IsVisible", typeof(bool), typeof(HideableToolbarItem), true, BindingMode.OneWay, null, OnIsVisibleChanged);

        public static readonly BindableProperty IconCheckedProperty = BindableProperty.Create("IconChecked", typeof(FileImageSource), typeof(HideableToolbarItem), null, BindingMode.OneWay);

        public static readonly BindableProperty IconUnCheckedProperty = BindableProperty.Create("IconUnChecked", typeof(FileImageSource), typeof(HideableToolbarItem), null, BindingMode.OneWay);


        /// <summary>
        /// The checked changed event.
        /// </summary>
        public event EventHandler<EventArgs<bool?>> CheckedChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the control is checked.
        /// </summary>
        /// <value>The checked state.</value>
        public bool? IsChecked
        {
            get
            {
                return this.GetValue<bool?>(IsCheckedProperty);
            }

            set
            {
                if (this.IsChecked != value)
                {
                    this.SetValue(IsCheckedProperty, value);
                    if (this.CheckedChanged != null)
                        this.CheckedChanged.Invoke(this, new EventArgs<bool?>(value));
                }
            }
        }

        public FileImageSource IconChecked
        {
            get
            {
                return this.GetValue<FileImageSource>(IconCheckedProperty);
            }
            set
            {
                this.SetValue(IconCheckedProperty, value);
            }
        }

        public FileImageSource IconUnChecked
        {
            get
            {
                return this.GetValue<FileImageSource>(IconUnCheckedProperty);
            }
            set
            {
                this.SetValue(IconUnCheckedProperty, value);
            }
        }

        public bool IsToggleable { get; set; }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        private static void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var item = bindable as HideableToolbarItem;

            if (item.Parent == null)
                return;

            var items = (item.Parent as Page).ToolbarItems;

            if ((bool)newValue && !items.Contains(item))
            {
                items.Add(item);
            }
            else if (!(bool)newValue && items.Contains(item))
            {
                items.Remove(item);
            }
        }

        private static void OnCheckedPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var item = (HideableToolbarItem)bindable;
            item.IsChecked = (bool?)newvalue;
            item.ShowToggleModes();
        }


        protected override void OnClicked()
        {
            if (IsToggleable)
            {
                if (!IsChecked.HasValue)
                    IsChecked = true;
                else
                    IsChecked = !IsChecked;
                ShowToggleModes();
            }
            base.OnClicked();
        }

        private void ShowToggleModes()
        {
            if (IsToggleable && IsChecked.HasValue && IconUnChecked != null && IconChecked != null)
            {
                if (!IsChecked.Value)
                {
                    IconImageSource = IconUnChecked;
                }
                else
                {
                    IconImageSource = IconChecked;
                }
            }
        }
    }
}
