# CLabs Spoon

Feature-level state containers. Redux-style pattern: immutable state snapshot, pure reducer, middleware pipeline, observable subscriptions.

Engine-agnostic. Zero Unity dependency. Works in pure .NET tests.

## When to use Spoon

Reach for Spoon when a piece of feature state:

- Is mutated through **more than one code path** (want centralized rules).
- Has **more than one observer** that cares when it changes (UI + analytics + networking).
- Would benefit from **audit, replay, or time-travel** debugging.

**Good fits:** game settings, UI navigation state, unlocked-content totals, multiplayer match state, tutorial progress flags, any state that needs to converge across networked peers.

**Not for:** per-frame transient gameplay data (velocity, AI state), per-entity data (use the entity-scoped `Owner + Registry` pattern), physics state (Unity owns that).

## Where Spoon sits next to Registry

| Need | Reach for |
|---|---|
| "Which live objects exist right now, look them up by ID" | `Registry<TKey, TValue>` |
| "Current feature state with controlled mutation + subscribers" | Spoon (`IStore<TState>`) |

Spoon is the **in-memory, per-session** layer. Persistence integrations compose on top via separate bridge packages.

## Quick start

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

public sealed class GameSettingsReducer : IReducer<GameSettings>
{
    public GameSettings InitialState => new GameSettings(1.0f, "en");

    public GameSettings Reduce(GameSettings state, IAction action) => action switch
    {
        SetVolumeAction   a => new GameSettings(a.Value,      state.Language),
        SetLanguageAction a => new GameSettings(state.Volume, a.Value),
        _                   => state,
    };
}

public static IConfigurableCollection UseGameSettingsFeature(this ApplicationBuilder builder)
{
    return builder.AddSpoonStore<GameSettings, GameSettingsReducer>();
}
```

Consume:

```csharp
var store = Application<IStore<GameSettings>>.Get();
store.Dispatch(new SetVolumeAction(0.5f));

using var sub = store.Subscribe(s => Console.WriteLine($"Volume: {s.Volume}"));
store.Dispatch(new SetVolumeAction(0.3f));
```

## Middleware

```csharp
public sealed class LoggingMiddleware : IMiddleware<GameSettings>
{
    public void Invoke(IStore<GameSettings> store, IAction action, SpoonDispatch next)
    {
        Console.WriteLine($"[before] {action.GetType().Name}");
        next(action);
        Console.WriteLine($"[after ] volume={store.State.Volume}");
    }
}

public static IConfigurableCollection UseGameSettingsFeature(this ApplicationBuilder builder)
{
    return builder.AddSpoonStore<GameSettings, GameSettingsReducer>(new LoggingMiddleware());
}
```

## Constraints and guarantees

- `TState : struct` — state is a value type. `readonly struct` with `readonly` fields keeps it immutable and Unity-6 compatible (Unity 6 ships C# 9, so `record struct` and `with` expressions on non-record structs are unavailable). On .NET 6+ outside Unity, `readonly record struct` is equivalent and slightly less verbose.
- Single-threaded by design. All `Dispatch` and subscription calls must come from the same thread (typically the game-loop thread). No internal locking.
- Re-entrant `Dispatch` throws `InvalidOperationException`. Reducers and middleware must be pure.
- Subscribing and unsubscribing each allocate one new observer array (copy-on-write, so dispatch stays safe against mid-callback subscription changes). Dispatching boxes the action once per call when it's a value type — the `IAction` marker forces it. For Spoon's intended use (feature-level state, not per-frame data) that's well below noticeable. Observers receive state by `in TState` so the snapshot isn't copied per observer.
- Null actions throw `ArgumentNullException`.

## Further reading

- [Example.md](Example.md) — recipe cookbook for people new to Redux. Start here if you haven't used a store-based state container before.
- [Guide.md](Guide.md) — longer walkthrough with patterns for async, cross-feature dispatch, and Belfry re-publishing.
