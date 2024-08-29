using MauiIcons.Core;
using Microsoft.Maui.Controls;

namespace TheEggOfFortune
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.MainPage = new AppShell();
            _ = new MauiIcon(); //Seems odd? That's because it is. This is a workaround for a bug in the MAUI Framework. Probably will never be fixed. ¯\_(ツ)_/¯ - This time to workaround the custom styles.
        }
    }
}
