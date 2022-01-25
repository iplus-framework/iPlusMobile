using System;
using System.Globalization;
using Xamarin.Forms;

namespace gip.vb.mobile.Helpers
{
    public class NumericTextBoxBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            //entry.TextChanged += OnEntryTextChanged;
            entry.Completed += Entry_Completed;
            entry.Focused += Entry_Focused;
            base.OnAttachedTo(entry);
        }

        private void Entry_Focused(object sender, FocusEventArgs e)
        {
            if (SelectAllOnFocus)
            {
                Entry ent = sender as Entry;
                if (ent != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ent.CursorPosition = 0;
                        ent.SelectionLength = ent.Text != null ? ent.Text.Length : 0;
                    });
                }
            }
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            //entry.TextChanged -= OnEntryTextChanged;
            entry.Completed -= Entry_Completed;
            base.OnDetachingFrom(entry);
        }

        //void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        private void Entry_Completed(object sender, EventArgs e)
        {
            Entry tb = sender as Entry;
            if (tb != null)
            {
                if (Precision > 0)
                {
                    if (IsDecimal)
                    {
                        Decimal value = 0;
                        if (   string.IsNullOrEmpty(tb.Text)
                            || Decimal.TryParse(tb.Text, out value))
                        {
                            _lastText = tb.Text;
                            Decimal roundedValue = Math.Round(value, Precision);
                            if (roundedValue != value)
                            {
                                _lastText = roundedValue.ToString();
                                tb.Text = _lastText;
                            }
                            return;
                        }
                    }
                    else
                    {
                        double value = 0;
                        if (   string.IsNullOrEmpty(tb.Text)
                            || Double.TryParse(tb.Text, out value))
                        {
                            _lastText = tb.Text;
                            double roundedValue = Math.Round(value, Precision);
                            if (roundedValue != value)
                            {
                                _lastText = roundedValue.ToString();
                                tb.Text = _lastText;
                            }
                            return;
                        }
                    }
                }
                else
                {
                    if (IsDecimal)
                    {
                        long value;
                        if (   string.IsNullOrEmpty(tb.Text)
                            || long.TryParse(tb.Text, out value))
                        {
                            _lastText = tb.Text;
                            return;
                        }
                    }
                    else
                    {
                        double value;
                        if (   string.IsNullOrEmpty(tb.Text)
                            || double.TryParse(tb.Text, out value))
                        {
                            _lastText = tb.Text;
                            return;
                        }
                    }
                }

                tb.Text = _lastText;
            }

            //double result;
            //bool isValid = double.TryParse(args.NewTextValue, out result);
            //((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
        }

        private string _lastText;
        public bool AllowDecimal { get; set;  }

        public bool IsDecimal { get; set; }

        public int Precision { get; set; }

        public bool SelectAllOnFocus
        {
            get;
            set;
        }
    }
}
