namespace LoudnessMeter.Models
{
    /// <summary>
    /// Holds all the information about a single chunk of audio for display in the UI
    /// </summary>
    public class AudioChunkData
    {
        public double Decibel { get; set; }
        public double DecibelMax { get; set; }
        public double DecibelMin { get; set; }
        public double DecibelAverageOverall { get; set; }
        public double DecibelAverage { get; set; }
    }
}
