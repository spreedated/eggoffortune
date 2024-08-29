using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiIcons.Fluent;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Timers;
using TheEggOfFortune.Logic;
using TheEggOfFortune.Views;

namespace TheEggOfFortune.ViewModels
{
    internal partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ContentPage instance;

        [ObservableProperty]
        private FluentIcons themeIcon = FluentIcons.WeatherSunny48;

        [ObservableProperty]
        private int tapsleft;

        [ObservableProperty]
        private string wiseText = "Some wise\nwords\nHello World!";

        [ObservableProperty]
        private bool showWiseText;

        [ObservableProperty]
        private bool showEndElements = false;

        [ObservableProperty]
        private ImageSource eggImageSource = null;

        [ObservableProperty]
        private Image eggImage;

        private readonly Timer saveTimer;
        private readonly Timer wiseWordTimer;
        private readonly Microsoft.Extensions.Logging.ILogger logger;

        public MainPageViewModel()
        {
            this.logger = new LoggerFactory().AddSerilog().CreateLogger("MainPage");
            this.Tapsleft = Globals.Configuration.RuntimeConfiguration.TapsLeft;

            Task.Run(() =>
            {
                if (Globals.Phrases == null || Globals.Phrases.Count <= 0)
                {
                    Globals.Phrases = [.. Utilities.LoadTextfileAsync("Phrases.txt").Result.Split('\n')];
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

            this.saveTimer.Elapsed += (s, e) =>
            {
                Globals.Configuration.Save();
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

        private void DetermineAndSetTheme()
        {
            if (Globals.Configuration.RuntimeConfiguration.Theme)
            {
                this.ThemeIcon = FluentIcons.WeatherSunny48;
                Application.Current.UserAppTheme = AppTheme.Light;
            }
            else
            {
                this.ThemeIcon = FluentIcons.WeatherMoon48;
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
                await this.EggImage.TranslateTo(rnd.Next(-7, 0), 0, (uint)rnd.Next(5, 10));    // Move image left
                await this.EggImage.TranslateTo(rnd.Next(-7, 0), rnd.Next(-7, 0), (uint)rnd.Next(5, 10)); // Move image diagonally up and left
                await this.EggImage.TranslateTo(rnd.Next(0, 7), rnd.Next(0, 7), (uint)rnd.Next(10, 20));   // Move image diagonally down and right
                await this.EggImage.TranslateTo(0, rnd.Next(0, 7), (uint)rnd.Next(5, 10));     // Move image left
                await this.EggImage.TranslateTo(0, 0, (uint)rnd.Next(80, 100));       // Move image up
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
                this.WiseText = GetRandomPhrase();
                this.ShowWiseText = true;
                this.wiseWordTimer.Start();
            }
        }

        private static string GetRandomPhrase()
        {
            Random rnd = new(BitConverter.ToInt32(Guid.NewGuid().ToByteArray()));
            return Globals.Phrases[rnd.Next(0, Globals.Phrases.Count - 1)];
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
