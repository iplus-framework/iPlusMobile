using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using gip.vb.mobile.Helpers;
#if DATALOGIC
using Com.Datalogic.Decode;
using Com.Datalogic.Device;
#endif
using Android.Util;
using gip.vb.mobile.barcode;
using Xamarin.Forms;

namespace gip.vb.mobile.Droid
{
    [Activity(Label = "iPlus Mobile", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
#if DATALOGIC
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IReadListener
    {
        BarcodeManager _DLBarcodeManager = null;
#else
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    { 
#endif
        public MainActivity()
        {
            Forms.SetFlags("SwipeView_Experimental");
        }

    //Com.Datalogic.Device.Nfc.NfcManager _DLNfcManager = null;
    DatalogicBarcodeService _DLBarcodeService = null;

        private readonly string LOGTAG = typeof(MainActivity).Name;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            XResourceLoader.SetResourceLoader(new MyXResourceLoader());
            XLocalSettings.SetLocalSettings(new MyXLocalSettings());

            _DLBarcodeService = Xamarin.Forms.DependencyService.Get<IBarcodeService>() as DatalogicBarcodeService;
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            LoadApplication(new App());
        }


        public override void OnRequestPermissionsResult(int requestCode,
        string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();

#if DATALOGIC
            // If the decoder instance is null, create it.
            if (_DLBarcodeManager == null)
            {
                // Remember an onPause call will set it to null.
                _DLBarcodeManager = new BarcodeManager();
            }

            //if (_DLNfcManager == null)
            //{
            //    _DLNfcManager = new Com.Datalogic.Device.Nfc.NfcManager();
            //    _DLNfcManager.EnableNfcAdapter(true);
            //}

            // From here on, we want to be notified with exceptions in case of errors.
            ErrorManager.EnableExceptions(true);

            try
            {
                // add our class as a listener
                _DLBarcodeManager.AddReadListener(this);
            }
            catch (DecodeException e)
            {
                Log.Error(LOGTAG, "Error while trying to bind a listener to BarcodeManager", e);
            }
#endif
        }

        protected override void OnPause()
        {
            base.OnPause();

#if DATALOGIC
            // If we have an instance of BarcodeManager.
            if (_DLBarcodeManager != null)
            {
                try
                {
                    // Unregister our listener from it and free resources
                    _DLBarcodeManager.RemoveReadListener(this);
                }
                catch (Exception e)
                {
                    Log.Error(LOGTAG, "Error while trying to remove a listener from BarcodeManager", e);
                }
            }
#endif
        }

#if DATALOGIC
        /// <summary>
        /// onRead method will be called when a good read event is fired.
        /// </summary>
        /// <param name="p0">To be added.</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>
        /// <para tool="javadoc-to-mdoc">onRead method will be called when a good read event is fired.</para>
        /// <para tool="javadoc-to-mdoc">
        ///   <format type="text/html">
        ///     <a href="http://datalogic.github.io/android-sdk-docs/reference/com/datalogic/decode/ReadListener.html#onRead(com.datalogic.decode.DecodeResult)" target="_blank">[Android Documentation]</a>
        ///   </format>
        /// </para>
        /// </remarks>
        /// <altmember cref="T:Com.Datalogic.Decode.IDecodeResult" />
        public void OnRead(IDecodeResult p0)
        {
            // https://datalogic.github.io/xamarin/overview
            if (_DLBarcodeService != null)
                _DLBarcodeService.OnRead(p0, this);
        }
#endif
    }
}