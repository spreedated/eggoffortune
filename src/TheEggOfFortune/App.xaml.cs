using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using TheEggOfFortune.Logic;

namespace TheEggOfFortune
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        //TODO: Add a splashscreen with my Name etc. and a loading animation while the config is being loaded
        protected override Window CreateWindow(IActivationState activationState)
        {
            Label message = new()
            {
                Text = "Loading saved progress…",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center
            };

            ContentPage loadingPage = new() { Content = message };
            Window window = new(loadingPage);

            loadingPage.Loaded += OnLoaded;
            return window;

            async void OnLoaded(object sender, EventArgs e)
            {
                loadingPage.Loaded -= OnLoaded;

                try
                {
                    await Globals.Configuration.LoadAsync();
                    Globals.ScoreService.SetScore(Globals.Configuration.RuntimeConfiguration.TapsLeft);
                    if (MauiProgram.Logger.IsEnabled(LogLevel.Trace))
                    {
                        MauiProgram.Logger.LogTrace("Config loaded with {Tapsleft} taps left", Globals.Configuration.RuntimeConfiguration.TapsLeft);
                    }

                    window.Page = new AppShell();
                }
                catch (Exception ex)
                {
                    MauiProgram.Logger.LogError(ex, "Application initialization failed");
                    message.Text = "Unable to start. Please restart the app.";
                }
            }
        }
    }
}
