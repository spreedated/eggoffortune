using Microsoft.Maui.Controls;
using System;
using TheEggOfFortune.ViewModels;

namespace TheEggOfFortune.Views;

public partial class LoadingScreen : ContentPage
{
    private LoadingScreenViewModel ViewModel => (LoadingScreenViewModel)this.BindingContext;

    public LoadingScreen()
    {
        this.InitializeComponent();
        this.ViewModel.Instance = this;

        this.Loaded += this.OnLoaded;
    }

    private async void OnLoaded(object sender, EventArgs e)
    {
        this.Loaded -= this.OnLoaded;

        this.ViewModel.StartTimerOnLoaded();
        await this.ViewModel.Load();
    }
}