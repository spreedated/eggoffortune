using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System.Timers;
using System.Windows.Input;

namespace TheEggOfFortune.ViewElements;

public partial class ButtonDoubleConfirm : ContentView
{
    private readonly Timer _buttonTextResetTimer = new(3000);

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ButtonDoubleConfirm), string.Empty);

    public string Text
    {
        get => (string)this.GetValue(TextProperty);
        set => this.SetValue(TextProperty, value);
    }

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ButtonDoubleConfirm), null);

    public ICommand Command
    {
        get => (ICommand)this.GetValue(CommandProperty);
        set => this.SetValue(CommandProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ButtonDoubleConfirm), null);

    public object CommandParameter
    {
        get => this.GetValue(CommandParameterProperty);
        set => this.SetValue(CommandParameterProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(string), typeof(ButtonDoubleConfirm), string.Empty);

    public string FontSize
    {
        get => (string)this.GetValue(FontSizeProperty);
        set => this.SetValue(FontSizeProperty, value);
    }

    private void ButtonTextResetTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        this.Text = this.Text.Replace("!!", "").Trim();
        _buttonTextResetTimer.Stop();
    }

    #region Ctor
    public ButtonDoubleConfirm()
    {
        this.InitializeComponent();

        _buttonTextResetTimer.Elapsed += this.ButtonTextResetTimer_Elapsed;
    }
    #endregion

    [RelayCommand]
    private void OnButtonClicked()
    {
        if (this.Text.StartsWith("!!"))
        {
            this.Command?.Execute(this.CommandParameter);
            return;
        }

        this.Text = $"!! {this.Text} !!";
        _buttonTextResetTimer.Start();
    }
}