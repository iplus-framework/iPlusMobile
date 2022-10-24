using CommunityToolkit.Maui;
using gip.vbm.mobile;
using gip.vbm.mobile.Helpers;
using ZXing.Net.Maui.Controls;

namespace gip.vbm.mobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            XResourceLoader.SetResourceLoader(new MyXResourceLoader());
            XLocalSettings.SetLocalSettings(new MyXLocalSettings());

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            return builder.Build();
        }
    }
}