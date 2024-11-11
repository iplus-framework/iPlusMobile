// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using Xamarin.Forms;

namespace gip.vb.mobile.Controls
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
                        msgLabel.BackgroundColor = Color.Green;
                        msgLabel.TextColor = Color.Black;
                    }
                    else if (msgLabel.Msg.MessageLevel < eMsgLevel.Warning)
                    {
                        msgLabel.BackgroundColor = Color.Lime;
                        msgLabel.TextColor = Color.Black;
                    }
                    else if (msgLabel.Msg.MessageLevel == eMsgLevel.Warning)
                    {
                        msgLabel.BackgroundColor = Color.Yellow;
                        msgLabel.TextColor = Color.Black;
                    }
                    else
                    {
                        msgLabel.BackgroundColor = Color.Red;
                        msgLabel.TextColor = Color.Black;
                    }
                    msgLabel.Text = msgLabel.Msg.Message;
                    msgLabel.IsVisible = true;
                }
            }
		}
	}
}