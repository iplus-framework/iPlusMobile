using System;
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
