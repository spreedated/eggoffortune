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
    internal partial class EndPagePopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private Popup instance;

        [ObservableProperty]
        private string endText = null;

        [ObservableProperty]
        private string endTextCredits = null;

        [ObservableProperty]
        private string lastWords = null;

        [ObservableProperty]
        private string license = null;

        public EndPagePopupViewModel()
        {
            Task.Run(this.LoadTexts);
        }

        [RelayCommand]
        private void CloseButton()
        {
            this.Instance.Close();
        }

        public async Task LoadTexts()
        {
            if (string.IsNullOrEmpty(this.EndText))
            {
                this.EndText = await Utilities.LoadTextfileAsync("End.txt");
            }

            if (string.IsNullOrEmpty(this.EndTextCredits))
            {
                DateTime builddate = neXn.Lib.Maui.Utilities.GetBuildDate(typeof(EndPagePopupViewModel).Assembly);
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
