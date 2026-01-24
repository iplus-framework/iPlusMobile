using CommunityToolkit.Maui.Views;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace gip.vbm.mobile.Controls;

//public class MyCameraBarcodeReaderView : CameraBarcodeReaderView
//{
//    public CameraBarcodeReaderViewHandler CameraHandler => base.Handler as CameraBarcodeReaderViewHandler;

//    public void DeactivateCamera()
//    {
//        //CameraHandler.dis
//    }
//}

public partial class BarcodePopup : Popup
{
	public BarcodePopup()
	{
        //CameraBarcodeReaderView

        InitializeComponent();
        scanView.Options = new BarcodeReaderOptions
        {
            AutoRotate = false,
            TryHarder = true,
            Formats = BarcodeFormat.Code128 | BarcodeFormat.Code39 | BarcodeFormat.Ean13 | BarcodeFormat.Ean8 | BarcodeFormat.QrCode,
            Multiple = true
        };
    }

    public static BindableProperty AutoCloseOnScanProperty = BindableProperty.Create(
         propertyName: nameof(AutoCloseOnScan),
         returnType: typeof(bool),
         declaringType: typeof(BarcodePopup),
         defaultValue: true,
         defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Option to automatically close barcode scanner window
    /// </summary>
    public bool AutoCloseOnScan
    {
        get
        {
            return (bool)base.GetValue(AutoCloseOnScanProperty);
        }
        set
        {
            base.SetValue(AutoCloseOnScanProperty, value);
        }
    }

    private BarcodeResult[] _Results;

    private void scanView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        if (e == null)
            _Results = null;
        _Results = e.Results;
        if (AutoCloseOnScan)
        {
            Task.Run(async () =>
            {
                //scanView?.DisconnectCamera();
                await CloseAsync();
            });
        }
    }

    async void OnOKButtonClicked(object sender, EventArgs e)
    {
        //scanView?.DisconnectCamera();
        await CloseAsync();
    }
}