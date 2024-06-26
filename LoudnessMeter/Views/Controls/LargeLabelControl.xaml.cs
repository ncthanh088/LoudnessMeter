namespace LoudnessMeter.Views.Controls;

public partial class LargeLabelControl : ContentView
{
    public static readonly BindableProperty LargeTextProperty
        = BindableProperty.Create(nameof(LargeText), typeof(string), typeof(LargeLabelControl), defaultValue: "LARGE TEXT");

    public static readonly BindableProperty SmallTextProperty
        = BindableProperty.Create(nameof(SmallText), typeof(string), typeof(LargeLabelControl), defaultValue: "SMALL TEXT");

    public static readonly BindableProperty TextColorProperty
        = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(LargeLabelControl), defaultValue: Colors.White);

    public static new readonly BindableProperty BackgroundColorProperty
        = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(LargeLabelControl), defaultValue: default(Color));

    public string LargeText
    {
        get => (string)GetValue(LargeTextProperty);
        set => SetValue(LargeTextProperty, value);
    }

    public string SmallText
    {
        get => (string)GetValue(SmallTextProperty);
        set => SetValue(SmallTextProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public new Color BackgroundColor
    {
        get => (Color)GetValue(LargeLabelControl.BackgroundColorProperty);
        set => SetValue(LargeLabelControl.BackgroundColorProperty, value);
    }

    public LargeLabelControl()
    {
        InitializeComponent();
    }
}