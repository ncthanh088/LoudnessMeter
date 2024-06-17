using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoudnessMeter.Models
{
    /// <summary>
    /// Holds all the information about a single chunk of audio for display in the UI
    /// </summary>
    /// <param name="Loudness"></param>
    /// <param name="ShortTermLUFS"></param>
    /// <param name="IntegratedLUFS"></param>
    /// <param name="LoudnessRange"></param>
    /// <param name="RealtimeDynamics"></param>
    /// <param name="AverageRealtimeDynamics"></param>
    /// <param name="MomentaryMaxLUFS"></param>
    /// <param name="ShortTermMaxLUFS"></param>
    /// <param name="TruePeakMax"></param>
    public record AudioChunkData(
        double Decibel,
        double DecibelMax,
        double DecibelMin,
        double DecibelAverage,
        double Loudness,
        double ShortTermLUFS,
        double IntegratedLUFS,
        double LoudnessRange,
        double RealtimeDynamics,
        double AverageRealtimeDynamics,
        double MomentaryMaxLUFS,
        double ShortTermMaxLUFS,
        double TruePeakMax
        );
}
