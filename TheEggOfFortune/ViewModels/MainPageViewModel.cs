using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TheEggOfFortune.Logic;
using TheEggOfFortune.Views;

namespace TheEggOfFortune.ViewModels
{
    internal partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial ContentPage Instance { get; set; }

        [ObservableProperty]
        public partial FluentIcons.Common.Icon ThemeIcon { get; set; } = FluentIcons.Common.Icon.WeatherSunny;

        [ObservableProperty]
        public partial int Tapsleft { get; set; }

        [ObservableProperty]
        public partial string WiseText { get; set; } = null;

        [ObservableProperty]
        public partial bool ShowWiseText { get; set; }

        [ObservableProperty]
        public partial bool ShowEndElements { get; set; } = false;

        [ObservableProperty]
        public partial ImageSource EggImageSource { get; set; } = null;

        [ObservableProperty]
        public partial Image EggImage { get; set; }

        private readonly Timer saveTimer;
        private readonly Timer wiseWordTimer;
        private readonly Microsoft.Extensions.Logging.ILogger logger;

        public MainPageViewModel()
        {
            this.logger = new LoggerFactory().AddSerilog().CreateLogger("MainPage");
            this.Tapsleft = Globals.Configuration.RuntimeConfiguration.TapsLeft;

            Task.Run(async () =>
            {
                if (Globals.Phrases.Count <= 0)
                {
                    foreach (string p in (await Utilities.LoadTextfileAsync("Phrases.txt")).Split('\n').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()))
                    {
                        Globals.Phrases.Add(new() { Text = p });
                    }
                    this.logger.LogTrace("Loaded {Phrasecount} phrases", Globals.Phrases.Count);
                }
            });

            this.saveTimer = new()
            {
                Interval = 5000
            };
            this.wiseWordTimer = new()
            {
                Interval = 3000
            };

            this.wiseWordTimer.Elapsed += (s, e) =>
            {
                this.ShowWiseText = false;
                this.wiseWordTimer.Stop();
            };

            this.saveTimer.Elapsed += async (s, e) =>
            {
                await Globals.Configuration.SaveAsync().ConfigureAwait(false);
            };

            this.saveTimer.Start();

            Task.Run(async () =>
            {
                this.DetermineAndSetEggImage();

                while (this.Instance == null)
                {
                    await Task.Delay(50);
                }

                this.Instance.Dispatcher.Dispatch(() =>
                {
                    this.DetermineAndSetTheme();
                });
            });
        }

        public void ReloadFromConfiguration()
        {
            this.Tapsleft = Globals.Configuration.RuntimeConfiguration.TapsLeft;
        }

        private void DetermineAndSetTheme()
        {
            if (Globals.Configuration.RuntimeConfiguration.Theme)
            {
                this.ThemeIcon = FluentIcons.Common.Icon.WeatherSunny;
                Application.Current.UserAppTheme = AppTheme.Light;
            }
            else
            {
                this.ThemeIcon = FluentIcons.Common.Icon.WeatherMoon;
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
        }

        private void DetermineAndSetEggImage()
        {
            if (Globals.Configuration.RuntimeConfiguration.TapsLeft > 12000)
            {
                this.EggImageSource = Globals.EggstateImage[Globals.EggState.Intact];
            }

            if (Globals.Configuration.RuntimeConfiguration.TapsLeft > 0 && Globals.Configuration.RuntimeConfiguration.TapsLeft <= 12000)
            {
                this.EggImageSource = Globals.EggstateImage[Globals.EggState.Broken];
            }

            this.ShowEndElements = false;

            if (Globals.Configuration.RuntimeConfiguration.TapsLeft <= 0)
            {
                this.ShowEndElements = true;
                this.EggImageSource = Globals.EggstateImage[Globals.EggState.Halfed];
            }
        }

        private void EggAnimation()
        {
            Random rnd = new(BitConverter.ToInt32(Guid.NewGuid().ToByteArray()));

            this.Instance.Dispatcher.Dispatch(async () =>
            {
                await this.EggImage.TranslateToAsync(rnd.Next(-7, 0), 0, (uint)rnd.Next(5, 10));    // Move image left
                await this.EggImage.TranslateToAsync(rnd.Next(-7, 0), rnd.Next(-7, 0), (uint)rnd.Next(5, 10)); // Move image diagonally up and left
                await this.EggImage.TranslateToAsync(rnd.Next(0, 7), rnd.Next(0, 7), (uint)rnd.Next(10, 20));   // Move image diagonally down and right
                await this.EggImage.TranslateToAsync(0, rnd.Next(0, 7), (uint)rnd.Next(5, 10));     // Move image left
                await this.EggImage.TranslateToAsync(0, 0, (uint)rnd.Next(80, 100));       // Move image up
            });
        }

        private void DetermineDisplayWiseText()
        {
            if (Globals.Configuration.RuntimeConfiguration.TapsLeft == 20000)
            {
                this.WiseText = "Your journey has begun...";
                this.ShowWiseText = true;
                this.wiseWordTimer.Start();
                return;
            }

            if (Globals.Configuration.RuntimeConfiguration.TapsLeft <= 0)
            {
                this.ShowWiseText = false;
                this.wiseWordTimer.Stop();
                return;
            }

            if (Globals.Configuration.RuntimeConfiguration.TapsLeft % 100 == 0)
            {
                this.WiseText = LogicLayer.Utilities.GetRandomPhrase(Globals.Phrases);
                this.ShowWiseText = true;
                this.wiseWordTimer.Start();
            }
        }

        [RelayCommand]
        private void OnTap()
        {
            if (Globals.Configuration.RuntimeConfiguration.TapsLeft == 20000)
            {
                this.DetermineDisplayWiseText();
            }

            if (Globals.Configuration.RuntimeConfiguration.TapsLeft <= 0)
            {
                Globals.Configuration.RuntimeConfiguration.TapsLeft = 0;
                this.Tapsleft = Globals.Configuration.RuntimeConfiguration.TapsLeft;
                return;
            }

            Globals.Configuration.RuntimeConfiguration.TapsLeft -= 1;
            this.Tapsleft = Globals.Configuration.RuntimeConfiguration.TapsLeft;

            this.DetermineAndSetEggImage();
            this.EggAnimation();
            this.DetermineDisplayWiseText();
        }

        [RelayCommand]
        private void SwitchThemeMode()
        {
            Globals.Configuration.RuntimeConfiguration.Theme ^= true;
            this.DetermineAndSetTheme();
        }

        [RelayCommand]
        private void QuestionButton()
        {
            this.Instance.ShowPopup(new EndPagePopup());
        }
    }
}
