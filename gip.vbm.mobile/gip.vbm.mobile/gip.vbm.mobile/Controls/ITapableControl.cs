// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Controls
{
    public interface ITapableControl
    {
        event EventHandler<ItemTappedEventArgs> ItemTapped;

        ICommand Command { get; set; }
    }
}
