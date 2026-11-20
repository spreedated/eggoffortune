using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using TheEggOfFortune.Logic;

namespace TheEggOfFortune.ViewModels
{
    public partial class DevScreenViewModel : ViewModelBase<Popup>
    {
        private readonly bool _initComplete = false;

        [ObservableProperty]
        public partial string Taps { get; set; }

        partial void OnTapsChanging(string value)
        {
            if (!_initComplete)
            {
                return;
            }

            if (int.TryParse(value, out int v))
            {
                Globals.ScoreService.SetScore(v);
                Task.Run(async () => await Globals.Configuration.SaveAsync());
                return;
            }

            this.Taps = "0";
        }

        #region Ctor
        public DevScreenViewModel() : base(nameof(DevScreenViewModel))
        {
            this.Taps = Globals.ScoreService.Score.ToString();
            _initComplete = true;
        }
        #endregion

        [RelayCommand]
        private async Task Close()
        {
            await this.Instance?.CloseAsync();
        }
    }
}
