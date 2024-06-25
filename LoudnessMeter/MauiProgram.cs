using CommunityToolkit.Maui;
using DevExpress.Maui;
using LoudnessMeter.Services;
using LoudnessMeter.ViewModels;
using LoudnessMeter.Views.Pages;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace LoudnessMeter
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseDevExpress()
                .UseDevExpressCharts()
                .UseDevExpressGauges()
                .UseDevExpressCollectionView()
                .UseDevExpressControls()
                .UseSkiaSharp()
                .UseMicrocharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMauiCommunityToolkit();

            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<IAudioCaptureService, BassAudioCaptureService>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
