using System;
using System.Windows.Input;
using gip.vbm.mobile.Helpers;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Controls
{
    public class PivotItem : StackLayout { }

    /// <summary>
    /// http://www.codeguru.com/virtual-mobile/a-tab-control-in-xamarin.html
    /// </summary>
    public class Pivot : StackLayout
    {
        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create("SelectedColor", typeof(Color), typeof(Pivot), Color.White, BindingMode.Default);
        public Color SelectedColor
        {
            get
            {
                return this.GetValue<Color>(SelectedColorProperty);
            }

            set
            {
                this.SetValue(SelectedColorProperty, value);
            }
        }

        public static readonly BindableProperty UnselectedColorProperty = BindableProperty.Create("UnselectedColor", typeof(Color), typeof(Pivot), Color.Silver, BindingMode.Default);
        public Color UnselectedColor
        {
            get
            {
                return this.GetValue<Color>(UnselectedColorProperty);
            }

            set
            {
                this.SetValue(UnselectedColorProperty, value);
            }
        }

        //public static readonly BindableProperty BorderColorProperty = BindableProperty.Create("BorderColor", typeof(Color), typeof(Pivot), Color.Silver, BindingMode.Default);
        //public Color BorderColor
        //{
        //    get
        //    {
        //        return this.GetValue<Color>(BorderColorProperty);
        //    }

        //    set
        //    {
        //        this.SetValue(BorderColorProperty, value);
        //    }
        //}

        internal List<PivotButton> TabButtons
        {
            get
            {
                PivotButtons tabButtons = (PivotButtons)Children.First(c => c.GetType() == typeof(PivotButtons));
                return tabButtons.Children.Select(c => (PivotButton)c).Where(c => c.GetType() == typeof(PivotButton)).ToList();
            }
        }

        internal List<PivotItem> Tabs
        {
            get
            {
                return Children.Where(c => c.GetType() == typeof(PivotItem)).Select(c => (PivotItem)c).ToList();
            }
        }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                _selectedTabIndex = value;

                if (Tabs.Count > 0)
                    SelectionUIUpdate();
            }
        }

        public PivotButton SelectedTabButton
        {
            get { return TabButtons[_selectedTabIndex]; }
            set
            {
                var tabIndex = TabButtons.FindIndex(t =>
                   t == value);
                if (tabIndex == -1)
                    throw new Exception(
                       "SelectedTabButton assigned a button " +
                       "that isn't among the children  of " +
                       "PivotButtons.");

                if (tabIndex != _selectedTabIndex)
                    SelectedTabIndex = tabIndex;
            }
        }

        public PivotItem SelectedTab
        {
            get { return Tabs[_selectedTabIndex]; }
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (TabButtons.Count != Tabs.Count)
            {
                throw new Exception(
                   "The number of tab buttons and the " +
                   "number of tabs to not match.");
            }

            SelectionUIUpdate();
        }

        private void SelectionUIUpdate()
        {
            foreach (var btn in TabButtons)
            {
                btn.TextColor = UnselectedColor;
                btn.BorderWidth = 0;
            }
            SelectedTabButton.TextColor = SelectedColor;
            SelectedTabButton.BorderColor = SelectedColor;
            SelectedTabButton.BorderWidth = 2;


            foreach (var tb in Tabs)
            {
                tb.IsVisible = false;
            }
            SelectedTab.IsVisible = true;
            if (SelectionChanged != null)
            {
                SelectionChanged.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler SelectionChanged;
    }

}

