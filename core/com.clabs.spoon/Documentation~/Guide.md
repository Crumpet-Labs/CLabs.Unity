# Spoon: Full Guide

## Mental model

Spoon is a Redux-style state container. Every piece of Spoon-managed state lives in a `Store<TState>`. The store owns an immutable `TState` snapshot and exposes exactly three operations to the outside world:

- `State`: read the current snapshot.
- `Dispatch(IAction)`: send an action. Runs through middleware, then the reducer, then notifies subscribers.
- `Subscribe(SpoonObserver<TState>)`: register an observer. Returns `IDisposable` for unsubscribe. The observer receives state via `in TState` so the snapshot isn't copied per call.

That's the entire interaction surface. Everything else (reducers, middleware, action types) is feature-owned.

## The four concepts

### 1. State

A `readonly struct` with `readonly` fields. Value-typed, immutable, allocation-free on the stack.

```csharp
public readonly struct GameSettings
{
    public readonly float  Volume;
    public readonly string Language;
    
    public GameSettings(float volume, string language) 
    {
        Volume = volume; 
        Language = language;
    }
}
```

Why `readonly struct` (not `readonly record struct`):
- **Immutable**: `readonly` fields can only be assigned in the constructor; the reducer returns a new value.
- **Value-typed**: no heap allocation; cheap to copy.
- **Unity 6 compatible**: Unity 6 ships C# 9.0, which doesn't include `record struct` (C# 10) or `with` expressions on non-record structs. Plain `readonly struct` works everywhere. On .NET 6+ outside Unity you can use `readonly record struct` if you prefer the brevity; Spoon's `TState : struct` constraint accepts either.

Spoon enforces the `struct` constraint on `IReducer<TState>` and `IStore<TState>`.

### 2. Actions

Plain C# types implementing the `IAction` marker. Use `readonly struct`:

```csharp
public readonly struct SetVolumeAction : IAction
{
    public readonly float Value;
    
    public SetVolumeAction(float value) 
    {
        Value = value;
    }
}

public readonly struct SetLanguageAction : IAction
{
    public readonly string Value;
    
    public SetLanguageAction(string value) 
    {
        Value = value;
    }
}

public readonly struct MuteAction : IAction { }
```

Parameter-less actions (commands without payload) are empty `readonly struct`s. Pattern matching in the reducer handles both forms.

### 3. Reducer

Implements `IReducer<TState>`. Two members:

- `TState InitialState { get; }`: the starting snapshot for a fresh store.
- `TState Reduce(TState state, IAction action)`: pure transformation: same state + same action → same new state, no side effects.

The idiomatic body is a `switch` expression on the action type:

```csharp
public GameSettings Reduce(GameSettings state, IAction action) => action switch
{
    SetVolumeAction   a => new GameSettings(a.Value,      state.Language),
    SetLanguageAction a => new GameSettings(state.Volume, a.Value),
    MuteAction          => new GameSettings(0f,           state.Language),
    _                   => state,
};
```

The `_ => state` arm ignores unhandled actions, so a reducer tolerates actions injected by middleware that it does not handle.

### 4. Middleware

Implements `IMiddleware<TState>`. A single method:

```csharp
void Invoke(IStore<TState> store, IAction action, SpoonDispatch next);
```

`SpoonDispatch` is a named delegate for `(IAction action) -> void`. Naming it keeps the middleware signature readable.

Each middleware decides what to do with the action:

- **Pass it on**: call `next(action)` and the action flows through to the next middleware (or the reducer, if this is the last one).
- **Transform it**: call `next(someOtherAction)` to replace the action mid-pipeline.
- **Swallow it**: return without calling `next`, the reducer never sees the action.
- **Observe it**: call `next(action)` then inspect `store.State` (now post-reduce).

Middleware runs in Russian-doll order: first-registered wraps last-registered. With `[outer, inner]`:

```
outer.before → inner.before → [reducer] → inner.after → outer.after
```

Outer middleware sees the fully-reduced state after `next(action)` returns, which is the ideal place for logging, Bell republishing, or analytics.

## Registering a store

The `AddSpoonStore<TState, TReducer>` extension bundles reducer + middleware-collection + store registrations into one call. Features compose this into their own `Use{Feature}Feature` extension:

```csharp
public static IConfigurableCollection UseGameSettingsFeature(this ApplicationBuilder builder)
{
    return builder.AddSpoonStore<GameSettings, GameSettingsReducer>();
}

// With middleware:
public static IConfigurableCollection UseGameSettingsFeature(this ApplicationBuilder builder)
{
    return builder.AddSpoonStore<GameSettings, GameSettingsReducer>(
        new LoggingMiddleware(),
        new AnalyticsMiddleware());
}
```

Middleware order in the call is the pipeline order, so `LoggingMiddleware` wraps `AnalyticsMiddleware`.

## Reading and observing state

```csharp
var store = Application<IStore<GameSettings>>.Get();

// Read current snapshot.
var volume = store.State.Volume;

// Observe future changes. Dispose the returned IDisposable to unsubscribe.
// The 'in' is optional in the lambda; the compiler infers it from SpoonObserver<TState>.
using var sub = store.Subscribe((in GameSettings settings) =>
{
    Console.WriteLine($"Volume: {settings.Volume}");
});
```

Subscriber callbacks fire synchronously after each dispatch, in registration order, with the post-reduce state snapshot.

Subscribers that dispose themselves mid-callback are safe, because the store uses a copy-on-write observer array. Unsubscribing during iteration does not disturb the in-flight notification.

## Async flows

Spoon does not ship a dedicated async action contract. Write a normal `async Ticket` method that dispatches when ready:

```csharp
public static async Ticket LoadSettingsFromServer(IUserApi api, IStore<GameSettings> store)
{
    var remote = await api.FetchSettingsAsync();
    
    store.Dispatch(new SetVolumeAction(remote.Volume));
    store.Dispatch(new SetLanguageAction(remote.Language));
}
```

The caller awaits your method; internally, each `Dispatch` call runs synchronously through the pipeline. Middleware sees every one of those dispatches as a normal action.

If you need middleware to intercept an "async work starting" concept specifically, wrap it in a dedicated action type (`BeginLoadSettings`) and dispatch that before awaiting.

## Cross-feature dispatch

Each feature has its own `Store<TState>`. Features can dispatch to other features' stores by injecting both. No special API is required:

```csharp
public sealed class ProfileMiddleware : IMiddleware<GameSettings>
{
    private readonly IStore<AnalyticsState> m_Analytics;

    public ProfileMiddleware(IStore<AnalyticsState> analytics) { m_Analytics = analytics; }

    public void Invoke(IStore<GameSettings> store, IAction action, SpoonDispatch next)
    {
        next(action);
        m_Analytics.Dispatch(new ActionTrackedAction(action.GetType().Name));
    }
}
```

Middleware injected via Buttr can hold references to any other store. Cross-feature coupling is explicit and traceable.

## Re-entrancy

Calling `Dispatch` from inside a reducer or middleware throws `InvalidOperationException`. The same guard covers `Restore`: calling it during an active dispatch, from a reducer, middleware or subscriber callback, also throws. The reasons:

- Reducers must be pure (no side effects). Dispatching from inside a reducer implies a side effect.
- Chained dispatches make action order non-deterministic when observers also dispatch.
- Mid-dispatch `Restore` would clobber the in-flight state transition under the subscribers' feet.

If you need a follow-up dispatch in response to some action, do it outside the pipeline:

- From a subscriber callback, noting that subscribers which dispatch can re-enter.
- From an async method that awaits the trigger and then dispatches.

## Thread safety

Spoon is single-threaded by design. All operations on a `Store<TState>` must come from the same thread, typically the game-loop thread.

This is an intentional constraint that keeps the store lock-free and allocation-free on the hot path. Games are almost always single-threaded for game logic; if multi-thread dispatch becomes necessary, wrap the store behind a thread-safe queue at the call site.

## Composing with other CLabs packages

- **Belfry**: use the `spoon-belfry` bridge to republish state-change events as Belfry messages. Lets distant systems (HUD, analytics, networking) react without importing Spoon's types.

## Anti-patterns

- **Don't dispatch from inside a reducer.** Throws.
- **Don't store mutable state inside a `TState` struct** (lists, dictionaries). Use immutable collections or record-struct snapshots.
- **Don't depend on subscriber ordering for correctness.** Subscribers fire in registration order but your feature should not rely on "subscriber B always runs after subscriber A."
- **Don't use Spoon for per-frame transient data.** The ceremony isn't worth it. Plain fields or entity-scoped `Owner + Registry` are the right tools.
- **Don't subscribe from inside a subscriber callback.** Works but hard to reason about; schedule new subscriptions from outside the dispatch cycle.

## Testing a feature that uses Spoon

Construct `Store<T>` directly in tests. No Buttr container is needed:

```csharp
[Fact]
public void Setting_The_Volume_Above_One_Clamps_To_One()
{
    var store = new Store<GameSettings>(
        new GameSettingsReducer(),
        new MiddlewareCollection<GameSettings>(new ClampVolumeMiddleware()));

    store.Dispatch(new SetVolumeAction(3.5f));

    Assert.Equal(1f, store.State.Volume);
}
```

See the `SpoonBasics` cluster tests for more patterns.
