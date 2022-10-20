using CommunityToolkit.Maui.Views;
using ZXing.Net.Maui;

namespace gip.vbm.mobile.Controls;

public partial class BarcodePopup : Popup
{
	public BarcodePopup()
	{
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
            Task.Run(() =>
            {
                Close(_Results);
            });
        }
    }

    void OnOKButtonClicked(object sender, EventArgs e)
    {
        Close(_Results);
    }
}