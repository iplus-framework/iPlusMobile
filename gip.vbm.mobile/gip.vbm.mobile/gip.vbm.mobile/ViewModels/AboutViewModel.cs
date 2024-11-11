// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using gip.core.datamodel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "iPlus Mobile";

            OpenWebCommand = new Command( async () => await Launcher.Default.OpenAsync(new Uri("https://iplus-framework.com")));
            SendPerfLog = new Command(async () => await ExecuteSendPerfLog());
        }

        public ICommand OpenWebCommand { get; }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }

        public bool PerfLoggingOn
        {
            get
            {
                return App.SettingsViewModel.PerfLoggingOn;
            }
            set
            {
                App.SettingsViewModel.PerfLoggingOn = value;
                App.PerfLogger.Active = value;
                OnPropertyChanged();
            }
        }

        public Command SendPerfLog { get; set; }
        public async Task ExecuteSendPerfLog()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                if (!App.PerfLogger.Active)
                    return;
                string dump = App.PerfLogger.DumpAndReset();
                var response = await _WebService.DumpPerfLog(dump);

                if (response.Suceeded)
                {
                    if (response.Message != null)
                    {
                        Message = response.Message;
                    }
                }
                else if (response.Message != null)
                {
                    Message = response.Message;
                }
            }
            catch (Exception e)
            {
                Message = new Msg(eMsgLevel.Exception, e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}