using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using gip.vb.mobile.DependencyServices;
using gip.vb.mobile.Droid.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(UtilityAndroid))]
namespace gip.vb.mobile.Droid.DependencyServices
{
    public class UtilityAndroid : IUtility
    {
        public void CloseApp()
        {
            Process.KillProcess(Process.MyPid());
        }

        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }


    }
}