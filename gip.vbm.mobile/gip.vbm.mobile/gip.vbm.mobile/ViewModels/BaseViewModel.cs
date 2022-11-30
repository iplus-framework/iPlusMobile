using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using gip.vbm.mobile.Services;
using gip.mes.webservices;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.webservices;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.ViewModels
{
    public abstract class BaseViewModel : EntityBase
    {
        public IVBWebService _WebService => DependencyService.Get<IVBWebService>() ?? new VBMockService();

        bool _IsBusy = false;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { SetProperty(ref _IsBusy, value); }
        }

        string _Title = string.Empty;
        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }

        IWSResponse _WSResponse;
        public IWSResponse WSResponse
        {
            get { return _WSResponse; }
            set
            {
                SetProperty(ref _WSResponse, value);
                if (_WSResponse != null)
                    Message = _WSResponse.Message;
            }
        }

        Msg _Message = null;
        public Msg Message
        {
            get { return _Message; }
            set
            {
                SetProperty(ref _Message, value);
            }
        }

        public delegate void DialogEventHandler(object sender, Controls.EventArgs<Msg> e);

        public event DialogEventHandler DialogEvent;

        public DialogOptions DialogOptions;

        public void ShowDialog(Msg msg, string title = "", Keyboard keyboard = null, string initialValue = "", short requestID = 0)
        {
            if (keyboard == null)
                keyboard = Keyboard.Default;

            DialogOptions.DialogPrompKeyboard = keyboard;
            DialogOptions.DialogPromptInitialValue = initialValue;
            DialogOptions.DialogTitle = title;
            DialogOptions.RequestID = requestID;
            DialogOptions.RequestMsg = msg;

            if (DialogEvent != null)
                DialogEvent(this, new Controls.EventArgs<Msg>(msg));
        }

        public abstract void DialogResponse(Global.MsgResult result, string enteredValue = null);

        public void SetTitleFromType(Type bsoType, VBUserRights userRights)
        {
            Type thisType = this.GetType();
            if (userRights.Menu != null)
            {
                var vbMenu = userRights.Menu.Where(c => c.DestPage == bsoType).FirstOrDefault();
                if (vbMenu != null)
                {
                    Title = vbMenu.Label;
                    return;
                }
            }
            Title = thisType.Name;
        }
    }

    public struct DialogOptions
    {
        public short RequestID
        {
            get;
            set;
        }

        public Msg RequestMsg
        {
            get;
            set;
        }

        public string DialogTitle
        {
            get;
            set;
        }

        public string DialogPromptInitialValue
        {
            get;
            set;
        }

        public Keyboard DialogPrompKeyboard
        {
            get;
            set;
        }
    }
}
