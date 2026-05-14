#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CLabs.Tickets
{
    public enum PlayerLoopTiming
    {
        Initialization = 0,
        LastInitialization = 1,

        EarlyUpdate = 2,
        LastEarlyUpdate = 3,

        FixedUpdate = 4,
        LastFixedUpdate = 5,

        PreUpdate = 6,
        LastPreUpdate = 7,

        Update = 8,
        LastUpdate = 9,

        PreLateUpdate = 10,
        LastPreLateUpdate = 11,

        PostLateUpdate = 12,
        LastPostLateUpdate = 13,

        // Unity 2020.2+ TimeUpdate slot. Always declared so the enum is engine-agnostic;
        // adapters gate the actual PlayerLoop wiring on engine version.
        TimeUpdate = 14,
        LastTimeUpdate = 15,
    }
}
