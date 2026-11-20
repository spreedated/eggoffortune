using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;
using System.Threading.Tasks;
using TheEggOfFortune.Logic;
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

        private async Task ShowHitMarkerAsync(Point position)
        {
            const double size = 28;
            const uint duration = 180;

            Ellipse ring = new()
            {
                WidthRequest = size,
                HeightRequest = size,
                StrokeThickness = 2,
                Fill = Brush.Transparent,
                InputTransparent = true,
                Scale = 0.5,
                Opacity = 0.9
            };

            ring.SetAppTheme<Brush>(Shape.StrokeProperty, Brush.Black, Brush.White);

            AbsoluteLayout.SetLayoutBounds(ring, new Rect(position.X - size / 2, position.Y - size / 2, size, size));

            HitMarkerLayer.Children.Add(ring);

            try
            {
                await Task.WhenAll(ring.ScaleToAsync(1.5, duration, Easing.CubicOut), ring.FadeToAsync(0, duration, Easing.CubicIn));
            }
            finally
            {
                HitMarkerLayer.Children.Remove(ring);
            }
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

            Task markerAnimation = ShowHitMarkerAsync(position);

            if (HapticFeedback.Default.IsSupported)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }

            await ((MainPageViewModel)this.BindingContext).TapCommand.ExecuteAsync(null);
            DeveloperService.AddTap(DeveloperService.TapCode.Egg);

            await markerAnimation;
        }

        private void TouchTrackingBehavior_TouchActionCounter(object sender, TouchActionEventArgs e)
        {
            if (e.Type != TouchActionType.Pressed)
            {
                return;

            }
            DeveloperService.AddTap(DeveloperService.TapCode.Counter);
        }
    }
}
