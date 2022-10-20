using System;
using System.Windows.Input;
using gip.vbm.mobile.Helpers;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Controls
{
    public class PivotButtons : StackLayout
    {
    }

    public class PivotButton : Button
    {

        public PivotButton()
        {
            //this.BorderColor = Color.FromRgba(0xFF, 0xFF, 0xFF, 0x00);
            this.BorderWidth = 0;
            this.BackgroundColor = Color.FromRgba(0xFF, 0xFF, 0xFF, 0x00);
            Clicked += ThisTabButtonClicked;
        }

        public void ThisTabButtonClicked(object s, EventArgs e)
        {
            Pivot prnt = validParentPivot();
            if (prnt == null)
                return;
            prnt.SelectedTabButton = this;
        }

        private Pivot validParentPivot()
        {
            // Work your way up to the grandparent; parent should be
            // PivotButtons and grandparent should be Pivot
            if (Parent != null && Parent.Parent != null && Parent.Parent.GetType() == typeof(Pivot))
                return ((Pivot)Parent.Parent);
            else
            {
                throw new Exception(
                   "Grandparent of a PivotButton " +
                   "must be a Pivot");
            }
        }
    }
}

