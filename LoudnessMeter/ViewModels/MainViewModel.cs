using LoudnessMeter.Models;
using LoudnessMeter.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using DevExpress.Maui.Charts;
using System;

namespace LoudnessMeter.ViewModels;

public partial class MainViewModel : ObservableObject
{

    [ObservableProperty] private string _boldTitle = "SOUND";

    [ObservableProperty] private string _regularTitle = "LEVEL METER";

    [ObservableProperty] private string _decibel = "0 dB";

    [ObservableProperty] private string _decibelAverage = "0 AVG";

    [ObservableProperty] private string _decibelMax = "0 dB";

    [ObservableProperty] private string _decibelMin = "0 dB";

    [ObservableProperty] private double _volumeMarkerIndicator;

    [ObservableProperty] private double _volumeNeedleIndicator;

    [ObservableProperty] private ObservableCollection<DecibelValue> _decibelValues = new();

    [ObservableProperty] private NumericRange _visualRangeAxisX;

    [ObservableProperty] private NumericRange _visualRangeAxisY;

    private int _updateCounter;
    
    private readonly IAudioCaptureService _audioCaptureService;

    public MainViewModel(IAudioCaptureService audioCaptureService)
    {
        PrepareChartConfig();
        _audioCaptureService = audioCaptureService;
        Task.Run(async () =>
        {
            while (true)
            {
                var decibel = _audioCaptureService.CurrentDecibel;
                await MainThread.InvokeOnMainThreadAsync(() => UpdateChart(decibel));
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        });
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

        // Set Volume Guages
        VolumeNeedleIndicator = audioChuckData.Decibel;
        VolumeMarkerIndicator = audioChuckData.DecibelMax;
    }

    private void UpdateChart(double decibel)
    {
        try
        {
            var currentTime = DateTime.Now.Second;
            var decibelValue = new DecibelValue(currentTime, (float)decibel);
            if (DecibelValues.Count >= 60)
            {
                DecibelValues.RemoveAt(0);
            }
            DecibelValues.Add(decibelValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    private void PrepareChartConfig()
    {
        VisualRangeAxisX = new NumericRange()
        {
            Min = 0,
            Max = 60
        };

        VisualRangeAxisY = new NumericRange()
        {
            Min = 0,
            Max = 100
        };

        DecibelValues = new ObservableCollection<DecibelValue>();
    }
}
