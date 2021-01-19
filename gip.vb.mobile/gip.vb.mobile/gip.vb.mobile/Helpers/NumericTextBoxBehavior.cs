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
            base.OnAttachedTo(entry);
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
                if (AllowDecimal)
                {
                    if (IsDecimal)
                    {
                        Decimal value = 0;
                        if (string.IsNullOrEmpty(tb.Text) ||
                            Decimal.TryParse(tb.Text, out value))
                        {
                            _lastText = tb.Text;
                            if (Precision >= 0)
                            {
                                Decimal roundedValue = Math.Round(value, Precision);
                                if (roundedValue != value)
                                {
                                    _lastText = roundedValue.ToString();
                                    tb.Text = _lastText;
                                    //tb.SelectionStart = tb.Text.Length;
                                }
                            }
                            return;
                        }
                    }
                    else
                    {
                        double value = 0;
                        if (string.IsNullOrEmpty(tb.Text) ||
                            Double.TryParse(tb.Text, out value))
                        {
                            _lastText = tb.Text;
                            if (Precision >= 0)
                            {
                                double roundedValue = Math.Round(value, Precision);
                                if (roundedValue != value)
                                {
                                    _lastText = roundedValue.ToString();
                                    tb.Text = _lastText;
                                    //tb.SelectionStart = tb.Text.Length;
                                }
                            }
                            return;
                        }
                    }
                }
                else
                {
                    long value;
                    if (string.IsNullOrEmpty(tb.Text) ||
                        long.TryParse(tb.Text, out value))
                    {
                        _lastText = tb.Text;
                        return;
                    }
                }

                tb.Text = _lastText;
                //tb.SelectionStart = tb.Text.Length;
            }

            //double result;
            //bool isValid = double.TryParse(args.NewTextValue, out result);
            //((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
        }

        private string _lastText;
        public bool AllowDecimal { get; set;  }

        public bool IsDecimal { get; set; }

        public int Precision { get; set; }
    }
}
