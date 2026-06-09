using CLabs.Spoon;

namespace CLabs.Spoon.Samples {
    /// <summary>Pure reducer: (state, action) → new state. No mutation, no side effects.</summary>
    public sealed class KitchenReducer : IReducer<KitchenState> {
        public KitchenState InitialState => new KitchenState(day: 1, coins: 0, ovenTemp: 20);

        public KitchenState Reduce(KitchenState state, IAction action) => action switch {
            ServedCrumpet served => new KitchenState(state.Day, state.Coins + served.Coin, state.OvenTemp),
            HeatedOven heated     => new KitchenState(state.Day, state.Coins, state.OvenTemp + heated.Delta),
            NewDay                => new KitchenState(state.Day + 1, state.Coins, ovenTemp: 20),
            _                     => state,
        };
    }
}
