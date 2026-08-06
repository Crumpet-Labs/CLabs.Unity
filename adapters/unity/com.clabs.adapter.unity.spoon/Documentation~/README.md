# CLabs Adapter: Spoon (Unity)

Unity-side companion for `com.clabs.spoon`. The core Spoon package is engine-agnostic: pure C#, with no `UnityEngine` reference. This adapter adds two things on top:

1. An **editor inspector window** (`Window > Crumpet Labs > Spoon Stores`) that lists every Spoon store live in the Buttr container and renders its state in real time during Play mode.
2. The **Unity-6-specific language conventions** Spoon's docs follow, so the same examples compile under Unity's C# 9.0 compiler.

There is no runtime code in this adapter. Unity consumers wire Spoon stores into MonoBehaviours via Buttr's `[Inject]` source-gen, exactly the same way every other CLabs adapter does. The canonical pattern is below.

## Canonical pattern: a Unity view of a Spoon store

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

Notes:

- `sealed partial class : MonoBehaviour`: Buttr's source generator requires `partial` to inject fields.
- `[Inject] private <Type> i_FieldName;`: the `i_` prefix marks injected fields (the rest of CLabs uses `m_` for serialized / regular fields).
- The store is resolved by Buttr at scene start. `Application<T>.Get()` is not used from MonoBehaviour code.
- Subscribe on `OnEnable`, dispose on `OnDisable`. Push an immediate snapshot after subscribing so the view never starts stale.

## What this adapter provides

### Spoon Stores window

A `CLabsEditorWindow` listing every `IStore<TState>` reachable from Buttr's global registration list. Discovery uses Buttr's own registry: `Application<object>.All()` enumerates every non-hidden registration across the process, the window checks each yielded instance's concrete type for an `IStore<>` interface, and pulls `TState` off its generic arguments. There is no assembly scanning and no `MakeGenericType` per tick. Discovery runs once per Play session, and the store instance is cached against the state type.

For the selected store the window renders the current state field-by-field via reflection on the struct's public fields and parameter-less properties. Refreshes live on `OnEditorUpdate` as actions dispatch.

Limitations in this release:

- **Play mode only.** Buttr's registry is populated by `builder.Build()`; nothing is shown in Edit mode.
- **One-shot discovery per Play session.** If you build a second `ApplicationContainer` mid-session, its stores won't appear until you exit/re-enter Play (or close/reopen the window).
- **Read-only.** No dispatch-from-window or action-history yet. Add those if a real need surfaces.
- **Nested struct fields show `.ToString()` only**, not expanded. For deep state, override `ToString()` on the struct or split into smaller stores.

## Unity 6 syntax conventions

Unity 6 ships **C# 9.0**. Modern C# features that Spoon's docs deliberately avoid:

| Feature | C# version | Spoon's stance |
|---|---|---|
| `record struct` | C# 10 | Not supported in Unity 6. Use a plain `readonly struct` with `readonly` fields. |
| `with` expressions on non-record structs | C# 10 | Not supported. Reducer arms build a new struct via the explicit constructor. |
| Primary constructors on classes | C# 12 | Not supported. Use a regular constructor. |
| File-scoped namespaces | C# 10 | Works under Unity 6's Roslyn; Spoon's docs use block-scoped for clarity. Use whichever style you prefer. |

The recommended state + action shape compiles everywhere (Unity 6 + .NET 6+):

```csharp
public readonly struct GameSettings
{
    public readonly float  Volume;
    public readonly string Language;
    public GameSettings(float volume, string language) { Volume = volume; Language = language; }
}

public readonly struct SetVolumeAction : IAction
{
    public readonly float Value;
    public SetVolumeAction(float value) { Value = value; }
}
```

If you also need `Assert.Equal`-style value comparison (typically in tests), implement `IEquatable<T>` yourself:

```csharp
public readonly struct GameSettings : IEquatable<GameSettings>
{
    // ... fields + ctor as above ...
    public bool Equals(GameSettings other) => Volume == other.Volume && Language == other.Language;
    public override bool Equals(object obj) => obj is GameSettings other && Equals(other);
    public override int GetHashCode() => (Volume, Language).GetHashCode();
}
```

`record struct` would generate all of that, at the cost of Unity 6 compatibility. The equivalent written by hand is a few extra lines per state type.

## Install

This adapter ships inside the `CLabs.Unity` UPM umbrella. Unity consumers add the umbrella to their `manifest.json`:

```json
{
  "dependencies": {
    "com.crumpetlabs.unity": "https://github.com/Crumpet-Labs/CLabs.Unity.git#v1.0.0"
  }
}
```

(Replace the tag with whichever unified version you're pinning to.) The adapter, its editor window, and the core Spoon package are all installed together as part of the umbrella.

There is no separate per-adapter UPM install path.

## Dependencies

- `com.clabs.spoon`: core store + contracts.
- `com.clabs.adapter.unity.editor`: `CLabsEditorWindow` framework + panels + status bar.
- `com.crumpetlabs.buttr`: `Application<T>` accessor (editor-only here).
- `com.clabs.utility`: foundational types reachable via Spoon's surface.

## See also

- [Guide.md](Guide.md): full walkthrough of the canonical view pattern, the editor window, and Unity bootstrap order.
- [../Code-Index.md](../Code-Index.md): auto-generated public surface listing.
- The `com.clabs.spoon` package's `Documentation~/Example.md`: recipe cookbook, in Unity-compatible syntax throughout.
- The `com.clabs.spoon` package's `Documentation~/Guide.md`: full Spoon walkthrough.
