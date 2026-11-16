using CommunityToolkit.Maui.Views;
using TheEggOfFortune.ViewModels;

namespace TheEggOfFortune.Views
{
    public partial class DevScreen : Popup
    {
        public DevScreen()
        {
            this.InitializeComponent();
            ((DevScreenViewModel)this.BindingContext).Instance = this;
        }
    }
}