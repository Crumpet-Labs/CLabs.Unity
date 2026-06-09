namespace CLabs.Belfry.Samples {
    /// <summary>Rung at the pass when a fresh batch of crumpets is ready to run.</summary>
    public readonly struct CrumpetReady {
        public string Table { get; }
        public int Count { get; }

        public CrumpetReady(string table, int count) {
            Table = table;
            Count = count;
        }
    }

    /// <summary>Rope keys for the lab's bell tower — kept in one place so publisher and
    /// subscribers agree on the channel.</summary>
    public static class PassBellKeys {
        public const string Service = "lab.service";
    }
}
