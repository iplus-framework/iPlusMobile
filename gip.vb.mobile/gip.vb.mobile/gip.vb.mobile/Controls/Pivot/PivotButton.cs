// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using Xamarin.Forms;
using System.Windows.Input;
using gip.vb.mobile.Helpers;

namespace gip.vb.mobile.Controls
{
    public class PivotButtons : StackLayout
    {
    }

    public class PivotButton : Button
    {

        public PivotButton()
        {
            //this.BorderColor = Color.Transparent;
            this.BorderWidth = 0;
            this.BackgroundColor = Color.Transparent;
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

