using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using gip.vb.mobile.Services;
using gip.mes.webservices;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.webservices;
using System.Linq;

namespace gip.vb.mobile.ViewModels
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

        public void ShowDialog(Msg msg)
        {
            if (DialogEvent != null)
                DialogEvent(this, new Controls.EventArgs<Msg>(msg));
        }

        public abstract void DialogResponse(Global.MsgResult result, string entredValue = null);

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
}
