namespace LoudnessMeter.Views.Pages;

public partial class SettingPage : ContentPage
{
	public SettingPage()
	{
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//MainPage");
        return true;
    }
}