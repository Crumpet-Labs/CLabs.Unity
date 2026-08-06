# Spoon by Example

A recipe cookbook for people who've never used Redux. Read the preamble once, then jump to whichever recipe you need.

## The mental model

A Spoon store holds one piece of game state: a snapshot of what the settings look like right now.

You can **read** what's in the box at any time. You can't reach in and change it directly. Instead, you push an **action** into the store: a message describing what happened, such as the user turning the volume down, or the language changing to French.

The store runs the action through a **reducer**, a pure function that takes the current state plus the action and returns the **new** state. Then anyone who **subscribed** to the store gets handed the new snapshot.

That's the whole loop. No events to wire up, no setters to expose, no thread-safety to think about.

```
            Dispatch(action)
                   │
                   ▼
        ┌─── middleware ───┐
        │  (optional)      │
        └─────────┬────────┘
                  ▼
              reducer
                  │
                  ▼
              new state
                  │
                  ▼
            subscribers notified
```

Concepts in one breath:

- **State**: a `readonly struct` holding the data.
- **Action**: a `readonly struct` implementing `IAction`, describing *what happened*.
- **Reducer**: a class implementing `IReducer<TState>`; pure function `(state, action) → new state`.
- **Store**: `IStore<TState>`; you get one per feature, registered with Buttr.
- **Middleware**: optional `IMiddleware<TState>` hooks that wrap dispatch (logging, validation, side effects).
- **Subscriber**: `SpoonObserver<TState>` callback invoked after each state change.

> Unity 6 ships C# 9.0, which doesn't include `record struct` (that's C# 10) or `with` expressions on non-record structs. Examples in this doc use plain `readonly struct` so they work in Unity as well as pure .NET. The `com.clabs.adapter.unity.spoon` package documents the canonical Unity wiring pattern (a `sealed partial class : MonoBehaviour` with `[Inject] private IStore<TState> i_Store;`) and ships a Spoon Stores editor window for live state inspection in Play mode.

---

## Recipes

Each recipe is independent. Code snippets are copy-pasteable; types defined in early recipes get reused in later ones.

### 1. Define a state shape

State is a value type. A `readonly struct` with `readonly` fields gives you immutability and stack allocation:

```csharp
public readonly struct GameSettings
{
    public readonly float  Volume;
    public readonly string Language;

    public GameSettings(float volume, string language)
    {
        Volume   = volume;
        Language = language;
    }
}
```

Keep state small and serializable. If a field needs to be a list or dictionary, prefer an immutable collection or a fixed-size struct.

### 2. Define your first action

Actions describe *what happened*. They carry just enough data to update the state.

```csharp
public readonly struct SetVolumeAction : IAction
{
    public readonly float Value;
    public SetVolumeAction(float value) { Value = value; }
}

public readonly struct SetLanguageAction : IAction
{
    public readonly string Value;
    public SetLanguageAction(string value) { Value = value; }
}

public readonly struct MuteAction : IAction { }
```

`IAction` is a marker interface with no methods. Anything that implements it is dispatchable. Parameter-less actions (commands without payload) can be empty `readonly struct`s.

### 3. Write a reducer

The reducer is where state changes happen. It takes the current state and an action, and returns the new state. **Same input, same output, no side effects.**

```csharp
public sealed class SettingsReducer : IReducer<GameSettings>
{
    public GameSettings InitialState => new(Volume: 1.0f, Language: "en");

    public GameSettings Reduce(GameSettings state, IAction action) => action switch
    {
        SetVolumeAction   a => new GameSettings(a.Value,      state.Language),
        SetLanguageAction a => new GameSettings(state.Volume, a.Value),
        MuteAction          => new GameSettings(0f,           state.Language),
        _                   => state,
    };
}
```

Each handled arm returns a brand-new `GameSettings` carrying the changed field plus the unchanged ones. The `_ => state` arm ignores actions the reducer does not recognise. Middleware can dispatch actions of its own, and a reducer is not required to handle them.

### 4. Stand up a store directly (for tests)

In tests you usually don't want Buttr's DI container. Construct `Store<T>` by hand:

```csharp
var store = new Store<GameSettings>(
    new SettingsReducer(),
    new MiddlewareCollection<GameSettings>());

// store.State is GameSettings { Volume = 1.0f, Language = "en" }
```

If you don't need middleware, pass an empty `MiddlewareCollection<GameSettings>()`. Recipe 8 shows how to add middleware to this constructor.

### 5. Register a store in your app (the production wiring)

`AddSpoonStore<TState, TReducer>` registers the reducer, an empty middleware collection, and the store as one call:

```csharp
public static IConfigurableCollection UseSettingsFeature(this ApplicationBuilder builder)
{
    return builder.AddSpoonStore<GameSettings, SettingsReducer>();
}
```

Retrieve it after `builder.Build()`:

```csharp
var store = Application<IStore<GameSettings>>.Get();
```

> Spoon has no `UseSpoonPackage()` step, unlike other CLabs packages. It registers nothing globally; every store is per-feature.

### 6. Dispatch an action

Push an action through the slot:

```csharp
var store = Application<IStore<GameSettings>>.Get();

Console.WriteLine(store.State.Volume); // 1.0
store.Dispatch(new SetVolumeAction(0.5f));
Console.WriteLine(store.State.Volume); // 0.5
```

`Dispatch` is synchronous. By the time it returns, the new state is visible and every subscriber has been notified.

### 7. Subscribe to state changes

Hand the store a callback and it'll run it on every state change. Dispose the returned `IDisposable` to stop receiving.

```csharp
using var sub = store.Subscribe((in GameSettings s) =>
{
    Console.WriteLine($"Volume now: {s.Volume}");
});

store.Dispatch(new SetVolumeAction(0.25f));
// Prints: Volume now: 0.25
```

The `in` keyword passes the state by reference, so the struct is not copied per subscriber. If you omit it (`s =>`), the compiler infers it from `SpoonObserver<TState>` anyway, but writing it makes the read-only intent explicit.

Subscribers receive every state change, even when the reducer didn't actually modify state (dispatching `new UnhandledAction()` still notifies).

### 8. Add a logging middleware

Middleware wraps `Dispatch`. The simplest case: log before and after.

```csharp
var logger = new LambdaMiddleware<GameSettings>((store, action, next) =>
{
    Console.WriteLine($"[before] {action.GetType().Name}");
    next(action);
    Console.WriteLine($"[after ] volume={store.State.Volume}");
});

var store = new Store<GameSettings>(
    new SettingsReducer(),
    new MiddlewareCollection<GameSettings>(logger));
```

`LambdaMiddleware<TState>` is the inline-lambda shortcut. The `next` parameter is a `SpoonDispatch`, a named delegate for `(IAction action) -> void`. Calling it passes the action to the next middleware in the chain (or to the reducer if you're the last one).

With Buttr:

```csharp
builder.AddSpoonStore<GameSettings, SettingsReducer>(logger);
```

### 9. Add a validating middleware (transform an action)

Middleware can rewrite an action before it reaches the reducer:

```csharp
var clamp = new LambdaMiddleware<GameSettings>((_, action, next) =>
{
    if (action is SetVolumeAction v && v.Value > 1f)
        next(new SetVolumeAction(1f));   // clamp and forward a corrected action
    else
        next(action);                     // forward unchanged
});

var store = new Store<GameSettings>(
    new SettingsReducer(),
    new MiddlewareCollection<GameSettings>(clamp));

store.Dispatch(new SetVolumeAction(3.5f));
// store.State.Volume == 1.0f
```

Middleware can also **swallow** an action by returning without calling `next`. The reducer never sees it.

### 10. Broadcast state changes to distant systems

The `spoon-belfry` bridge republishes state changes as Belfry messages. UI, networking, analytics, or anything else can react without taking a direct Spoon dependency.

```json
"dependencies": {
    "com.clabs.spoon":               "1.0.0",
    "com.clabs.bridge.spoon-belfry": "0.1.0"
}
```

```csharp
builder.AddSpoonStore<GameSettings, SettingsReducer>();
builder.AddSpoonBelfryBridge<GameSettings>("game-settings");
```

A listener anywhere in the app:

```csharp
var tower = Application<IBellTower>.Get();
using var sub = tower.Rope("game-settings")
    .OnBell<SpoonStateChangedMessage<GameSettings>>(
        (in SpoonStateChangedMessage<GameSettings> msg) =>
        {
            Console.WriteLine($"Volume changed: {msg.State.Volume}");
        });
```

### 11. Undo a change (time-travel)

`Restore(state)` writes any snapshot directly into the store, bypassing the reducer, and notifies subscribers. The typical use is loading a saved snapshot back in, but it works as the foundation for undo / time-travel debugging too.

```csharp
// Push the current state onto an undo stack.
var snapshot = store.State;

// Make some changes...
store.Dispatch(new SetVolumeAction(0.1f));

// Roll back.
store.Restore(snapshot);
// store.State == snapshot
```

> Don't call `Restore` from inside a dispatch (a reducer, middleware, or subscriber callback). It throws `InvalidOperationException`. Keep restore calls outside the dispatch cycle.

### 12. Cross-feature reactions (one store reacts to another)

Sometimes a change in feature A should trigger a dispatch into feature B. Do this from a middleware holding references to both stores rather than from inside a reducer:

```csharp
public sealed class SettingsAnalyticsMiddleware : IMiddleware<GameSettings>
{
    private readonly IStore<AnalyticsState> m_Analytics;

    public SettingsAnalyticsMiddleware(IStore<AnalyticsState> analytics)
    {
        m_Analytics = analytics;
    }

    public void Invoke(IStore<GameSettings> store, IAction action, SpoonDispatch next)
    {
        next(action);
        m_Analytics.Dispatch(new ActionTrackedAction(action.GetType().Name));
    }
}
```

Buttr injects the other store. Cross-feature coupling becomes explicit and discoverable.

### 13. Test a reducer

Reducers are pure functions, so they can be tested directly without a Store:

```csharp
[Fact]
public void Mute_Action_Sets_Volume_To_Zero()
{
    var reducer = new SettingsReducer();
    var state = new GameSettings(Volume: 0.8f, Language: "en");

    var result = reducer.Reduce(state, new MuteAction());

    Assert.Equal(0f, result.Volume);
    Assert.Equal("en", result.Language); // unchanged
}
```

For testing middleware or the dispatch chain end-to-end, construct a `Store<T>` (see recipe 4) and dispatch into it.

### 14. Common mistakes

- **Don't mutate state in place.** Reducers return *new* state. Never write `state.Volume = …`. Construct a fresh struct via the constructor, carrying the new values for changed fields and `state.X` for everything else.
- **Don't dispatch from inside a reducer.** Throws `InvalidOperationException`. Reducers must stay pure. If you need a follow-up dispatch, do it from a middleware (after `next(action)`) or from outside the cycle.
- **Don't share a store across threads.** Spoon is single-threaded by design. Drive all dispatches from the game-loop thread.
- **Don't put live references in `TState`.** State should be data, not handles. Lists, dictionaries, and MonoBehaviour refs make non-destructive updates misleading and persistence impossible.
- **Don't reach for Spoon for per-frame data.** Position, velocity and AI state belong in components and entity registries. Spoon is for feature-level state that changes deliberately.

---

## What next?

- [Guide.md](Guide.md): full walkthrough of the four concepts plus async flows, cross-feature dispatch, and anti-patterns.
- [README.md](README.md): quick reference and when-to-use guidance.
