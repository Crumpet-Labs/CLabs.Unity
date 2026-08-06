# Spoon-Belfry Bridge: Full Guide

This bridge republishes every Spoon state change as a Belfry message. Distant systems (HUD, networking, analytics, achievements, audio) can react to a feature's state without importing that feature's types.

If you're new to either package, read their respective `Documentation~/Guide.md` first. This guide assumes you know what a Spoon `Store<TState>` is and how Belfry's bell/toll lanes work.

## What the bridge gives you

Two types:

1. **`SpoonStateChangedMessage<TState>`**: a `readonly struct` carrying a single field, `State`. This is the message that listeners subscribe to.
2. **`SpoonBelfryMediator<TState>`**: a singleton, registered per-feature, that subscribes to the store on construction and rings a `SpoonStateChangedMessage<TState>` on a Belfry rope on every change.

There is no separate `Use...Package()` step. The bridge registers per-feature via `AddSpoonBelfryBridge<TState>(bellKey)`, mirroring `AddSpoonStore<TState, TReducer>`.

## Wiring

```csharp
public static IConfigurableCollection UseGameSettingsFeature(this ApplicationBuilder builder)
{
    return new ConfigurableCollection()
        .Register(builder.AddSpoonStore<GameSettings, GameSettingsReducer>())
        .Register(builder.AddSpoonBelfryBridge<GameSettings>("game-settings"));
}
```

After `builder.Build()`, you must touch the mediator at least once to bring it to life (Buttr resolves singletons lazily):

```csharp
_ = Application<SpoonBelfryMediator<GameSettings>>.Get();
```

Many features do this from a one-shot bootstrap method that runs after the container is built. Once resolved, the subscription is live for the container's lifetime.

The bell key (`"game-settings"` above) is whatever Belfry rope you want to publish on. Listeners use the same key to subscribe, as below.

## Consuming

```csharp
var tower = Application<IBellTower>.Get();
var rope = tower.Rope("game-settings");

using var handle = rope.OnBell<SpoonStateChangedMessage<GameSettings>>(
    (in SpoonStateChangedMessage<GameSettings> msg) =>
    {
        // React to state change. msg.State holds the post-reduce snapshot.
        Console.WriteLine($"Volume = {msg.State.Volume}");
    });
```

`OnBell` is the synchronous bell lane: the callback runs on the dispatch thread, before `store.Dispatch(...)` returns. Use `OnToll` for the async queue lane (`Ticket`-awaitable, decoupled from the dispatch thread). The bridge always rings the bell lane; if you need the toll lane, build a thin wrapper that subscribes to the bell and re-rings on the toll lane, or subscribe to the store directly.

## When a message fires

The mediator fires on every `IStore.Subscribe` callback. That means:

- **Every dispatch.** Even when the reducer returned `_ => state` and made no real change. The bridge cannot distinguish "state actually changed" from "subscribers were notified", because Spoon does not report the difference.
- **Every `Restore`.** When the spoon-knife bridge (or your code) calls `store.Restore(snapshot)`, observers fire, including this bridge. The restored snapshot lands on the rope as a normal `SpoonStateChangedMessage`.

If you want deduplication ("only fire when state actually changed"), implement `IEquatable<TState>` on your state type and filter in the listener:

```csharp
TState last = default;
using var handle = rope.OnBell<SpoonStateChangedMessage<GameSettings>>(
    (in SpoonStateChangedMessage<GameSettings> msg) =>
    {
        if (msg.State.Equals(last)) return;
        last = msg.State;
        HandleChange(msg.State);
    });
```

## Key choice

The bell key can be anything Belfry accepts as a rope key: a `string`, an enum, or a custom struct. Recommended:

- One key per Spoon feature: `"game-settings"`, `"profile"`, `"match"`.
- Don't share keys between features. The bridge's message type is `SpoonStateChangedMessage<TState>` and is unique per `TState`, but the rope key is shared. Two features ringing on the same rope means listeners may pull from either, with no compile-time guard against confusing them.

The bridge registers `SpoonBelfryMediator<TState>` as a Buttr singleton keyed by `TState`. That means **one bridge per state type**: calling `AddSpoonBelfryBridge<GameSettings>(...)` twice with different keys won't give you two mediators, the second registration replaces the first. If you need to broadcast the same state on multiple rope keys, subscribe to one rope inside a listener and re-ring on a second rope yourself.

## Synchronous reactions, async dispatches

Listener callbacks on the bell lane are synchronous. They run **before `Dispatch` returns**. Dispatching back into the same store from a listener throws, caught by Spoon's re-entrancy guard.

Two safe patterns for cross-store reactions:

1. **Listen on the bell, dispatch to a different store.** Different `TState`, no re-entrancy. Works fine.
2. **Move to the toll lane.** Wrap the listener in a queued dispatch (`Ticket`-aware) so the actual reaction runs after the original dispatch unwinds.

```csharp
// Pattern 1: cross-store reaction
using var handle = tower.Rope("game-settings").OnBell<SpoonStateChangedMessage<GameSettings>>(
    (in SpoonStateChangedMessage<GameSettings> msg) =>
    {
        analyticsStore.Dispatch(new SettingsChangedAction(msg.State.Volume));
    });
```

## Mediator lifetime

The mediator is a singleton scoped to the Buttr container. When the container is disposed, the mediator's subscription on the store is disposed too, and republishing stops. You normally never call `Dispose()` manually.

If you need to stop republishing without tearing down the container (e.g. during a "mute analytics" mode), there's no built-in pause. Options:

- Don't register the bridge for that feature.
- Filter at the listener side and have the listener no-op when muted.
- Replace the bridge with custom mediator code that respects a runtime flag.

## Composing with other Spoon bridges

When other Spoon bridges register subscribers on the same store, they all fire in registration order on every dispatch. The Belfry bridge is a normal subscriber, and its `RingBell` happens synchronously inside the `IStore.Dispatch` call that triggered it. No coupling between bridges; they neither know about each other nor coordinate.

This bridge also fires when something calls `IStore.Restore(snapshot)` outside the dispatch cycle (for example, an undo or a snapshot-load mechanism in your own code). The restored snapshot lands on the rope as a normal `SpoonStateChangedMessage`. UI listeners re-render. Usually what you want.

## Dependencies

- `com.clabs.spoon`: the store source.
- `com.clabs.belfry`: the message bus and bell/toll lanes.

Pure C#. `noEngineReferences: true`. Compiles and runs in pure-.NET test environments.
