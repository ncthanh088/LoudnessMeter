using LoudnessMeter.ViewModels;

namespace LoudnessMeter;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RequestRecordAudioPermissionAsync();
        await _viewModel.LoadCommand.ExecuteAsync(null);
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
}
