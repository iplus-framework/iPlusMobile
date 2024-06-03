using Android.App;
using Android.Content.PM;
using Android.OS;
#if DATALOGIC
using Com.Datalogic.Decode;
using Com.Datalogic.Device;
#elif ZEBRA
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using static Symbol.XamarinEMDK.Barcode.StatusData;
#endif
using Android.Util;
using gip.vbm.mobile;
using gip.vbm.mobile.barcode;
using gip.vbm.mobile.Helpers;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobileApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
#if DATALOGIC
    public class MainActivity : MauiAppCompatActivity, IReadListener
    {
        BarcodeManager _DLBarcodeManager = null;
#elif ZEBRA
    public class MainActivity : MauiAppCompatActivity, EMDKManager.IEMDKListener
    {
        EMDKManager _EMDKManager = null;
        ProfileManager _ProfileManager = null;
        private String _ProfileName = "ClockProfile";
        BarcodeManager _BarcodeManager = null;
        Scanner _Scanner = null;
#else
    public class MainActivity : MauiAppCompatActivity
    {
#endif
        public MainActivity()
        {
        }

        DatalogicBarcodeService _DLBarcodeService = null;
        private readonly string LOGTAG = typeof(MainActivity).Name;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            _DLBarcodeService = DependencyService.Get<IBarcodeService>() as DatalogicBarcodeService;
            //ZXing.Net.Mobile.Forms.Android.Platform.Init();
#if ZEBRA
            try
            {
                EMDKResults results = EMDKManager.GetEMDKManager(Android.App.Application.Context, this);
                if (results.StatusCode != EMDKResults.STATUS_CODE.Success)
                    Log.Debug(LOGTAG, "Status: EMDKManager object creation failed ...");
                else
                    Log.Debug(LOGTAG, "Status: EMDKManager object creation succeeded ...");
            }
            catch (Exception ex)
            {
                Log.Error(LOGTAG, ex.Message);
            }
#endif

            base.OnCreate(savedInstanceState);
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
#elif ZEBRA
            InitScanner();
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
#elif ZEBRA
            DeinitScanner();
#endif
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
#if ZEBRA
            if (_ProfileManager != null)
            {
                _ProfileManager = null;
            }

            if (_EMDKManager != null)
            {
                _EMDKManager.Release();
                _EMDKManager = null;
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
#elif ZEBRA

        public void OnClosed()
        {
            if (_EMDKManager != null)
            {
                _EMDKManager.Release();
                _EMDKManager = null;
            }
        }

        public void OnOpened(EMDKManager p0)
        {
            this._EMDKManager = p0;
            InitScanner();

            //try
            //{
            //    _ProfileManager = (ProfileManager)_EMDKManager.GetInstance(EMDKManager.FEATURE_TYPE.Profile);
            //}
            //catch (Exception e)
            //{
            //}
        }

        //void ApplyProfile()
        //{
        //    if (_ProfileManager != null)
        //    {
        //        EMDKResults results = _ProfileManager.ProcessProfile(_ProfileName, ProfileManager.PROFILE_FLAG.Set, new String[] { "" });
        //        if (results.StatusCode == EMDKResults.STATUS_CODE.Success)
        //        {
        //            Log.Debug(LOGTAG, "Status: Profile applied successfully ...");
        //        }
        //        else if (results.StatusCode == EMDKResults.STATUS_CODE.CheckXml)
        //        {
        //            //Inspect the XML response to see if there are any errors, if not report success
        //            using (XmlReader reader = XmlReader.Create(new StringReader(results.StatusString)))
        //            {
        //                String checkXmlStatus = "Status:\n\n";
        //                while (reader.Read())
        //                {
        //                    switch (reader.NodeType)
        //                    {
        //                        case XmlNodeType.Element:
        //                            switch (reader.Name)
        //                            {
        //                                case "parm-error":
        //                                    checkXmlStatus += "Parm Error:\n";
        //                                    checkXmlStatus += reader.GetAttribute("name") + " - ";
        //                                    checkXmlStatus += reader.GetAttribute("desc") + "\n\n";
        //                                    break;
        //                                case "characteristic-error":
        //                                    checkXmlStatus += "characteristic Error:\n";
        //                                    checkXmlStatus += reader.GetAttribute("type") + " - ";
        //                                    checkXmlStatus += reader.GetAttribute("desc") + "\n\n";
        //                                    break;
        //                            }
        //                            break;
        //                    }
        //                }

        //                if (checkXmlStatus == "Status:\n\n")
        //                {
        //                    Log.Debug(LOGTAG, "Status: Profile applied successfully ...");
        //                }
        //                else
        //                {
        //                    Log.Debug(LOGTAG, checkXmlStatus);
        //                }

        //            }
        //        }
        //        else
        //        {
        //            Log.Debug(LOGTAG, "Status: Profile initialization failed ... " + results.StatusCode);
        //        }
        //    }
        //    else
        //    {
        //        Log.Debug(LOGTAG, "Status: _ProfileManager is null ...");
        //    }
        //}

        public void InitScanner()
        {
            // https://techdocs.zebra.com/home/dev/
            // https://techdocs.zebra.com/emdk-for-xamarin/9-0/guide/about/
            // https://techdocs.zebra.com/emdk-for-xamarin/8-0/guide/maui/
            if (_EMDKManager != null)
            {
                if (_BarcodeManager == null)
                {
                    try
                    {

                        //Get the feature object such as BarcodeManager object for accessing the feature.
                        _BarcodeManager = (BarcodeManager)_EMDKManager.GetInstance(EMDKManager.FEATURE_TYPE.Barcode);

                        _Scanner = _BarcodeManager.GetDevice(BarcodeManager.DeviceIdentifier.Default);

                        if (_Scanner != null)
                        {

                            //Attahch the Data Event handler to get the data callbacks.
                            _Scanner.Data += _Scanner_Data; ;

                            //Attach Scanner Status Event to get the status callbacks.
                            _Scanner.Status += _Scanner_Status; ;

                            _Scanner.Enable();

                            //EMDK: Configure the _Scanner settings
                            ScannerConfig config = _Scanner.GetConfig();
                            config.SkipOnUnsupported = ScannerConfig.SkipOnUnSupported.None;
                            config.ScanParams.DecodeLEDFeedback = true;
                            config.ReaderParams.ReaderSpecific.ImagerSpecific.PickList = ScannerConfig.PickList.Enabled;
                            config.DecoderParams.Code39.Enabled = true;
                            config.DecoderParams.Code128.Enabled = false;
                            _Scanner.SetConfig(config);

                        }
                        else
                        {
                            Log.Debug(LOGTAG, "Failed to enable _Scanner.\n");
                        }
                    }
                    catch (ScannerException e)
                    {
                        Log.Debug(LOGTAG, "Error: " + e.Message);
                    }
                    catch (Exception ex)
                    {
                        Log.Debug(LOGTAG, "Error: " + ex.Message);
                    }
                }

            }
        }

        void DeinitScanner()
        {
            if (_EMDKManager != null)
            {

                if (_Scanner != null)
                {
                    try
                    {

                        _Scanner.Data -= _Scanner_Data;
                        _Scanner.Status -= _Scanner_Status;

                        _Scanner.Disable();
                    }
                    catch (ScannerException e)
                    {
                        Log.Debug(this.Class.SimpleName, "Exception:" + e.Result.Description);
                    }
                }

                if (_BarcodeManager != null)
                {
                    _EMDKManager.Release(EMDKManager.FEATURE_TYPE.Barcode);
                }
                _BarcodeManager = null;
                _Scanner = null;
            }
        }

        private void _Scanner_Status(object sender, Scanner.StatusEventArgs e)
        {
            if (_DLBarcodeService != null)
                _DLBarcodeService.Scanner_Status(sender, e);
            String statusStr = "";

            //EMDK: The status will be returned on multiple cases. Check the state and take the action.
            StatusData.ScannerStates state = e.P0.State;

            if (state == StatusData.ScannerStates.Idle)
            {
                statusStr = "Scanner is idle and ready to submit read.";
                try
                {
                    if (_Scanner != null && _Scanner.IsEnabled && !_Scanner.IsReadPending)
                    {
                        _Scanner.Read();
                    }
                }
                catch (ScannerException e1)
                {
                    statusStr = e1.Message;
                }
            }
            if (state == StatusData.ScannerStates.Waiting)
            {
                statusStr = "Waiting for Trigger Press to scan";
            }
            if (state == StatusData.ScannerStates.Scanning)
            {
                statusStr = "Scanning in progress...";
            }
            if (state == StatusData.ScannerStates.Disabled)
            {
                statusStr = "Scanner disabled";
            }
            if (state == StatusData.ScannerStates.Error)
            {
                statusStr = "Error occurred during scanning";
            }
            Log.Debug(LOGTAG, statusStr);
        }

        private void _Scanner_Data(object sender, Scanner.DataEventArgs e)
        {
            if (_DLBarcodeService != null)
                _DLBarcodeService.Scanner_Data(sender, e);
        }
#endif
    }
}