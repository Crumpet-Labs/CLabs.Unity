namespace CLabs.Spoon.Samples {
    /// <summary>Immutable snapshot of the lab's live numbers. Spoon never mutates in place — the
    /// reducer returns a fresh copy on every action.</summary>
    public readonly struct KitchenState {
        public int Day { get; }
        public int Coins { get; }
        public int OvenTemp { get; }

        public KitchenState(int day, int coins, int ovenTemp) {
            Day = day;
            Coins = coins;
            OvenTemp = ovenTemp;
        }

        /// <summary>A derived value (selector): Old Hob is hot enough to serve safely.</summary>
        public bool IsSafeToServe => OvenTemp >= 200;
    }
}
