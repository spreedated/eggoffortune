//#define DEBUG_KEEP_LOADINGSCREEN

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
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

                Stopwatch sw = Stopwatch.StartNew();

                await this.LoadConfigurationAsync();
                this.ProgressValue += 0.4d;

                this.LoadScoreService();
                this.ProgressValue += 0.3d;

                await this.LoadStringsFromResources();
                this.ProgressValue += 0.2d;

                sw.Stop();

                this.LogLoadingTime(sw.ElapsedMilliseconds);

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
                _logger.LogError(ex, "Application initialization failed");
                this.IsProgressVisible = false;
                this.Message = "Unable to start. Please restart the app.";
            }
        }

        private async Task LoadConfigurationAsync()
        {
            string confPath = Path.Combine(FileSystem.Current.AppDataDirectory, "config.json");

            this.LogConfigurationInfo(confPath);
            Globals.Configuration = new(new(confPath));
            await Globals.Configuration.LoadAsync();

            _logger.LogTrace("Configuration loaded");
            this.Message = "Configuration loaded...";
        }

        private void LoadScoreService()
        {
            Globals.ScoreService.SetScore(Globals.Configuration.RuntimeConfiguration.TapsLeft);
            this.LogScoreInfo(Globals.Configuration.RuntimeConfiguration.TapsLeft);
            this.Message = "Score service initialized...";
        }

        private async Task LoadStringsFromResources()
        {
            //End text
            Globals.EndText = await Utilities.LoadTextfileAsync("End.txt");

            //Credits text
            DateTime builddate = neXn.Maui.Utilities.GetBuildDate(typeof(EndPagePopupViewModel).Assembly);
            Assembly a = this.GetType().Assembly;

            Globals.EndCreditText = string.Format(await Utilities.LoadTextfileAsync("Credits.txt"),
                a.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkDisplayName,
                builddate == default ? "N/A" : builddate,
                a.GetName().Version.ToString());

            //Last words text
            Globals.LastWords = await Utilities.LoadTextfileAsync("LastWords.txt");

            //License text
            Globals.License = await Utilities.LoadTextfileAsync("License.txt");

            _logger.LogTrace("Strings loaded from resources");
            this.Message = "Strings loaded...";
        }

        #region Logging
        [LoggerMessage(Level = LogLevel.Trace, Message = "Loading completed in {ElapsedMilliseconds} ms")]
        private partial void LogLoadingTime(long elapsedMilliseconds);

        [LoggerMessage(Level = LogLevel.Trace, Message = "Loading configuration from {ConfigPath} ...")]
        private partial void LogConfigurationInfo(string configPath);

        [LoggerMessage(Level = LogLevel.Trace, Message = "Config loaded with {Tapsleft} taps left")]
        private partial void LogScoreInfo(int tapsLeft);
        #endregion
    }
}
