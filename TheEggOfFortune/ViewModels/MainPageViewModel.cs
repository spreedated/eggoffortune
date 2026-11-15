using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TheEggOfFortune.Logic;
using TheEggOfFortune.Views;

namespace TheEggOfFortune.ViewModels
{
    internal partial class MainPageViewModel : ViewModelBase<ContentPage>
    {
        [ObservableProperty]
        public partial FluentIcons.Common.Icon ThemeIcon { get; set; } = FluentIcons.Common.Icon.WeatherSunny;

        [ObservableProperty]
        public partial int Tapsleft { get; set; }

        [ObservableProperty]
        public partial string WiseText { get; set; }

        [ObservableProperty]
        public partial bool ShowWiseText { get; set; }

        [ObservableProperty]
        public partial bool ShowEndElements { get; set; }

        [ObservableProperty]
        public partial ImageSource EggImageSource { get; set; }

        [ObservableProperty]
        public partial Image EggImage { get; set; }

        private readonly Timer _saveTimer;
        private readonly Timer _wiseWordTimer;

        public MainPageViewModel() : base(nameof(MainPageViewModel))
        {
            this.Tapsleft = Globals.Configuration.RuntimeConfiguration.TapsLeft;

            Task.Run(async () =>
            {
                if (Globals.Phrases.Count <= 0)
                {
                    foreach (string p in (await Utilities.LoadTextfileAsync("Phrases.txt")).Split('\n').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()))
                    {
                        Globals.Phrases.Add(new() { Text = p });
                    }
                    if (_logger.IsEnabled(LogLevel.Trace))
                    {
                        _logger.LogTrace("Loaded {Phrasecount} phrases", Globals.Phrases.Count);
                    }
                }
            });

            _saveTimer = new()
            {
                Interval = 5000
            };
            _wiseWordTimer = new()
            {
                Interval = 3000
            };

            _wiseWordTimer.Elapsed += (s, e) =>
            {
                this.ShowWiseText = false;
                _wiseWordTimer.Stop();
            };

            _saveTimer.Elapsed += async (s, e) =>
            {
                try
                {
                    await Globals.SaveConfigurationAsync(skipIfBusy: true).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Autosave failed. Recent progress may not be saved.");
                }
            };

            _saveTimer.Start();

            Task.Run(async () =>
            {
                this.DetermineAndSetEggImage();

                while (base.Instance == null)
                {
                    await Task.Delay(50);
                }

                base.Instance.Dispatcher.Dispatch(() =>
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

            base.Instance.Dispatcher.Dispatch(async () =>
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
                _wiseWordTimer.Start();
                return;
            }

            if (Globals.Configuration.RuntimeConfiguration.TapsLeft <= 0)
            {
                this.ShowWiseText = false;
                _wiseWordTimer.Stop();
                return;
            }

            if (Globals.Configuration.RuntimeConfiguration.TapsLeft % 100 == 0)
            {
                this.WiseText = LogicLayer.Utilities.GetRandomPhrase(Globals.Phrases);
                this.ShowWiseText = true;
                _wiseWordTimer.Start();
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
            base.Instance.ShowPopup(new EndPagePopup());
        }
    }
}
