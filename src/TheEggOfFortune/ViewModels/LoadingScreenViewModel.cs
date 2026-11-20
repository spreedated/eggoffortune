//#define DEBUG_KEEP_LOADINGSCREEN

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using TheEggOfFortune.Logic;

namespace TheEggOfFortune.ViewModels
{
    public partial class LoadingScreenViewModel : ViewModelBase<ContentPage>
    {
        private readonly Timer _minimumLoadTimeTimer = new(2500);
        private bool _timerHit;

        [ObservableProperty]
        public partial string Message { get; set; } = "Loading...";

        [ObservableProperty]
        public partial double ProgressValue { get; set; }

        [ObservableProperty]
        public partial bool IsProgressVisible { get; set; } = true;

        #region Ctor
        public LoadingScreenViewModel() : base(nameof(LoadingScreenViewModel))
        {
            _minimumLoadTimeTimer.Elapsed += (s, e) =>
            {
                _minimumLoadTimeTimer.Stop();
                _timerHit = true;
                _minimumLoadTimeTimer.Dispose();
            };
        }
        #endregion

        public void StartTimerOnLoaded()
        {
            _minimumLoadTimeTimer.Start();
        }

        public async Task Load()
        {
#if DEBUG_KEEP_LOADINGSCREEN
            this.ProgressValue = 0.8d;
            this.Message = "Debug mode: LoadingScreen";
            return;
#endif

            try
            {
#if !DEBUG
                await Task.Delay(500);
#endif

                await this.LoadConfigurationAsync();
                this.ProgressValue += 0.4d;

                this.LoadScoreService();
                this.ProgressValue += 0.4d;

                while (!_timerHit)
                {
                    await Task.Delay(50);
                }

                this.ProgressValue = 1d;
                this.Message = "Experience loaded!";

                await Task.Delay(500);

                this.Instance.Window.Page = new AppShell();
            }
            catch (Exception ex)
            {
                MauiProgram.Logger.LogError(ex, "Application initialization failed");
                this.IsProgressVisible = false;
                this.Message = "Unable to start. Please restart the app.";
            }
        }

        private async Task LoadConfigurationAsync()
        {
            _logger.LogTrace("Loading configuration from {ConfigPath} ...", Path.Combine(FileSystem.Current.AppDataDirectory, "config.json"));
            Globals.Configuration = new(new(Path.Combine(FileSystem.Current.AppDataDirectory, "config.json")));
            await Globals.Configuration.LoadAsync();
            _logger.LogTrace("Configuration loaded");
            this.Message = "Configuration loaded...";
        }

        private void LoadScoreService()
        {
            Globals.ScoreService.SetScore(Globals.Configuration.RuntimeConfiguration.TapsLeft);
            _logger.LogTrace("Config loaded with {Tapsleft} taps left", Globals.Configuration.RuntimeConfiguration.TapsLeft);
            this.Message = "Score service initialized...";
        }
    }
}
