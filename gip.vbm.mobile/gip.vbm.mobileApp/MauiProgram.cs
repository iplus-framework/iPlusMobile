using gip.vbm.mobile;
using gip.vbm.mobile.Helpers;
using ZXing.Net.Maui;

namespace gip.vbm.mobileApp
{
    public static class MauiProgram
    {
        static DatalogicBarcodeService _DLBarcodeService = null;
        public static MauiApp CreateMauiApp()
        {
            XResourceLoader.SetResourceLoader(new MyXResourceLoader());
            XLocalSettings.SetLocalSettings(new MyXLocalSettings());

            _DLBarcodeService = new DatalogicBarcodeService(); // Xamarin.Forms.DependencyService.Get<IBarcodeService>() as DatalogicBarcodeService;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            return builder.Build();
        }
    }
}