using Mapsui.UI.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Estadios_de_futbol;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp() 
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        return builder.Build();
    }
}






//using Microsoft.Extensions.Logging;

//namespace Estadios_de_futbol
//{
//    public static class MauiProgram
//    {
//        public static MauiApp CreateMauiApp()
//        {
//            var builder = MauiApp.CreateBuilder();
//            builder
//                .UseMauiApp<App>()
//                .ConfigureFonts(fonts =>
//                {
//                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
//                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
//                });

//#if DEBUG
//            builder.Logging.AddDebug();
//#endif

//            return builder.Build();
//        }
//    }
//}