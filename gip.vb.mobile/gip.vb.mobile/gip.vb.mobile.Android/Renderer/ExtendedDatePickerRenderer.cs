// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.ComponentModel;
using Android.Content.Res;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using gip.vb.mobile.Droid;
using gip.vb.mobile.Controls;
using Android.App;
using Android.Content;

[assembly: ExportRenderer(typeof(gip.vb.mobile.Controls.ExtendedDatePicker), typeof(ExtendedDatePickerRenderer))]

namespace gip.vb.mobile.Droid
{
    /// <summary>
    /// Class ExtendedDatePickerRenderer.
    /// </summary>
    public class ExtendedDatePickerRenderer : DatePickerRenderer
    {
        public ExtendedDatePickerRenderer(Android.Content.Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);

            //Disposing
            if (e.OldElement != null)
            {
                _element = null;
            }

            //Creating
            if (e.NewElement != null)
            {
                _element = e.NewElement as ExtendedDatePicker;
            }
        }

        protected gip.vb.mobile.Controls.ExtendedDatePicker _element;

        protected override DatePickerDialog CreateDatePickerDialog(int year, int month, int day)
        {
            var dialog = new DatePickerDialog(Context, (o, e) =>
            {
                _element.Date = e.Date;
                ((IElementController)_element).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
            }, year, month, day);

            dialog.SetButton((int)DialogButtonType.Positive, Context.Resources.GetString(global::Android.Resource.String.Ok), OnOk);
            dialog.SetButton((int)DialogButtonType.Negative, Context.Resources.GetString(global::Android.Resource.String.Cancel), OnCancel);

            return dialog;
        }

        private void OnCancel(object sender, DialogClickEventArgs e)
        {
            _element.OnDateCanceled();
            _element.Unfocus();
        }
        private void OnOk(object sender, DialogClickEventArgs e)
        {
            //need to set date from native control manually now
            _element.Date = ((DatePickerDialog)sender).DatePicker.DateTime;
            _element.OnDateSelected();
            _element.Unfocus();
        }

    }
}