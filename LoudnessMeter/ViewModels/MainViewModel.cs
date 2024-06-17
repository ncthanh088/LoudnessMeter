using LoudnessMeter.Models;
using LoudnessMeter.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LoudnessMeter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAudioCaptureService _audioCaptureService;
    
    private int _updateCounter;

    private const double DecibelRuler = 130;

    [ObservableProperty] private string _boldTitle = "SOUND";

    [ObservableProperty] private string _regularTitle = "LEVEL METER";

    [ObservableProperty] private string _decibel = "0 dB";

    [ObservableProperty] private string _decibelAverage = "0 AVG";
    
    [ObservableProperty] private string _decibelMax = "0 dB";
    
    [ObservableProperty] private string _decibelMin = "0 dB";

    [ObservableProperty] private bool _channelConfigurationListIsOpen;

    [ObservableProperty] private double _volumePercentPosition;

    [ObservableProperty] private double _volumeContainerHeight;

    [ObservableProperty] private double _volumeBarHeight;

    [ObservableProperty] private double _volumeBarMaskHeight;

    public MainViewModel(IAudioCaptureService audioCaptureService)
    {
        _audioCaptureService = audioCaptureService;
        VolumeBarMaskHeight = VolumeContainerHeight;
    }

    public void PauseRecordAudio()
    {
        _audioCaptureService.Stop();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        try
        {
            await RequestRecordAudioPermissionAsync();

            StartCapture(deviceId: 1);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
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

    private void StartCapture(int deviceId)
    {
        try
        {
            // Initialize capturing on specific device
            _audioCaptureService.InitCapture(deviceId);

            // Listen out for chunks of information
            _audioCaptureService.AudioChunkAvailable += audioChuckData =>
            {
                ProcessAudioChunk(audioChuckData);
            };

            // Start capturing
            _audioCaptureService.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    private void ProcessAudioChunk(AudioChunkData audioChuckData)
    {
        // Counter between 0-1-2
        _updateCounter = (_updateCounter + 1) % 3;

        // Every time counter is at 0...
        if (_updateCounter == 0)
        {
            Decibel = $"{Math.Max(-60, audioChuckData.Decibel):0.0} dB";
            DecibelAverage = $"{Math.Max(-60, audioChuckData.DecibelAverage):0.0} dB";
            DecibelMax = $"{Math.Max(-60, audioChuckData.DecibelMax):0.0} dB";
            DecibelMin = $"{Math.Max(-60, audioChuckData.DecibelMin):0.0} dB";
        }

        // Set volume bar height
        var decibelHeight = (VolumeBarHeight * audioChuckData.Decibel) / DecibelRuler;
        VolumeBarMaskHeight = Math.Min(VolumeBarHeight, VolumeBarHeight - decibelHeight);

        // Set Volume Arrow height
        var decibelAverageHeight = (VolumeContainerHeight * audioChuckData.DecibelMax) / DecibelRuler;
        VolumePercentPosition = Math.Min(VolumeContainerHeight, VolumeContainerHeight- decibelAverageHeight);
    }
}
