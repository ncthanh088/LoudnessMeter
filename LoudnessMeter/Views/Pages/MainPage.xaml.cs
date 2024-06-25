using LoudnessMeter.ViewModels;

namespace LoudnessMeter.Views.Pages;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        ApplyTheme(Application.Current.RequestedTheme);
        Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;

        _viewModel = serviceProvider.GetService<MainViewModel>();
        
        BindingContext = _viewModel;

        bottomBar.PauseOrResumeClicked += OnPauseOrResumeClicked;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadCommand.ExecuteAsync(null);
    }

    private void OnPauseOrResumeClicked(object sender, EventArgs e)
    {
        _viewModel.PausedOrResumed();
    }

    private void OnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
    {
        ApplyTheme(e.RequestedTheme);
    }

    private void ApplyTheme(AppTheme theme)
    {
        if (!MainThread.IsMainThread)
        {
            MainThread.BeginInvokeOnMainThread(() => ApplyTheme(theme));
            return;
        }

#if  ANDROID
        statusBar.StatusBarColor = (Color)Application.Current.Resources["BackgroundColor"];
        if (theme == AppTheme.Dark)
        {
            statusBar.StatusBarStyle = CommunityToolkit.Maui.Core.StatusBarStyle.LightContent;
        }
        else
        {
            statusBar.StatusBarStyle = CommunityToolkit.Maui.Core.StatusBarStyle.DarkContent;
        }
#endif
    }
}
