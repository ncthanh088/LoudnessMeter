using LoudnessMeter.ViewModels;

namespace LoudnessMeter;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;
    private Timer _sizingTimer;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
        //_viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Your code to run when the page appears
        // For example, initializing data or starting animations

        await RequestRecordAudioPermissionAsync();

        await _viewModel.LoadCommand.ExecuteAsync(null);

        _sizingTimer = new System.Threading.Timer((state) =>
        {
            // Invoke on the main UI thread to update UI elements
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Update the desired size
                UpdateSizes(_viewModel);
            });
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    //private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //    if (e.PropertyName == nameof(MainViewModel.VolumeBarMaskHeight))
    //    {
    //        var viewModel = sender as MainViewModel;
    //        AnimateVolumeBarHeight(viewModel.VolumeBarMaskHeight);
    //    }
    //}

    //private void AnimateVolumeBarHeight(double targetHeight)
    //{
    //    double startHeight = VolumeBarBoxView.Height;
    //    var animation = new Animation(v => VolumeBarBoxView.HeightRequest = v, startHeight, targetHeight, Easing.Linear);
    //    animation.Commit(this, "HeightAnimation", length: 0);
    //}

    private void UpdateSizes(MainViewModel viewModel)
    {
        viewModel.VolumeBarHeight = VolumeBar.Bounds.Height;
        viewModel.VolumeContainerHeight = VolumeContainer.Bounds.Height;
    }

    private async Task RequestRecordAudioPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            await Permissions.RequestAsync<Permissions.Microphone>();
        }

        //await Shell.Current.DisplayAlert("Permission required",
        //   "Record Audio permission is required for audio capture. " +
        //   "We do not store or use your audio at all.", "OK");
    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        _viewModel.PauseRecordAudio();
    }
}
