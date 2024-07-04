using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace gip.vb.mobile.Controls
{
    public class ExtendedDatePicker : DatePicker
    {
        public ExtendedDatePicker()
        {
        }

        public event EventHandler DateOk;

        public event EventHandler DateCancel;

        public void OnDateSelected()
        {
            DateOk.Invoke(this, new EventArgs());
        }

        public void OnDateCanceled()
        {
            DateCancel.Invoke(this, new EventArgs());
        }
    }
}
