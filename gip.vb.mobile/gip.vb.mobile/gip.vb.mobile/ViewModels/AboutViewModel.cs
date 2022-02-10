using System;
using System.Windows.Input;
using gip.core.datamodel;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "iPlus Mobile";

            OpenWebCommand = new Command( async () => await Xamarin.Essentials.Launcher.OpenAsync(new Uri("https://iplus-framework.com")));
        }

        public ICommand OpenWebCommand { get; }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }
    }
}