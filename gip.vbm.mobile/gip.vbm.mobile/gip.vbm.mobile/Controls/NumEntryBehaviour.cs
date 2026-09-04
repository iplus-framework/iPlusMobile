using System.Globalization;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile
{
    public class NumEntryBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry || string.IsNullOrEmpty(e.NewTextValue))
                return;

            string decimalSeparator = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
            string wrongSeparator = decimalSeparator == "," ? "." : ",";

            string normalized = e.NewTextValue.Replace(wrongSeparator, decimalSeparator);

            if (normalized != e.NewTextValue)
            {
                // Temporarily detach to avoid re-entrant TextChanged
                entry.TextChanged -= OnTextChanged;
                entry.Text = normalized;
                entry.TextChanged += OnTextChanged;
            }
        }
    }
}