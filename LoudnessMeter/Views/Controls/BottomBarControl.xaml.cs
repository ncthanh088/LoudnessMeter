namespace LoudnessMeter.Views.Controls;

public partial class BottomBarControl : ContentView
{
    public event EventHandler PauseOrResumeClicked;

    private bool isPaused = true;
    public BottomBarControl()
	{
		InitializeComponent();
	}

    private async void OnSettingsButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SettingPage");
    }

    private async void OnAboutButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AboutPage");
    }

    private void OnExitButtonClicked(object sender, EventArgs e)
    {
        // Handle exit logic here
    }

    private void OnPauseOrResumeRecording(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        SwitchButtonThemeColor(button);

        PauseOrResumeClicked?.Invoke(this, EventArgs.Empty);
    }

    private void SwitchButtonThemeColor(ImageButton button)
    {
        if (isPaused)
        {
            // Resume recording
            button.Source = "paused.png";
            isPaused = false;
        }
        else
        {
            // Pause recording
            if (Application.Current.RequestedTheme == AppTheme.Light)
            {
                button.Source = "resumed_light.png";
            }
            else
            {
                button.Source = "resumed_dark.png";
            }
            isPaused = true;
        }
    }
}