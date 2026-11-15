using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using TheEggOfFortune.Logic;

namespace TheEggOfFortune.ViewModels
{
    internal partial class EndPagePopupViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial Popup Instance { get; set; }

        [ObservableProperty]
        public partial string EndText { get; set; } = null;

        [ObservableProperty]
        public partial string EndTextCredits { get; set; } = null;

        [ObservableProperty]
        public partial string LastWords { get; set; } = null;

        [ObservableProperty]
        public partial string License { get; set; } = null;

        public EndPagePopupViewModel()
        {
            Task.Run(this.LoadTexts);
        }

        [RelayCommand]
        private async Task ResetButton()
        {
            Globals.Configuration.RuntimeConfiguration.TapsLeft = 20000;
            await Globals.Configuration.SaveAsync().ConfigureAwait(false);
            await Globals.Configuration.LoadAsync().ConfigureAwait(false);
            ((MainPageViewModel)Application.Current.Windows[0].BindingContext).ReloadFromConfiguration();

            await this.CloseButton();
        }

        [RelayCommand]
        private async Task CloseButton()
        {
            await this.Instance.CloseAsync();
        }

        public async Task LoadTexts()
        {
            if (string.IsNullOrEmpty(this.EndText))
            {
                this.EndText = await Utilities.LoadTextfileAsync("End.txt");
            }

            if (string.IsNullOrEmpty(this.EndTextCredits))
            {
                DateTime builddate = neXn.Maui.Utilities.GetBuildDate(typeof(EndPagePopupViewModel).Assembly);
                Assembly a = this.GetType().Assembly;

                this.EndTextCredits = string.Format(await Utilities.LoadTextfileAsync("Credits.txt"), a.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkDisplayName, builddate == default ? "N/A" : builddate, a.GetName().Version.ToString());
            }

            if (string.IsNullOrEmpty(this.LastWords))
            {
                this.LastWords = await Utilities.LoadTextfileAsync("LastWords.txt");
            }

            if (string.IsNullOrEmpty(this.License))
            {
                this.License = await Utilities.LoadTextfileAsync("License.txt");
            }
        }
    }
}
