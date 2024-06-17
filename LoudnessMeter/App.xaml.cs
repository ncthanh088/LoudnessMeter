﻿namespace LoudnessMeter
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
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
    }
}
