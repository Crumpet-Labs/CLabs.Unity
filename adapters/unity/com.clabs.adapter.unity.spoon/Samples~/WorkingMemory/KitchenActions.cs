using CLabs.Spoon;

namespace CLabs.Spoon.Samples {
    /// <summary>A crumpet was served for <see cref="Coin"/> coins.</summary>
    public readonly struct ServedCrumpet : IAction {
        public int Coin { get; }
        public ServedCrumpet(int coin) => Coin = coin;
    }

    /// <summary>Old Hob's temperature changed by <see cref="Delta"/> degrees.</summary>
    public readonly struct HeatedOven : IAction {
        public int Delta { get; }
        public HeatedOven(int delta) => Delta = delta;
    }

    /// <summary>A new service day opened: bank the day, cool the oven back to room temperature.</summary>
    public readonly struct NewDay : IAction { }
}
