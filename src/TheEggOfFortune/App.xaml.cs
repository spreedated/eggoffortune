using Microsoft.Maui;
using Microsoft.Maui.Controls;
using TheEggOfFortune.Views;

namespace TheEggOfFortune
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            return new(new LoadingScreen());
        }
    }
}
