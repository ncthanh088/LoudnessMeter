using ManagedBass;
using LoudnessMeter.Models;

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

    public event Action<AudioChunkData> AudioChunkAvailable;

    /// <summary>
    /// Initializes the audio capture with the specified device ID and frequency.
    /// </summary>
    /// <param name="deviceId">The ID of the audio capture device. Defaults to 1.</param>
    /// <param name="frequency">The frequency of the audio capture. Defaults to 44100.</param>
    /// <exception cref="Exception">Thrown if there is an error starting the audio capture.</exception>
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

    /// <summary>
    /// Starts the audio capture service. Throws an InvalidOperationException if the service is not initialized.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the capture service is not initialized.</exception>
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

    // Processes the captured audio chunk if the length is greater than zero and invokes the event handler.
    private bool AudioChunkCaptured(int handle, IntPtr buffer, int length, IntPtr user)
    {
        if (length > 0)
        {
            _buffer = new byte[length];
            System.Runtime.InteropServices.Marshal.Copy(buffer, _buffer, 0, length);
            var data = ProcessAudioBuffer(_buffer, length);
            AudioChunkAvailable?.Invoke(data);
        }
        return true;
    }

    /// <summary>
    /// Processes the captured audio buffer and calculates various audio metrics such as RMS, decibel, average decibel, max and min decibel, 
    /// loudness, short term LUFS, integrated LUFS, loudness range, realtime dynamics, average realtime dynamics, momentary max LUFS, short term max LUFS, and true peak max.
    /// </summary>
    /// <param name="buffer">The audio buffer to process.</param>
    /// <param name="length">The length of the audio buffer.</param>
    /// <returns>An instance of AudioChunkData containing the calculated audio metrics.</returns>
    private AudioChunkData ProcessAudioBuffer(byte[] buffer, int length)
    {
        double rms = CalculateRMS(buffer);
        
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

        // Replace with actual methods for calculation
        double loudness = CalculateLoudness(buffer);
        double shortTermLUFS = CalculateShortTermLUFS(buffer);
        double integratedLUFS = CalculateIntegratedLUFS(buffer);
        double loudnessRange = CalculateLoudnessRange(buffer);
        double realtimeDynamics = CalculateRealtimeDynamics(buffer);
        double averageRealtimeDynamics = CalculateAverageRealtimeDynamics(buffer);
        double momentaryMaxLUFS = CalculateMomentaryMaxLUFS(buffer);
        double shortTermMaxLUFS = CalculateShortTermMaxLUFS(buffer);
        double truePeakMax = CalculateTruePeakMax(buffer);

        return new AudioChunkData
        (
            Decibel: decibel,
            DecibelAverage: decibelAverage,
            DecibelMax: _maxDecibel,
            DecibelMin: _minDecibel,
            Loudness: loudness,
            ShortTermLUFS: shortTermLUFS,
            IntegratedLUFS: integratedLUFS,
            LoudnessRange: loudnessRange,
            RealtimeDynamics: realtimeDynamics,
            AverageRealtimeDynamics: averageRealtimeDynamics,
            MomentaryMaxLUFS: momentaryMaxLUFS,
            ShortTermMaxLUFS: shortTermMaxLUFS,
            TruePeakMax: truePeakMax
        );
    }

    // Calculates the Root Mean Square (RMS) value of the audio samples stored in the given byte array buffer.
    private double CalculateRMS(byte[] buffer)
    {
        double sum = 0;
        for (int i = 0; i < buffer.Length; i += 2)
        {
            short sample = BitConverter.ToInt16(buffer, i);
            sum += sample * sample;
        }
        double rms = (double)Math.Sqrt(sum / (buffer.Length / 2));
        return rms;
    }

    private double CalculateDecibelMax(short[] buffer)
    {
        foreach (short sample in buffer)
        {
            double decibel = 20 * Math.Log10(Math.Abs(sample) / (double)short.MaxValue);
            if (decibel > _maxDecibel)
            {
                _maxDecibel = decibel;
            }
        }
        return _maxDecibel;
    }

    private double CalculateDecibelMin(short[] buffer)
    {
        double _minDecibel = double.MaxValue;
        foreach (short sample in buffer)
        {
            double decibel = 20 * Math.Log10(Math.Abs(sample) / (double)short.MaxValue);
            if (decibel < _minDecibel)
            {
                _minDecibel = decibel;
            }
        }
        return _minDecibel;
    }

    // TODO: Implement for features
    private double CalculateLoudness(byte[] buffer) => 0.0;
    private double CalculateShortTermLUFS(byte[] buffer) => 0.0;
    private double CalculateIntegratedLUFS(byte[] buffer) => 0.0;
    private double CalculateLoudnessRange(byte[] buffer) => 0.0;
    private double CalculateRealtimeDynamics(byte[] buffer) => 0.0;
    private double CalculateAverageRealtimeDynamics(byte[] buffer) => 0.0;
    private double CalculateMomentaryMaxLUFS(byte[] buffer) => 0.0;
    private double CalculateShortTermMaxLUFS(byte[] buffer) => 0.0;
    private double CalculateTruePeakMax(byte[] buffer) => 0.0;
}
