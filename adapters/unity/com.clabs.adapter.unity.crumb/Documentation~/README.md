# CLabs Adapter: Crumb (Unity)

Unity bindings for `com.clabs.crumb`. Supplies a `Debug.Log` sink, a `ScriptableObject`-backed configuration, an application loader that wires the package up with the Unity-specific defaults, and an editor window for inspecting live loggers.

## What this provides

| Type | Purpose |
|---|---|
| `UnityCrumbSink` | `ICrumbSink` that maps levels to `Debug.Log` / `Debug.LogWarning` / `Debug.LogError` |
| `CrumbConfigurationSO` | `ScriptableObject` implementing `ICrumbConfiguration` (file logging, log dir under `Application.persistentDataPath`, rotation, default filters) |
| `CrumbApplicationLoader` | `UnityApplicationLoaderBase` SO that builds a Crumb application container with `CrumbConfigurationSO` and `UnityCrumbSink` overrides applied |
| `CrumbManagerWindow` | `Window > Crumpet Labs > Crumb Manager`: live per-logger toggles and filter chips during Play mode |

## Setup

### Option A: use the application loader (recommended)

1. Create a `CrumbConfigurationSO` asset: `Create > CLabs > Crumb > Configuration`.
2. Create a `CrumbApplicationLoader` asset: `Create > CLabs / Crumb / Application Loader`.
3. Assign the configuration SO to the loader's `m_Configuration` field.
4. Add the loader to your `UnityApplicationBoot`'s loader list.

The loader builds an `ApplicationBuilder`, calls `UseCrumbPackage()`, then overrides `ICrumbConfiguration` and `ICrumbSink` via the returned `IConfigurableCollection`:

```csharp
var builder = new ApplicationBuilder();
builder.UseCrumbPackage()
    .WithImplementation<ICrumbConfiguration>(() => m_Configuration)
    .WithImplementation<ICrumbSink>(() => new UnityCrumbSink());
m_Application = builder.Build();
```

### Option B: register Crumb yourself inside a larger loader

If your project has a single composite loader that registers many packages, replicate the override block manually:

```csharp
public override Awaitable LoadAsync(CancellationToken cancellationToken) {
    var builder = new ApplicationBuilder();
    builder.UseCrumbPackage()
        .WithImplementation<ICrumbConfiguration>(() => m_CrumbConfig)
        .WithImplementation<ICrumbSink>(() => new UnityCrumbSink());
    // ... other packages ...
    m_App = builder.Build();
    return AwaitableUtility.CompletedTask;
}
```

The overrides work because `UseCrumbPackage()` registers both ports with the two-argument `AddSingleton<TAbstract, TConcrete>()` form. `WithImplementation<TAbstract>` scans the collection by abstract type and accepts any implementation of it, which is why the loader can supply a `CompositeCrumbSink` where the default was a `ConsoleCrumbSink`.

## Dependencies

- `com.clabs.crumb`: core logger + filter framework
- `com.crumpetlabs.buttr` + `com.crumpetlabs.buttr.unity`: DI and `UnityApplicationLoaderBase`
- `com.clabs.adapter.unity.editor`: `CLabsEditorWindow` framework

## See also

- The `com.clabs.crumb` package's `Documentation~/Guide.md`: full logger walkthrough.
- [../Code-Index.md](../Code-Index.md): auto-generated public surface listing.
