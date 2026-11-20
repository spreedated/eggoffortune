using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using TheEggOfFortune.Logic;

namespace TheEggOfFortune.ViewModels
{
    internal partial class EndPagePopupViewModel : ViewModelBase<Popup>
    {
        [ObservableProperty]
        public partial string EndText { get; set; } = Globals.EndText;

        [ObservableProperty]
        public partial string EndTextCredits { get; set; } = Globals.EndCreditText;

        [ObservableProperty]
        public partial string LastWords { get; set; } = Globals.LastWords;

        [ObservableProperty]
        public partial string License { get; set; } = Globals.License;

        #region Ctor
        public EndPagePopupViewModel() : base(nameof(EndPagePopupViewModel))
        {

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
    }
}
