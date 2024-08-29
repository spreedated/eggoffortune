using CommunityToolkit.Maui.Views;
using TheEggOfFortune.ViewModels;

namespace TheEggOfFortune.Views;

public partial class EndPagePopup : Popup
{
    public EndPagePopup()
    {
        this.InitializeComponent();
        ((EndPagePopupViewModel)this.BindingContext).Instance = this;
    }
}