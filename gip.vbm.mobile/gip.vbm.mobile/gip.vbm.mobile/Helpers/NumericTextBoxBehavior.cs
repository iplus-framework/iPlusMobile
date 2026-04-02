// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Globalization;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Helpers
{
    public class NumericTextBoxBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnTextChanged;
            entry.Completed += Entry_Completed;
            entry.Focused += Entry_Focused;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnTextChanged;
            entry.Completed -= Entry_Completed;
            entry.Focused -= Entry_Focused;
            base.OnDetachingFrom(entry);
        }

        private void Entry_Focused(object sender, FocusEventArgs e)
        {
            if (SelectAllOnFocus && sender is Entry ent)
            {
                Application.Current?.Dispatcher.Dispatch(() =>
                {
                    ent.CursorPosition = 0;
                    ent.SelectionLength = ent.Text != null ? ent.Text.Length : 0;
                });
            }
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
                entry.TextChanged -= OnTextChanged;
                entry.Text = normalized;
                entry.TextChanged += OnTextChanged;
            }
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            if (sender is not Entry tb)
                return;

            CultureInfo culture = CultureInfo.CurrentUICulture;

            if (Precision > 0)
            {
                if (IsDecimal)
                {
                    decimal decValue = 0;

                    if (string.IsNullOrEmpty(tb.Text)
                        || decimal.TryParse(tb.Text, NumberStyles.Any, culture, out decValue))
                    {
                        _lastText = tb.Text;
                        decimal rounded = Math.Round(decValue, Precision);
                        if (rounded != decValue)
                        {
                            _lastText = rounded.ToString(culture);
                            tb.Text = _lastText;
                        }
                        return;
                    }
                }
                else
                {
                    double dblValue = 0;

                    if (string.IsNullOrEmpty(tb.Text)
                        || double.TryParse(tb.Text, NumberStyles.Any, culture, out dblValue))
                    {
                        _lastText = tb.Text;
                        double rounded = Math.Round(dblValue, Precision);
                        if (rounded != dblValue)
                        {
                            _lastText = rounded.ToString(culture);
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
                    if (string.IsNullOrEmpty(tb.Text)
                        || long.TryParse(tb.Text, NumberStyles.Any, culture, out _))
                    {
                        _lastText = tb.Text;
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(tb.Text)
                        || double.TryParse(tb.Text, NumberStyles.Any, culture, out _))
                    {
                        _lastText = tb.Text;
                        return;
                    }
                }
            }

            tb.Text = _lastText;
        }

        private string _lastText;

        public bool IsDecimal { get; set; }
        public int Precision { get; set; }
        public bool SelectAllOnFocus { get; set; }
    }
}