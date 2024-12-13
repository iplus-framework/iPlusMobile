// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Controls
{
	public class MsgLabel : Label
	{
        public static readonly BindableProperty MsgProperty = BindableProperty.Create("Msg", typeof(Msg), typeof(MsgLabel), null, BindingMode.OneWay, null, OnMsgPropertyChanged);

		public Msg Msg
        {
			get
			{
				return this.GetValue<Msg>(MsgProperty);
			}

			set
			{
				if (this.Msg != value)
                {
					this.SetValue(MsgProperty, value);
				}
			}
		}

		private static void OnMsgPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
            MsgLabel msgLabel = (MsgLabel) bindable;
            if (msgLabel != null)
            {
                if (msgLabel.Msg == null)
                {
                    msgLabel.IsVisible = false;
                    msgLabel.Text = null;
                }
                else
                {
                    if (msgLabel.Msg.MessageLevel < eMsgLevel.Info)
                    {
                        msgLabel.BackgroundColor = Colors.Green;
                        msgLabel.TextColor = Colors.Black;
                    }
                    else if (msgLabel.Msg.MessageLevel < eMsgLevel.Warning)
                    {
                        msgLabel.BackgroundColor = Colors.Lime;
                        msgLabel.TextColor = Colors.Black;
                    }
                    else if (msgLabel.Msg.MessageLevel == eMsgLevel.Warning)
                    {
                        msgLabel.BackgroundColor = Colors.Yellow;
                        msgLabel.TextColor = Colors.Black;
                    }
                    else
                    {
                        msgLabel.BackgroundColor = Colors.Red;
                        msgLabel.TextColor = Colors.Black;
                    }
                    msgLabel.Text = msgLabel.Msg.Message;
                    msgLabel.IsVisible = true;
                }
            }
		}
	}
}