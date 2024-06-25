using ManagedBass;
using LoudnessMeter.Models;
using System.Buffers.Binary;
using LoudnessMeter.Enums;
using LoudnessMeter.Extentions;

namespace LoudnessMeter.Services;

public class BassAudioCaptureService : IAudioCaptureService, IDisposable
{
    private int _device = 1;
    private int _handle;
    private byte[] _buffer;
    private double _totalRMS = 0;
    private int _bufferCount = 0;
    private bool _isInitialized = false;
    private double _maxDecibel = double.MinValue;
    private double _minDecibel = double.MinValue;
    private double _currentDecibel;
    public double CurrentDecibel => _currentDecibel;

    public event Action<AudioChunkData> AudioChunkAvailable;

    public void InitCapture(int deviceId = 1, int frequency = 44100)
    {
        Bass.RecordFree();

        Bass.RecordInit(deviceId);
        _handle = Bass.RecordStart(frequency, 2, BassFlags.RecordPause, 20, AudioChunkCaptured, IntPtr.Zero);
        if (_handle == 0)
        {
            throw new Exception("Bass record start error");
        }

        _isInitialized = true;
    }

    public void Start()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("Capture service is not initialized.");
        }

        Bass.ChannelPlay(_handle, Restart: false);
    }

    public void Paused()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("Capture service is not initialized.");
        }
        Bass.ChannelPause(_handle);
    }

    public void Resumed()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("Capture service is not initialized.");
        }
        Bass.ChannelPlay(_handle, Restart: true);
    }

    public void Stop()
    {
        if (_isInitialized)
        {
            Bass.ChannelStop(_handle);
        }
    }

    public string GetSoundLevelDescription(double decibel)
    {
        SoundLevel soundLevel = GetSoundLevel(decibel);

        switch (soundLevel)
        {
            case SoundLevel.Silent:
                return nameof(SoundLevel.Silent).SplitUpperCase();
            case SoundLevel.VeryQuiet:
                return nameof(SoundLevel.VeryQuiet).SplitUpperCase();
            case SoundLevel.Quiet:
                return nameof(SoundLevel.Quiet).SplitUpperCase();
            case SoundLevel.NormalConversation:
                return nameof(SoundLevel.NormalConversation).SplitUpperCase();
            case SoundLevel.BusyTraffic:
                return nameof(SoundLevel.BusyTraffic).SplitUpperCase();
            case SoundLevel.LoudTraffic:
                return nameof(SoundLevel.LoudTraffic).SplitUpperCase();
            case SoundLevel.VacuumCleaner:
                return nameof(SoundLevel.VacuumCleaner).SplitUpperCase();
            case SoundLevel.LoudMusic:
                return nameof(SoundLevel.LoudMusic).SplitUpperCase();
            case SoundLevel.NoisyRestaurant:
                return nameof(SoundLevel.NoisyRestaurant).SplitUpperCase();
            case SoundLevel.PowerLawnmower:
                return nameof(SoundLevel.PowerLawnmower).SplitUpperCase();
            case SoundLevel.ChainSaw:
                return nameof(SoundLevel.ChainSaw).SplitUpperCase();
            case SoundLevel.RockConcert:
                return nameof(SoundLevel.RockConcert).SplitUpperCase();
            default:
                throw new ArgumentOutOfRangeException(nameof(soundLevel), soundLevel, null);
        }
    }


    public void Dispose()
    {
        Bass.CurrentRecordingDevice = _device;
        Bass.RecordFree();
    }

    private bool AudioChunkCaptured(int handle, IntPtr buffer, int length, IntPtr user)
    {
        if (length > 0)
        {
            _buffer = new byte[length];
            System.Runtime.InteropServices.Marshal.Copy(buffer, _buffer, 0, length);

            Task.Run(() => ProcessAudioBufferAsync(_buffer, length));
        }
        return true;
    }

    private async Task ProcessAudioBufferAsync(byte[] buffer, int length)
    {
        double rms = await CalculateRMSAsync(buffer);

        // Calculate decibel
        double decibel = 20 * Math.Log10(rms);

        // Calculate average decibel
        _totalRMS += rms;
        _bufferCount++;
        double averageRMS = _totalRMS / _bufferCount;
        double decibelAverage = 20 * Math.Log10(averageRMS);

        // Calculate max and min decibel
        _maxDecibel = Math.Max(_maxDecibel, decibel);
        _minDecibel = Math.Min(_minDecibel, decibel);

        var data = new AudioChunkData
        {
            Decibel = decibel,
            DecibelAverage = 20 * Math.Log10(_totalRMS / _bufferCount),
            DecibelMax = _maxDecibel,
            DecibelMin = _minDecibel
        };

        _currentDecibel = data.Decibel;
        AudioChunkAvailable?.Invoke(data);
    }

    // Calculates the Root Mean Square (RMS)
    private Task<double> CalculateRMSAsync(byte[] buffer)
    {
        return Task.Run(() =>
        {
            double sum = 0;
            int samples = buffer.Length / 2;
            for (int i = 0; i < buffer.Length; i += 2)
            {
                short sample = BinaryPrimitives.ReadInt16LittleEndian(buffer.AsSpan(i, 2));
                sum += sample * sample;
            }
            return Math.Sqrt(sum / samples);
        });
    }

    private static SoundLevel GetSoundLevel(double decibel)
    {
        if (decibel < 20)
        {
            return SoundLevel.Silent;
        }
        else if (decibel >= 20 && decibel < 30)
        {
            return SoundLevel.VeryQuiet;
        }
        else if (decibel >= 30 && decibel < 40)
        {
            return SoundLevel.Quiet;
        }
        else if (decibel >= 40 && decibel < 60)
        {
            return SoundLevel.NormalConversation;
        }
        else if (decibel >= 60 && decibel < 70)
        {
            return SoundLevel.BusyTraffic;
        }
        else if (decibel >= 70 && decibel < 80)
        {
            return SoundLevel.LoudTraffic;
        }
        else if (decibel >= 80 && decibel < 90)
        {
            return SoundLevel.LoudMusic;
        }
        else if (decibel >= 90 && decibel < 100)
        {
            return SoundLevel.PowerLawnmower;
        }
        else if (decibel >= 100 && decibel < 120)
        {
            return SoundLevel.ChainSaw;
        }
        else
        {
            return SoundLevel.RockConcert;
        }
    }
}
