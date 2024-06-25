using CommunityToolkit.Mvvm.Messaging;
using LoudnessMeter.ViewModels;

namespace LoudnessMeter
{
    public partial class App : Application
    {
        public App()
        {

            InitializeComponent();

            ApplyTheme(Application.Current.RequestedTheme);
            Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;

            MainPage = new AppShell();

            // Shell.Current.GoToAsync("//SettingPage");
        }

        private void OnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            ApplyTheme(e.RequestedTheme);
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

#if WINDOWS
    if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
    {
      window.Width = 400;
      window.Height = 800;
    }
#endif

            return window;
        }

        private void ApplyTheme(AppTheme theme)
        {
            if (!MainThread.IsMainThread)
            {
                MainThread.BeginInvokeOnMainThread(() => ApplyTheme(theme));
                return;
            }

            this.Resources.MergedDictionaries.Clear();

            if (theme == AppTheme.Dark)
            {
                this.Resources.MergedDictionaries.Add(new Resources.Styles.Dark());
            }
            else
            {
                this.Resources.MergedDictionaries.Add(new Resources.Styles.Light());
            }
        }
    }
}
