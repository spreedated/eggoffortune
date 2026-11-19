using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogicLayer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TheEggOfFortune.Logic;
using TheEggOfFortune.Views;

namespace TheEggOfFortune.ViewModels
{
    internal partial class MainPageViewModel : ViewModelBase<ContentPage>
    {
        private readonly Timer _saveTimer;
        private readonly Timer _wiseWordTimer;

        #region Observable Properties
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
        #endregion

        private void DetermineAndSetTheme()
        {
            DeveloperService.AddTap(DeveloperService.TapCode.Theme);

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
            if (Globals.ScoreService.Score > 12000)
            {
                this.EggImageSource = Globals.EggstateImage[Globals.EggState.Intact];
            }

            if (Globals.ScoreService.Score > 0 && Globals.ScoreService.Score <= 12000)
            {
                this.EggImageSource = Globals.EggstateImage[Globals.EggState.Broken];
            }

            this.ShowEndElements = false;

            if (Globals.ScoreService.Score <= 0)
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
            if (Globals.ScoreService.Score == 20000)
            {
                this.WiseText = "Your journey has begun...";
                this.ShowWiseText = true;
                _wiseWordTimer.Start();
                return;
            }

            if (Globals.ScoreService.Score <= 0)
            {
                this.ShowWiseText = false;
                _wiseWordTimer.Stop();
                return;
            }

            if (Globals.ScoreService.Score % 100 == 0)
            {
                List<Phrase> selectedPhraseCategory = null;

                if (Globals.ScoreService.Score <= 20000 && Globals.ScoreService.Score >= 19000)
                {
                    selectedPhraseCategory = Globals.PhrasesInitial;
                }

                if (Globals.ScoreService.Score <= 18999 && Globals.ScoreService.Score >= 12000)
                {
                    selectedPhraseCategory = Globals.PhrasesRoutine;
                }

                if (Globals.ScoreService.Score <= 11999 && Globals.ScoreService.Score >= 6000)
                {
                    selectedPhraseCategory = Globals.PhrasesImpatience;
                }

                if (Globals.ScoreService.Score <= 5999 && Globals.ScoreService.Score >= 2000)
                {
                    selectedPhraseCategory = Globals.PhrasesDoubt;
                }

                if (Globals.ScoreService.Score <= 1999)
                {
                    selectedPhraseCategory = Globals.PhrasesReflection;
                }

                this.WiseText = LogicLayer.Utilities.GetRandomPhrase(selectedPhraseCategory);

                this.ShowWiseText = true;
                _wiseWordTimer.Start();
            }
        }

        #region Ctor
        public MainPageViewModel() : base(nameof(MainPageViewModel))
        {
            this.Tapsleft = Globals.ScoreService.Score;

            Globals.ScoreService.ScoreChanged += (s, e) =>
            {
                this.Tapsleft = Globals.ScoreService.Score;
                Globals.Configuration.RuntimeConfiguration.TapsLeft = Globals.ScoreService.Score;

                this.DetermineAndSetEggImage();
                this.DetermineDisplayWiseText();
            };

            Task.Run(async () =>
            {
                if (Globals.PhrasesAll.Count <= 0)
                {
                    foreach (string p in (await Utilities.LoadTextfileAsync("Phrases.txt")).Split('\n').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()))
                    {
                        Globals.PhrasesAll.Add(new() { Text = p });
                    }

                    Globals.PhrasesInitial.AddRange(Globals.PhrasesAll.Skip(0).Take(60));
                    Globals.PhrasesRoutine.AddRange(Globals.PhrasesAll.Skip(60).Take(47));
                    Globals.PhrasesImpatience.AddRange(Globals.PhrasesAll.Skip(107).Take(38));
                    Globals.PhrasesDoubt.AddRange(Globals.PhrasesAll.Skip(145).Take(78));
                    Globals.PhrasesReflection.AddRange(Globals.PhrasesAll.Skip(223));

                    if (_logger.IsEnabled(LogLevel.Trace))
                    {
                        _logger.LogTrace("Loaded {Phrasecount} phrases", Globals.PhrasesAll.Count);
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
        #endregion

        [RelayCommand]
        private async Task OnTap()
        {
            if (Globals.ScoreService.Score == 20000)
            {
                this.DetermineDisplayWiseText();
            }

            if (Globals.ScoreService.Score <= 0)
            {
                Globals.ScoreService.SetScore(0);
                await Globals.Configuration.SaveAsync().ConfigureAwait(false);
                return;
            }

            this.EggAnimation();

            Globals.ScoreService.SubtractScore();

            DeveloperService.AddTap(DeveloperService.TapCode.Egg);
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
