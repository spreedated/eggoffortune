using CommunityToolkit.Maui;
using FluentIcons.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Serilog;
using Serilog.Events;

namespace TheEggOfFortune
{
    public static class MauiProgram
    {
        public static Microsoft.Extensions.Logging.ILogger Logger { get; private set; }

        private static void SetupSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Is(LogEventLevel.Verbose)
                .WriteTo.Debug()
                .CreateLogger();

            Logger = new LoggerFactory().AddSerilog().CreateLogger("App");
        }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            SetupSerilog();

            builder
                .UseMauiApp<App>()
                .UseFluentIcons()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Doom2016Text-GOlBq.ttf", "Doom");
                    fonts.AddFont("AgencyFB-Bold.ttf", "Agency");
                });

            builder.Logging.AddSerilog();

            return builder.Build();
        }
    }
}
