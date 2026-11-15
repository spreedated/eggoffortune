using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
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

        private void TouchTrackingBehavior_TouchAction(object sender, TouchActionEventArgs e)
        {
            if (e.Type != TouchActionType.Pressed)
            {
                return;
            }

            if (HapticFeedback.Default.IsSupported)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }

            ((MainPageViewModel)this.BindingContext).TapCommand.Execute(null);
        }
    }
}
