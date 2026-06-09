using System;

namespace CLabs.Fork.Samples {
    /// <summary>One playthrough's save payload — plain serializable data. Fork handles integrity,
    /// write-then-swap, and migration around it.</summary>
    [Serializable]
    public sealed class ChefSave {
        public string ChefName;
        public int Day;
        public int Coins;
    }
}
