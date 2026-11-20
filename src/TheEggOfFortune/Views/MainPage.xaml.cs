using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;
using System.Threading.Tasks;
using TheEggOfFortune.Logic;
using TheEggOfFortune.ViewEffects;
using TheEggOfFortune.ViewModels;
using TouchTracking;

namespace TheEggOfFortune.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            ((MainPageViewModel)this.BindingContext).Instance = this;
            ((MainPageViewModel)this.BindingContext).EggImage = this.EggImage;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
#if ANDROID
            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
#endif
        }

        private async void TouchTrackingBehavior_TouchAction(object sender, TouchActionEventArgs e)
        {
            if (e.Type != TouchActionType.Pressed)
            {
                return;
            }

            if (Globals.ScoreService.Score <= 0)
            {
                DeveloperService.AddTap(DeveloperService.TapCode.Egg);
                return;
            }

            //Hitmarker
            var position = new Point(EggImage.X + EggImage.TranslationX + e.Location.X - HitMarkerLayer.X, EggImage.Y + EggImage.TranslationY + e.Location.Y - HitMarkerLayer.Y);
            Task markerAnimation = HitMarker.ShowHitMarkerAsync(HitMarkerLayer, position);

            if (HapticFeedback.Default.IsSupported)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }

            await ((MainPageViewModel)this.BindingContext).TapCommand.ExecuteAsync(null);
            DeveloperService.AddTap(DeveloperService.TapCode.Egg);

            await markerAnimation;
        }

#pragma warning disable S2325
        private void TouchTrackingBehavior_TouchActionCounter(object sender, TouchActionEventArgs e)
#pragma warning restore S2325
        {
            if (e.Type != TouchActionType.Pressed)
            {
                return;

            }
            DeveloperService.AddTap(DeveloperService.TapCode.Counter);
        }
    }
}
