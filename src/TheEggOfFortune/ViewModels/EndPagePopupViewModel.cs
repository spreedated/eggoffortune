using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using TheEggOfFortune.Logic;

namespace TheEggOfFortune.ViewModels
{
    internal partial class EndPagePopupViewModel : ViewModelBase<Popup>
    {
        [ObservableProperty]
        public partial string EndText { get; set; }

        [ObservableProperty]
        public partial string EndTextCredits { get; set; }

        [ObservableProperty]
        public partial string LastWords { get; set; }

        [ObservableProperty]
        public partial string License { get; set; }

        #region Ctor
        public EndPagePopupViewModel() : base(nameof(EndPagePopupViewModel))
        {
            Task.Run(this.LoadTexts);
        }
        #endregion

        [RelayCommand]
        private async Task ResetButton()
        {
            Globals.ScoreService.ResetScore();
            await base.Instance.CloseAsync();
        }

        [RelayCommand]
        private async Task CloseButton()
        {
            await base.Instance.CloseAsync();
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
