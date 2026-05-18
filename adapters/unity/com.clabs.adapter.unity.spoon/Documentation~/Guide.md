# Spoon Unity Adapter — Full Guide

This adapter is the Unity-side companion to engine-agnostic Spoon. Core Spoon has no `UnityEngine` reference; this adapter adds a debug window and documents the canonical pattern for wiring a Spoon store into a Unity MonoBehaviour.

If you're new to Spoon, read the core `com.clabs.spoon` package's `Documentation~/Guide.md` first. This guide assumes you know what a store, action, and reducer are.

## The canonical view pattern

Unity MonoBehaviours that bind to a Spoon store should follow the same `[Inject]` pattern every other CLabs adapter uses. Three rules:

1. **`sealed partial class : MonoBehaviour`** — `partial` is required for Buttr's source generator to populate `[Inject]` fields. `sealed` because there's no inheritance story for these in CLabs — every concrete view is its own leaf type.
2. **`[Inject] private IStore<TState> i_Store;`** — the `i_` prefix marks injected fields. Buttr resolves these at scene start. No `Application<T>.Get()` from MonoBehaviour code.
3. **Subscribe on `OnEnable`, dispose on `OnDisable`.** Push an immediate snapshot after subscribing so the view never starts stale.

Worked example:

```csharp
using System;
using Buttr.Injection;
using CLabs.Spoon;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class VolumeLabel : MonoBehaviour
{
    [Inject] private IStore<GameSettings> i_Store;

    [SerializeField] private Text m_Label;

    private IDisposable m_Subscription;

    private void OnEnable()
    {
        m_Subscription = i_Store.Subscribe(OnStateChanged);
        var snapshot = i_Store.State;
        OnStateChanged(in snapshot);
    }

    private void OnDisable()
    {
        m_Subscription?.Dispose();
        m_Subscription = null;
    }

    private void OnStateChanged(in GameSettings state)
    {
        m_Label.text = $"Volume: {state.Volume:P0}";
    }
}
```

Drop the component on a GameObject inside a scene that's loaded *after* the Buttr container is built. Buttr's source-gen populates `i_Store`; from there the lifecycle is yours.

## Dispatching from a view

A view can also dispatch back into the store — for buttons, sliders, input. Hold a `[SerializeField]` on the Unity control and forward changes:

```csharp
public sealed partial class VolumeSlider : MonoBehaviour
{
    [Inject] private IStore<GameSettings> i_Store;

    [SerializeField] private Slider m_Slider;

    private IDisposable m_Subscription;

    private void OnEnable()
    {
        m_Subscription = i_Store.Subscribe(OnStateChanged);
        var snapshot = i_Store.State;
        OnStateChanged(in snapshot);
        m_Slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDisable()
    {
        m_Slider.onValueChanged.RemoveListener(OnSliderChanged);
        m_Subscription?.Dispose();
        m_Subscription = null;
    }

    private void OnStateChanged(in GameSettings state)
    {
        if (!Mathf.Approximately(m_Slider.value, state.Volume))
            m_Slider.SetValueWithoutNotify(state.Volume);
    }

    private void OnSliderChanged(float v)
    {
        i_Store.Dispatch(new SetVolumeAction(v));
    }
}
```

The `SetValueWithoutNotify` guard prevents a feedback loop — dispatch → `OnStateChanged` → re-set slider → `onValueChanged` fires → re-dispatch → repeat. Always set Unity controls without notify when echoing state back to them.

## Selecting a sub-field

`OnStateChanged` always receives the full state. If your view only cares about one field, compare in your handler:

```csharp
private string m_LastLanguage;

private void OnStateChanged(in GameSettings state)
{
    if (state.Language == m_LastLanguage) return;
    m_LastLanguage = state.Language;
    RebuildLocalisedText();
}
```

Spoon doesn't ship a selector API — by design. `TState` is a small `readonly struct` and field comparison is cheap. If you find yourself wanting selectors across many views, the state probably wants splitting into smaller per-feature stores.

## Order of operations at scene startup

The most common Spoon-in-Unity failure: a MonoBehaviour enables *before* the Buttr container is built. `[Inject]` fields are `null` and the first call into `i_Store` NREs.

Standard CLabs bootstrap:

1. **App entry point** (a `UnityApplicationLoaderBase` subclass in a bootstrap scene) builds the Buttr container and registers all stores + bridges. This runs first.
2. **Game scenes load** *after* the container is ready.
3. Scene MonoBehaviours can now safely use their `[Inject]` fields in `OnEnable`.

If your scene loads in an editor-quick-start scenario without going through the bootstrap, you'll see `null` injections. Two mitigations:

- Make your bootstrap scene gate scene transitions behind container readiness.
- Add a "warn if missing" check at the top of `OnEnable`: `if (i_Store == null) { Debug.LogError("Container not built before " + name); return; }`.

## The Spoon Stores window

`Window > Crumpet Labs > Spoon Stores` opens an editor window that, **in Play mode only**, lists every `IStore<TState>` registered in the active Buttr container and renders the current state of the selected store field-by-field. The window refreshes live on `OnEditorUpdate`, so values change in real time as actions dispatch.

How discovery works:

- The window calls `Application<object>.All()` — Buttr's enumerator over every non-hidden registration. The filter (`typeof(object).IsAssignableFrom(...)`) matches everything, so the enumerator yields the resolved instance of every registered service.
- For each yielded instance, the window inspects the concrete type's interfaces. If any of them is `IStore<TState>` for some `TState`, the instance is recorded keyed by that state type. Other registrations (registries, services, middleware collections) are silently skipped.
- Discovery runs once per Play session and caches the result. If you build additional `ApplicationContainer`s during a single Play session (which Buttr supports — each container has its own resolvers and lifetime), the new containers' stores **won't appear in the window until** the cache refreshes. Two ways to refresh: exit and re-enter Play, or close and reopen the window.

How rendering works:

- For the selected store, the window reads `IStore.State` through a cached `PropertyInfo` and reflects on the struct's public fields and parameter-less properties.
- Each field/property gets one row: name on the left, `.ToString()` of the value on the right.
- Nested structs aren't expanded — they show their default `ToString()`. For complex state, override `ToString()` on your struct or split the state into smaller stores.

Limitations on purpose for v1.0:

- **Play mode only.** Buttr's registry is populated by `builder.Build()`; nothing is shown in Edit mode.
- **No history / no dispatch UI.** Adding action history would mean tracking a ring buffer inside `Store<TState>` — that's core Spoon scope, deferred to a later release.
- **No middleware introspection.** Middleware is a closure built at store-construction time; there's no clean way to enumerate it post-build.

The window is a debugging aid, not a control surface. Treat it like the Unity Inspector for Spoon state.

## Unity 6 syntax — why `readonly struct`

Unity 6 ships C# 9.0. Spoon's docs deliberately stick to the C# 9 feature set so the same code compiles in pure .NET *and* under Unity. The README in this folder has the full conventions table; here are the two that bite most often:

- **No `record struct`** — use `readonly struct` with `readonly` fields and an explicit constructor.
- **No `with` expressions on non-record structs** — reducer arms construct a new struct: `new GameSettings(a.Value, state.Language)` instead of `state with { Volume = a.Value }`.

If your code needs to compile in both pure .NET (newer C#) and Unity 6, stick with `readonly struct`. The verbosity is the cost of portability.

## Dependencies

- `com.clabs.spoon` — store + contracts.
- `com.clabs.adapter.unity.editor` — `CLabsEditorWindow` framework.
- `com.crumpetlabs.buttr` — `Application<T>` accessor (used here only inside editor code).
- `com.clabs.utility` — foundational types reachable via Spoon's surface.

Editor-only runtime footprint. The adapter ships no MonoBehaviour, no SO, no Unity runtime code. Just the inspector window and documentation.
