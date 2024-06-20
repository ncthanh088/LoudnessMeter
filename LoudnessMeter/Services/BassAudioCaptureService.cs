using ManagedBass;
using LoudnessMeter.Models;
using System.Buffers.Binary;

namespace LoudnessMeter.Services;

public class BassAudioCaptureService : IAudioCaptureService, IDisposable
{
    private int _device;
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

        Bass.ChannelPlay(_handle);
    }

    public void Stop()
    {
        if (_isInitialized)
        {
            Bass.ChannelStop(_handle);
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
}
