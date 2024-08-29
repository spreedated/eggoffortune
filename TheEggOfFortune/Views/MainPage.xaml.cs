using MauiIcons.Core;
using Microsoft.Maui.Controls;
using TheEggOfFortune.ViewModels;
using TouchTracking;

namespace TheEggOfFortune.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            _ = new MauiIcon(); //Seems odd? That's because it is. This is a workaround for a bug in the MAUI Framework. Probably will never be fixed. ¯\_(ツ)_/¯
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
            if (e.Type == TouchActionType.Pressed)
            {
                ((MainPageViewModel)this.BindingContext).TapCommand.Execute(null);
            }
        }
    }
}
