using CommunityToolkit.Maui;
using MauiIcons.Fluent;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;
using Serilog;
using Serilog.Events;
using System.IO;
using TheEggOfFortune.Logic;

namespace TheEggOfFortune
{
    public static class MauiProgram
    {
        public static Microsoft.Extensions.Logging.ILogger AppLogger { get; private set; }
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseFluentMauiIcons()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Doom2016Text-GOlBq.ttf", "Doom");
                    fonts.AddFont("AgencyFB-Bold.ttf", "Agency");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Is(LogEventLevel.Verbose)
                .WriteTo.Debug()
                .CreateLogger();

            AppLogger = new LoggerFactory().AddSerilog().CreateLogger("App");

            Globals.Configuration = new(new(Path.Combine(FileSystem.Current.AppDataDirectory, "config.json")));
            Globals.Configuration.Load();
            AppLogger.LogTrace("Config loaded with \"{Tapsleft}\" taps left", Globals.Configuration.RuntimeConfiguration.TapsLeft);

            return builder.Build();
        }
    }
}
