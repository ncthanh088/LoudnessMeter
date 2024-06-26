namespace LoudnessMeter.Enums;

public enum SoundLevel
{
    Silent,                 // <= 20 dB
    VeryQuiet,              // From 20 To 30 dB
    Quiet,                  // From 30 To 40 dB
    NormalConversation,     // From 40 To 60 dB
    BusyTraffic,            // From 60 To 70 dB
    LoudTraffic,            // From 70 To 80 dB
    VacuumCleaner,          // From 70 To 80 dB
    LoudMusic,              // From 80 To 90 dB
    NoisyRestaurant,        // From 80 To 90 dB
    PowerLawnmower,         // From 90 To 100 dB
    ChainSaw,               // From 100 To 120 dB
    RockConcert             // >= 120 dB
}
