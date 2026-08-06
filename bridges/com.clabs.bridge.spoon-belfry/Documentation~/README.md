# CLabs Bridge: Spoon-Belfry

Re-publishes Spoon store state changes as Belfry messages. Lets distant systems react to state changes without importing Spoon's types directly.

## What it does

When both `com.clabs.spoon` and `com.clabs.belfry` are installed, this bridge attaches a subscriber to a `Store<TState>` that fires a `SpoonStateChangedMessage<TState>` on every change. Listeners hook Belfry through their own keys, with no compile-time coupling to the feature that owns the store.

## Wiring

```csharp
public static IConfigurableCollection UseGameSettingsFeature(this ApplicationBuilder builder)
{
    return new ConfigurableCollection()
        .Register(builder.AddSpoonStore<GameSettings, GameSettingsReducer>())
        .Register(builder.AddSpoonBelfryBridge<GameSettings>("game-settings"));
}
```

The `bellKey` (`"game-settings"` above) is the Belfry rope key. Listeners must use the same key.

## Consuming

```csharp
var tower = Application<IBellTower>.Get();
var rope = tower.Rope("game-settings");

using var handle = rope.OnBell<SpoonStateChangedMessage<GameSettings>>(
    (in SpoonStateChangedMessage<GameSettings> msg) =>
    {
        Console.WriteLine($"Volume changed: {msg.State.Volume}");
    });
```

## Dependencies

- `com.clabs.spoon`: the store source.
- `com.clabs.belfry`: the message bus.

Pure C#. `noEngineReferences: true`. Runs in tests without Unity.

## Notes

- Every dispatch produces a Belfry message, even if the reducer returned identical state. Filter in the subscriber if that matters.
- The mediator is a singleton per `TState`. Dispose it to stop republishing (normally the container lifetime handles this).
- Belfry's `RingBell` is synchronous, so the subscriber callback runs on the dispatch thread. Use `RingToll` (the async lane) if you need queued, awaitable delivery.

## Further reading

- [Guide.md](Guide.md): full walkthrough of message firing, listener patterns, and bell vs toll choice.
