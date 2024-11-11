// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Windows.Input;

namespace gip.vb.mobile.Controls
{
    public interface ITapableControl
    {
        event EventHandler<ItemTappedEventArgs> ItemTapped;

        ICommand Command { get; set; }
    }
}
